using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hover")]
    [SerializeField] private float hoverHeight = 0.5f;
    [SerializeField] private float hoverDuration = 0.15f;
    [Header("Touch")]
    [SerializeField] private float longPressDuration = 0.35f;
    [SerializeField] private float dragStartDistance = 10f;

    private SpriteRenderer spriteRenderer;
    private CardDefinition definition;

    private Vector3 handPosition;
    private int handSortingOrder;

    private bool isHovered;
    private bool isDragging;
    private bool isPressing;
    private float pressStartTime;
    private Vector2 pressStartScreenPosition;
    private Vector3 dragOffset;

    private Camera mainCamera;

    private PointerEventData activePointerEventData;

    private void Awake()
    {
        mainCamera = Camera.main;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (!isPressing)
            return;

        // Hvis man har holdt fingeren nede længe nok, kan man bevæge kortet.
        if (!isDragging)
            CheckForDragStart();

        if (isDragging)
            FollowTouch();
    }

    public void Initialize(CardDefinition cardDefinition)
    {
        definition = cardDefinition;
        spriteRenderer.sprite = definition.CardSprite;
    }
    public void SetSortingOrder(int order)
    {
        handSortingOrder = order;
        if (!isHovered && !isDragging)
            spriteRenderer.sortingOrder = order;
    }

    public void SetHandPosition(Vector3 position)
    {
        handPosition = position;
        if (!isDragging)
            transform.DOMove(handPosition, 0.25f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        isPressing = true;
        // Gem den PointerEventData, som startede interaktionen.
        activePointerEventData = eventData;
        // Start tracking the press duration and position.
        pressStartTime = Time.time;
        pressStartScreenPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!isPressing)
            return;

        isPressing = false;
        if (isDragging)
        {
            StopDragging();
            return;
        }

        // Hvis fingeren blev sluppet hurtigt, bliver kortet løftet/hovered.
        float pressDuration = Time.time - pressStartTime;
        if (pressDuration < longPressDuration)
            ToggleHover();

        activePointerEventData = null;
    }

    private void ToggleHover()
    {
        if (isHovered)
            LowerCard();
        else
            LiftCard();
    }

    private void CheckForDragStart()
    {
        float pressDuration = Time.time - pressStartTime;

        if (pressDuration < longPressDuration)
            return;

        if (activePointerEventData == null)
            return;

        Vector2 currentScreenPosition = activePointerEventData.position;

        float distance = Vector2.Distance(pressStartScreenPosition,currentScreenPosition);

        if (distance >= dragStartDistance)
            StartDragging();
    }

    public void LiftCard()
    {
        isHovered = true;

        transform.DOKill();
        transform.DOMove(handPosition + Vector3.up * hoverHeight,hoverDuration).SetEase(Ease.OutQuad);

        spriteRenderer.sortingOrder = 100;
    }
    public void LowerCard()
    {
        if (isDragging)
            return;

        isHovered = false;

        transform.DOKill();
        transform.DOMove(handPosition,hoverDuration).SetEase(Ease.OutQuad);

        spriteRenderer.sortingOrder = handSortingOrder;
    }

    private void StartDragging()
    {
        if (isDragging)
            return;

        if (activePointerEventData == null)
            return;

        isDragging = true;
        isHovered = false;
        transform.DOKill();

        Vector3 touchWorldPosition = GetTouchWorldPosition();
        dragOffset = transform.position - touchWorldPosition;

        // Make the dragged card appear above all other cards.
        spriteRenderer.sortingOrder = 1000;
    }

    private void StopDragging()
    {
        if (!isDragging)
            return;

        isDragging = false;
        transform.DOKill();
        transform.DOMove(handPosition,hoverDuration).SetEase(Ease.OutQuad);
        spriteRenderer.sortingOrder = handSortingOrder;
        activePointerEventData = null;
    }

    private void FollowTouch()
    {
        if (activePointerEventData == null)
            return;

        Vector3 touchWorldPosition = GetTouchWorldPosition();
        Vector3 targetPosition = touchWorldPosition + dragOffset;

        // Behold kortets eksisterende Z-position.
        targetPosition.z = transform.position.z;
        transform.position = targetPosition;
    }

    private Vector3 GetTouchWorldPosition()
    {
        if (activePointerEventData == null)
            return transform.position;

        Vector2 touchPosition = activePointerEventData.position;
        float distanceFromCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 screenPosition = new Vector3(touchPosition.x,touchPosition.y,distanceFromCamera);
        return mainCamera.ScreenToWorldPoint(screenPosition);
    }
}
