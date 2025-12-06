using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField] private int cardId;
    [SerializeField] private GameObject cardFrontHolder;
    [SerializeField] private Image cardFrontImage;
    [SerializeField] private Image cardBackImage;
    [SerializeField] private Button cardButton;
    [SerializeField] private RectTransform cardTransform;
    [SerializeField] private float flipDuration = 0.3f;
    
    private bool isFlipped = false;
    private bool isMatched = false;
    private bool isAnimating = false;
    private CardGameManager gameManager;
    
    private void Awake()
    {
        gameManager = FindObjectOfType<CardGameManager>();
        
        if (cardButton == null)
            cardButton = GetComponent<Button>();
            
        if (cardTransform == null)
            cardTransform = GetComponent<RectTransform>();
            
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
        if (isMatched || isAnimating)
            return;
            
        isFlipped = !isFlipped;
        AnimateFlip();
        
        if (isFlipped)
        {
            gameManager.CardFlipped(this);
        }
    }
    
    private void AnimateFlip()
    {
        isAnimating = true;
        
        // Play flip sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCardFlip();
        }
        
        // Rotate to 90 degrees (hide current side)
        cardTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Switch card sides at 90 degrees
                if (cardFrontHolder != null && cardBackImage != null)
                {
                    cardFrontHolder.SetActive(isFlipped);
                    cardBackImage.gameObject.SetActive(!isFlipped);
                }
                
                // Rotate to 180 degrees (show new side)
                cardTransform.DORotate(new Vector3(0, 180, 0), flipDuration / 2f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        isAnimating = false;
                    });
            });
    }
    
    public void ResetCard()
    {
        isFlipped = false;
        AnimateReset();
    }
    
    private void AnimateReset()
    {
        isAnimating = true;
        
        // Play flip sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCardFlip();
        }
        
        // Rotate back to 90 degrees
        cardTransform.DORotate(new Vector3(0, 90, 0), flipDuration / 2f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Switch back to card back
                if (cardFrontHolder != null && cardBackImage != null)
                {
                    cardFrontHolder.SetActive(false);
                    cardBackImage.gameObject.SetActive(true);
                }
                
                // Rotate back to 0 degrees
                cardTransform.DORotate(new Vector3(0, 0, 0), flipDuration / 2f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        isAnimating = false;
                    });
            });
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
    
    public void ShowFaceInstant()
    {
        isFlipped = true;
        
        // Show front side without animation
        if (cardFrontHolder != null && cardBackImage != null)
        {
            cardFrontHolder.SetActive(true);
            cardBackImage.gameObject.SetActive(false);
        }
        
        // Set rotation to show front (180 degrees)
        if (cardTransform != null)
        {
            cardTransform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
    
    private void OnCardClicked()
    {
        if (!isMatched && !isFlipped && !isAnimating)
        {
            FlipCard();
        }
    }
    
    private void OnDestroy()
    {
        if (cardButton != null)
            cardButton.onClick.RemoveListener(OnCardClicked);
            
        // Kill any active tweens
        cardTransform.DOKill();
    }
}
