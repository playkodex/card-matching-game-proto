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
    [SerializeField] private RectTransform containerRect;
    
    [Header("Layout Settings")]
    [SerializeField] private float spacing = 10f;
    [SerializeField] private float padding = 20f;
    [SerializeField] private float aspectRatio = 0.7f; // Width to height ratio for cards
    
    [Header("Card Images")]
    [SerializeField] private Sprite[] cardImages;
    [SerializeField] private Sprite cardBackImage;
    
    [Header("Scoring")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private UIManager uiManager;
    
    private List<Card> allCards = new List<Card>();
    private List<Card> flippedCards = new List<Card>();
    private int matchesFound = 0;
    private int totalMatches;
    private bool isProcessingMatches = false;
    
    private void Start()
    {
        if (scoreManager == null)
        {
            scoreManager = ScoreManager.Instance;
        }
        
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        
        if (containerRect == null && gridContainer != null)
        {
            containerRect = gridContainer.GetComponent<RectTransform>();
        }
        
        totalMatches = (rows * columns) / 2;
        SetupGrid();
        GenerateCards();
    }
    
    private void SetupGrid()
    {
        if (gridLayoutGroup == null || containerRect == null)
            return;
            
        // Set constraint to fixed column count
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;
        
        // Calculate available space
        float availableWidth = containerRect.rect.width - (padding * 2);
        float availableHeight = containerRect.rect.height - (padding * 2);
        
        // Calculate spacing between cards
        float totalHorizontalSpacing = spacing * (columns - 1);
        float totalVerticalSpacing = spacing * (rows - 1);
        
        // Calculate maximum card width and height
        float maxCardWidth = (availableWidth - totalHorizontalSpacing) / columns;
        float maxCardHeight = (availableHeight - totalVerticalSpacing) / rows;
        
        // Determine optimal size while maintaining aspect ratio
        float cardWidth, cardHeight;
        
        // Try to fit by width first
        cardWidth = maxCardWidth;
        cardHeight = cardWidth / aspectRatio;
        
        // If height exceeds available space, fit by height instead
        if (cardHeight > maxCardHeight)
        {
            cardHeight = maxCardHeight;
            cardWidth = cardHeight * aspectRatio;
        }
        
        // Apply calculated size
        gridLayoutGroup.cellSize = new Vector2(cardWidth, cardHeight);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
        gridLayoutGroup.padding = new RectOffset(
            Mathf.RoundToInt(padding),
            Mathf.RoundToInt(padding),
            Mathf.RoundToInt(padding),
            Mathf.RoundToInt(padding)
        );
        
        Debug.Log($"Grid Setup: {rows}x{columns}, Cell Size: {cardWidth}x{cardHeight}");
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
        // Add card to flipped cards list
        if (!flippedCards.Contains(card))
        {
            flippedCards.Add(card);
        }
        
        // Check for pairs when we have at least 2 flipped cards
        if (flippedCards.Count >= 2 && !isProcessingMatches)
        {
            // Increment moves when a pair is formed
            if (uiManager != null)
            {
                uiManager.IncrementMoves();
            }
            
            StartCoroutine(ProcessMatches());
        }
    }
    
    private IEnumerator ProcessMatches()
    {
        isProcessingMatches = true;
        
        yield return new WaitForSeconds(flipDelay);
        
        // Process pairs from the flipped cards list
        List<Card> cardsToRemove = new List<Card>();
        
        // Check cards in pairs
        for (int i = 0; i < flippedCards.Count - 1; i += 2)
        {
            if (i + 1 < flippedCards.Count)
            {
                Card firstCard = flippedCards[i];
                Card secondCard = flippedCards[i + 1];
                
                // Skip if either card is already matched
                if (firstCard.IsMatched() || secondCard.IsMatched())
                {
                    cardsToRemove.Add(firstCard);
                    cardsToRemove.Add(secondCard);
                    continue;
                }
                
                if (firstCard.GetCardId() == secondCard.GetCardId())
                {
                    // Match found
                    firstCard.SetMatched();
                    secondCard.SetMatched();
                    matchesFound++;
                    
                    // Add score for match
                    if (scoreManager != null)
                    {
                        scoreManager.AddMatchScore();
                    }
                    
                    if (matchesFound >= totalMatches)
                    {
                        Debug.Log("Game Complete! All matches found!");
                        
                        // Stop the timer
                        if (uiManager != null)
                        {
                            uiManager.StopGame();
                        }
                        
                        // You can add win screen logic here
                    }
                }
                else
                {
                    // No match, flip cards back
                    firstCard.ResetCard();
                    secondCard.ResetCard();
                    
                    // Apply penalty for mismatch
                    if (scoreManager != null)
                    {
                        scoreManager.AddMismatchPenalty();
                    }
                }
                
                cardsToRemove.Add(firstCard);
                cardsToRemove.Add(secondCard);
            }
        }
        
        // Remove processed cards from flipped list
        foreach (Card card in cardsToRemove)
        {
            flippedCards.Remove(card);
        }
        
        isProcessingMatches = false;
        
        // If there are still pairs to process, continue
        if (flippedCards.Count >= 2)
        {
            StartCoroutine(ProcessMatches());
        }
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
        flippedCards.Clear();
        isProcessingMatches = false;
        matchesFound = 0;
        
        // Reset score
        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }
        
        // Reset UI
        if (uiManager != null)
        {
            uiManager.ResetUI();
        }
        
        // Generate new cards
        GenerateCards();
    }
    
    // Helper method to change layout at runtime
    public void SetGridLayout(int newRows, int newColumns)
    {
        rows = newRows;
        columns = newColumns;
        totalMatches = (rows * columns) / 2;
        
        ResetGame();
    }
    
    // Preset layout methods
    public void SetLayout2x2() => SetGridLayout(2, 2);
    public void SetLayout3x3() => SetGridLayout(3, 3);
    public void SetLayout4x4() => SetGridLayout(4, 4);
    public void SetLayout4x5() => SetGridLayout(4, 5);
    public void SetLayout5x6() => SetGridLayout(5, 6);
    public void SetLayout6x6() => SetGridLayout(6, 6);
}
