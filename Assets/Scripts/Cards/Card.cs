using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover")]
    [SerializeField] private float hoverHeight = 0.5f;
    [SerializeField] private float hoverDuration = 0.15f;

    private int handSortingOrder;
    private SpriteRenderer spriteRenderer;
    private CardDefinition definition;
    private Vector3 handPosition;
    public CardDefinition Definition => definition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(CardDefinition cardDefinition)
    {
        definition = cardDefinition;
        spriteRenderer.sprite = definition.CardSprite;
    }
    public void SetSortingOrder(int order)
    {
        handSortingOrder = order;
        spriteRenderer.sortingOrder = order;
    }

    public void SetHandPosition(Vector3 position)
    {
        handPosition = position;
        transform.DOMove(handPosition,0.25f);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOMove(handPosition + Vector3.up * hoverHeight,hoverDuration).SetEase(Ease.OutQuad);

        spriteRenderer.sortingOrder = 100;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOMove(handPosition,hoverDuration).SetEase(Ease.OutQuad);

        spriteRenderer.sortingOrder = handSortingOrder;
    }
}
