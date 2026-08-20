using UnityEngine;

[CreateAssetMenu(
    fileName = "CardDefinition",
    menuName = "Conquian/Card Definition"
)]
public class CardDefinition : ScriptableObject
{
    [SerializeField] private CardSuit suit;
    [SerializeField] private CardRank rank;
    [SerializeField] private Sprite cardSprite;

    public CardSuit Suit => suit;
    public CardRank Rank => rank;
    public Sprite CardSprite => cardSprite;
}

public enum CardSuit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades,
    None
}

public enum CardRank
{
    Ace = 1,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Joker
}