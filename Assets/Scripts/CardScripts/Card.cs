using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private int cardId;
    [SerializeField] private Image cardFrontImage;
    [SerializeField] private Image cardBackImage;
    [SerializeField] private Button cardButton;
    
    private bool isFlipped = false;
    private bool isMatched = false;
    private CardGameManager gameManager;
    
    private void Awake()
    {
        gameManager = FindObjectOfType<CardGameManager>();
        
        if (cardButton == null)
            cardButton = GetComponent<Button>();
            
        if (cardButton != null)
            cardButton.onClick.AddListener(OnCardClicked);
    }
    
    public void SetCardId(int id)
    {
        cardId = id;
    }
    
    public int GetCardId()
    {
        return cardId;
    }
    
    public void SetCardImage(Sprite image)
    {
        if (cardFrontImage != null)
        {
            cardFrontImage.sprite = image;
        }
    }
    
    public void SetCardBackImage(Sprite image)
    {
        if (cardBackImage != null)
        {
            cardBackImage.sprite = image;
        }
    }
    
    public void FlipCard()
    {
        if (isMatched || gameManager.IsCheckingMatch())
            return;
            
        isFlipped = !isFlipped;
        
        if (cardFrontImage != null && cardBackImage != null)
        {
            cardFrontImage.gameObject.SetActive(isFlipped);
            cardBackImage.gameObject.SetActive(!isFlipped);
        }
        
        if (isFlipped)
        {
            gameManager.CardFlipped(this);
        }
    }
    
    public void ResetCard()
    {
        isFlipped = false;
        if (cardFrontImage != null && cardBackImage != null)
        {
            cardFrontImage.gameObject.SetActive(false);
            cardBackImage.gameObject.SetActive(true);
        }
    }
    
    public void SetMatched()
    {
        isMatched = true;
        if (cardButton != null)
            cardButton.interactable = false;
    }
    
    public bool IsMatched()
    {
        return isMatched;
    }
    
    public bool IsFlipped()
    {
        return isFlipped;
    }
    
    private void OnCardClicked()
    {
        if (!isMatched && !isFlipped)
        {
            FlipCard();
        }
    }
    
    private void OnDestroy()
    {
        if (cardButton != null)
            cardButton.onClick.RemoveListener(OnCardClicked);
    }
}
