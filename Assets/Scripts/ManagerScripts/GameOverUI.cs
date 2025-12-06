using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI finalMovesText;
    [SerializeField] private TextMeshProUGUI finalComboText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button quitButton;
    
    [Header("Settings")]
    [SerializeField] private string victoryMessage = "Victory!";
    
    [Header("Animation Settings")]
    [SerializeField] private float panelFadeDuration = 0.3f;
    [SerializeField] private float panelScaleDuration = 0.5f;
    [SerializeField] private float textStaggerDelay = 0.1f;
    [SerializeField] private float textAnimDuration = 0.4f;
    [SerializeField] private float buttonAnimDelay = 0.5f;
    
    private CardGameManager gameManager;
    
    private void Awake()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            
            // Get or add CanvasGroup for fade animation
            if (panelCanvasGroup == null)
            {
                panelCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
                if (panelCanvasGroup == null)
                {
                    panelCanvasGroup = gameOverPanel.AddComponent<CanvasGroup>();
                }
            }
            
            // Get RectTransform for scale animation
            if (panelRect == null)
            {
                panelRect = gameOverPanel.GetComponent<RectTransform>();
            }
        }
        
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }
    
    private void Start()
    {
        gameManager = FindObjectOfType<CardGameManager>();
    }
    
    public void ShowGameOver(int finalScore, float finalTime, int finalMoves, int finalCombo)
    {
        if (gameOverPanel == null)
            return;
        
        // Reset initial states
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0;
        }
        
        if (panelRect != null)
        {
            panelRect.localScale = Vector3.zero;
        }
        
        // Show panel
        gameOverPanel.SetActive(true);
        
        // Animate panel fade and scale in
        Sequence panelSequence = DOTween.Sequence();
        
        if (panelCanvasGroup != null)
        {
            panelSequence.Append(panelCanvasGroup.DOFade(1f, panelFadeDuration));
        }
        
        if (panelRect != null)
        {
            panelSequence.Join(panelRect.DOScale(1f, panelScaleDuration).SetEase(Ease.OutBack));
        }
        
        // Animate title with bounce
        if (titleText != null)
        {
            titleText.text = victoryMessage;
            titleText.transform.localScale = Vector3.zero;
            titleText.transform.DOScale(1f, textAnimDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(panelScaleDuration * 0.5f);
        }
        
        // Animate stats texts with stagger
        float delay = panelScaleDuration * 0.7f;
        
        // Display and animate final score
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {finalScore}";
            AnimateText(finalScoreText, delay);
            delay += textStaggerDelay;
        }
        
        // Display and animate final time
        if (finalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(finalTime / 60f);
            int seconds = Mathf.FloorToInt(finalTime % 60f);
            finalTimeText.text = $"Time: {minutes:00}:{seconds:00}";
            AnimateText(finalTimeText, delay);
            delay += textStaggerDelay;
        }
        
        // Display and animate final moves
        if (finalMovesText != null)
        {
            finalMovesText.text = $"Moves: {finalMoves}";
            AnimateText(finalMovesText, delay);
            delay += textStaggerDelay;
        }
        
        // Display and animate best combo
        if (finalComboText != null)
        {
            if (finalCombo > 1)
            {
                finalComboText.text = $"Best Combo: x{finalCombo}";
                AnimateText(finalComboText, delay);
                delay += textStaggerDelay;
            }
            else
            {
                finalComboText.text = "";
            }
        }
        
        // Animate buttons
        AnimateButton(playAgainButton, delay + buttonAnimDelay);
        AnimateButton(quitButton, delay + buttonAnimDelay + 0.1f);
    }
    
    private void AnimateText(TextMeshProUGUI text, float delay)
    {
        if (text == null) return;
        
        text.alpha = 0;
        text.transform.localPosition += Vector3.down * 20f;
        
        Sequence textSequence = DOTween.Sequence();
        textSequence.Append(text.DOFade(1f, textAnimDuration));
        textSequence.Join(text.transform.DOLocalMoveY(text.transform.localPosition.y + 20f, textAnimDuration)
            .SetEase(Ease.OutQuad));
        textSequence.SetDelay(delay);
    }
    
    private void AnimateButton(Button button, float delay)
    {
        if (button == null) return;
        
        button.transform.localScale = Vector3.zero;
        button.transform.DOScale(1f, textAnimDuration)
            .SetEase(Ease.OutBack)
            .SetDelay(delay);
    }
    
    public void HideGameOver()
    {
        if (gameOverPanel == null)
            return;
        
        // Animate panel out
        Sequence hideSequence = DOTween.Sequence();
        
        if (panelCanvasGroup != null)
        {
            hideSequence.Append(panelCanvasGroup.DOFade(0f, panelFadeDuration));
        }
        
        if (panelRect != null)
        {
            hideSequence.Join(panelRect.DOScale(0.8f, panelFadeDuration).SetEase(Ease.InBack));
        }
        
        hideSequence.OnComplete(() => gameOverPanel.SetActive(false));
    }
    
    private void OnPlayAgainClicked()
    {
        HideGameOver();
        
        if (gameManager != null)
        {
            gameManager.NewGame();
        }
    }
    
    private void OnQuitClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void OnDestroy()
    {
        if (playAgainButton != null)
        {
            playAgainButton.onClick.RemoveListener(OnPlayAgainClicked);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }
        
        // Kill all tweens on this object
        DOTween.Kill(this);
        if (gameOverPanel != null)
        {
            gameOverPanel.transform.DOKill();
        }
        if (panelRect != null)
        {
            panelRect.DOKill();
        }
        if (titleText != null)
        {
            titleText.transform.DOKill();
        }
    }
}
