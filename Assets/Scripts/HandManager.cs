using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class HandManager : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHandSize = 15;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private DeckManager deckManager;
    private List<Card> handCards = new();

    private void Start()
    {
        StartCoroutine(Delay());
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            DrawCard();

        CheckForTouch();
    }

    IEnumerator Delay()
    {
        // Dealing up to the configured max hand size to users
        for (int i = 0; i < maxHandSize; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void DrawCard()
    {
        CardDefinition cardDefinition = deckManager.DrawCard();

        if (handCards.Count >= maxHandSize || cardDefinition == null)
            return;

        GameObject cardObject = Instantiate(
            cardPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Card card = cardObject.GetComponent<Card>();

        card.Initialize(cardDefinition);
        handCards.Add(card);
        UpdateCardPositions();
    }

    private void UpdateCardPositions()
    {
        if (handCards.Count == 0)
            return;

        float cardSpacing = 1f / handCards.Count; // Adjust spacing based on the number of cards
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2; // Center the cards around the middle of the spline

        Spline spline = splineContainer.Spline;
        for (int i = 0; i < handCards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            handCards[i].SetHandPosition(splinePosition);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 25f);
            handCards[i].SetSortingOrder(i);
        }
    }

    // This method is called to lower all cards in the hand which is currently highlighted
    public void ClearHoverCard()
    {
        foreach (Card card in handCards)
        {
            card.LowerCard();
        }
    }

    // This method checks if the user has touched the screen and if the touch is not on any card, it clears the hover state of all cards
    private void CheckForTouch()
    {
        if (Touchscreen.current == null)
            return;

        if (!Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return;

        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        PointerEventData pointerData = new PointerEventData(EventSystem.current);

        pointerData.position = touchPosition;

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, results);

        bool touchedCard = false;

        foreach (RaycastResult result in results)
        {
            Card card = result.gameObject.GetComponentInParent<Card>();

            if (card != null)
            {
                touchedCard = true;
                break;
            }
        }

        if (!touchedCard)
            ClearHoverCard();
    }
}