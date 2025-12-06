using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 4;
    [SerializeField] private float flipDelay = 1f;
    
    [Header("UI References")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    
    [Header("Card Images")]
    [SerializeField] private Sprite[] cardImages;
    [SerializeField] private Sprite cardBackImage;
    
    private List<Card> allCards = new List<Card>();
    private Card firstFlippedCard;
    private Card secondFlippedCard;
    private bool isCheckingMatch = false;
    private int matchesFound = 0;
    private int totalMatches;
    
    private void Start()
    {
        totalMatches = (rows * columns) / 2;
        SetupGrid();
        GenerateCards();
    }
    
    private void SetupGrid()
    {
        if (gridLayoutGroup != null)
        {
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = columns;
        }
    }
    
    private void GenerateCards()
    {
        // Calculate total number of cards
        int totalCards = rows * columns;
        
        // Ensure we have enough images and even number of cards
        if (totalCards % 2 != 0)
        {
            Debug.LogError("Total cards must be an even number!");
            return;
        }
        
        int pairsNeeded = totalCards / 2;
        
        if (cardImages.Length < pairsNeeded)
        {
            Debug.LogWarning($"Not enough card images. Need {pairsNeeded}, have {cardImages.Length}");
        }
        
        // Create list of card IDs (pairs)
        List<int> cardIds = new List<int>();
        for (int i = 0; i < pairsNeeded; i++)
        {
            cardIds.Add(i % cardImages.Length);
            cardIds.Add(i % cardImages.Length);
        }
        
        // Shuffle the card IDs
        ShuffleList(cardIds);
        
        // Create cards in grid
        for (int i = 0; i < totalCards; i++)
        {
            CreateCard(cardIds[i]);
        }
    }
    
    private void CreateCard(int cardId)
    {
        GameObject cardObject = Instantiate(cardPrefab, gridContainer);
        
        Card card = cardObject.GetComponent<Card>();
        if (card != null)
        {
            card.SetCardId(cardId);
            if (cardId < cardImages.Length)
            {
                card.SetCardImage(cardImages[cardId]);
            }
            if (cardBackImage != null)
            {
                card.SetCardBackImage(cardBackImage);
            }
            allCards.Add(card);
        }
    }
    
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    
    public void CardFlipped(Card card)
    {
        if (firstFlippedCard == null)
        {
            firstFlippedCard = card;
        }
        else if (secondFlippedCard == null)
        {
            secondFlippedCard = card;
            StartCoroutine(CheckMatch());
        }
    }
    
    private IEnumerator CheckMatch()
    {
        isCheckingMatch = true;
        
        yield return new WaitForSeconds(flipDelay);
        
        if (firstFlippedCard.GetCardId() == secondFlippedCard.GetCardId())
        {
            // Match found
            firstFlippedCard.SetMatched();
            secondFlippedCard.SetMatched();
            matchesFound++;
            
            if (matchesFound >= totalMatches)
            {
                Debug.Log("Game Complete! All matches found!");
                // You can add win screen logic here
            }
        }
        else
        {
            // No match, flip cards back
            firstFlippedCard.ResetCard();
            secondFlippedCard.ResetCard();
        }
        
        firstFlippedCard = null;
        secondFlippedCard = null;
        isCheckingMatch = false;
    }
    
    public bool IsCheckingMatch()
    {
        return isCheckingMatch;
    }
    
    public void ResetGame()
    {
        // Clear all existing cards
        foreach (Card card in allCards)
        {
            Destroy(card.gameObject);
        }
        allCards.Clear();
        
        // Reset game state
        firstFlippedCard = null;
        secondFlippedCard = null;
        isCheckingMatch = false;
        matchesFound = 0;
        
        // Generate new cards
        GenerateCards();
    }
}
