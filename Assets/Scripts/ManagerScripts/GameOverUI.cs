using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI finalMovesText;
    [SerializeField] private TextMeshProUGUI finalComboText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button quitButton;
    
    [Header("Settings")]
    [SerializeField] private string victoryMessage = "Victory!";
    
    private CardGameManager gameManager;
    
    private void Awake()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
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
            
        // Show panel
        gameOverPanel.SetActive(true);
        
        // Set title
        if (titleText != null)
        {
            titleText.text = victoryMessage;
        }
        
        // Display final score
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {finalScore}";
        }
        
        // Display final time
        if (finalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(finalTime / 60f);
            int seconds = Mathf.FloorToInt(finalTime % 60f);
            finalTimeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
        
        // Display final moves
        if (finalMovesText != null)
        {
            finalMovesText.text = $"Moves: {finalMoves}";
        }
        
        // Display best combo
        if (finalComboText != null)
        {
            if (finalCombo > 1)
            {
                finalComboText.text = $"Best Combo: x{finalCombo}";
            }
            else
            {
                finalComboText.text = "";
            }
        }
    }
    
    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
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
    }
}
