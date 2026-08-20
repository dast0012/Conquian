using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            DrawCard();
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
            handCards[i].transform.DOMove(splinePosition, 0.25f);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 25f);
            handCards[i].SetSortingOrder(i);
        }
    }
}