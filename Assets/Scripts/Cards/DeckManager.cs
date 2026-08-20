using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private CardDefinition[] cardDefinitions;

    private List<CardDefinition> deck = new();

    public int CardsRemaining => deck.Count;

    private void Awake()
    {
        CreateDeck();
        ShuffleDeck();
    }

    private void CreateDeck()
    {
        deck.Clear();

        foreach (CardDefinition definition in cardDefinitions)
        {
            // Every CardDefinition represents one card type.
            // We have two copies of every type in the game.
            deck.Add(definition);
            deck.Add(definition);
        }
    }

    private void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            CardDefinition temporary = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temporary;
        }
    }

    public CardDefinition DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("The deck is empty.");
            return null;
        }

        int lastIndex = deck.Count - 1;

        CardDefinition drawnCard = deck[lastIndex];

        deck.RemoveAt(lastIndex);

        return drawnCard;
    }
}