using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Card : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private CardDefinition definition;

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
        spriteRenderer.sortingOrder = order;
    }
}
