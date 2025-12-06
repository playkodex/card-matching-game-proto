using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private GameObject comboContainer;
    
    [Header("Game UI")]
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timerText;
    
    private ScoreManager scoreManager;
    private int moveCount = 0;
    private float gameTime = 0;
    private bool isGameActive = false;
    
    private void Start()
    {
        scoreManager = ScoreManager.Instance;
        
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged += UpdateScoreDisplay;
            scoreManager.OnComboChanged += UpdateComboDisplay;
        }
        
        UpdateScoreDisplay(0);
        UpdateComboDisplay(0);
        UpdateMovesDisplay();
        
        isGameActive = true;
    }
    
    private void Update()
    {
        if (isGameActive && timerText != null)
        {
            gameTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }
    
    private void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    private void UpdateComboDisplay(int combo)
    {
        if (comboText != null)
        {
            if (combo > 1)
            {
                comboText.text = $"Combo x{combo}!";
                if (comboContainer != null)
                {
                    comboContainer.SetActive(true);
                }
            }
            else
            {
                if (comboContainer != null)
                {
                    comboContainer.SetActive(false);
                }
            }
        }
    }
    
    public void IncrementMoves()
    {
        moveCount++;
        UpdateMovesDisplay();
    }
    
    private void UpdateMovesDisplay()
    {
        if (movesText != null)
        {
            movesText.text = $"Moves: {moveCount}";
        }
    }
    
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
    
    public void ResetUI()
    {
        moveCount = 0;
        gameTime = 0;
        isGameActive = true; // Restart the timer
        UpdateScoreDisplay(0);
        UpdateComboDisplay(0);
        UpdateMovesDisplay();
        UpdateTimerDisplay();
    }
    
    public void StopGame()
    {
        isGameActive = false;
    }
    
    public void StartGame()
    {
        isGameActive = true;
        gameTime = 0;
    }
    
    public void ResumeGame()
    {
        isGameActive = true;
        // Don't reset gameTime - keep the loaded value
    }
    
    public float GetGameTime()
    {
        return gameTime;
    }
    
    public int GetMoveCount()
    {
        return moveCount;
    }
    
    public void SetGameTime(float time)
    {
        gameTime = time;
        UpdateTimerDisplay();
    }
    
    public void SetMoveCount(int moves)
    {
        moveCount = moves;
        UpdateMovesDisplay();
    }
    
    private void OnDestroy()
    {
        if (scoreManager != null)
        {
            scoreManager.OnScoreChanged -= UpdateScoreDisplay;
            scoreManager.OnComboChanged -= UpdateComboDisplay;
        }
    }
}
