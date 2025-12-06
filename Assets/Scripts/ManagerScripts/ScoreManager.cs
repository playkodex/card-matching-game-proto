using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Scoring Settings")]
    [SerializeField] private int matchPoints = 100;
    [SerializeField] private int mismatchPenalty = 10;
    [SerializeField] private int comboMultiplier = 50;
    [SerializeField] private float comboTimeWindow = 3f;
    
    private int currentScore = 0;
    private int currentCombo = 0;
    private float lastMatchTime = 0;
    
    public event Action<int> OnScoreChanged;
    public event Action<int> OnComboChanged;
    
    private static ScoreManager instance;
    
    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScoreManager>();
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public void AddMatchScore()
    {
        // Check if within combo time window
        if (Time.time - lastMatchTime <= comboTimeWindow)
        {
            currentCombo++;
        }
        else
        {
            currentCombo = 1;
        }
        
        lastMatchTime = Time.time;
        
        // Calculate score with combo bonus
        int scoreToAdd = matchPoints + (currentCombo - 1) * comboMultiplier;
        currentScore += scoreToAdd;
        
        OnScoreChanged?.Invoke(currentScore);
        OnComboChanged?.Invoke(currentCombo);
        
        Debug.Log($"Match! +{scoreToAdd} points (Combo x{currentCombo})");
    }
    
    public void AddMismatchPenalty()
    {
        // Reset combo on mismatch
        if (currentCombo > 0)
        {
            currentCombo = 0;
            OnComboChanged?.Invoke(currentCombo);
        }
        
        // Apply penalty (don't go below 0)
        currentScore = Mathf.Max(0, currentScore - mismatchPenalty);
        OnScoreChanged?.Invoke(currentScore);
        
        Debug.Log($"Mismatch! -{mismatchPenalty} points");
    }
    
    public int GetCurrentScore()
    {
        return currentScore;
    }
    
    public int GetCurrentCombo()
    {
        return currentCombo;
    }
    
    public void ResetScore()
    {
        currentScore = 0;
        currentCombo = 0;
        lastMatchTime = 0;
        
        OnScoreChanged?.Invoke(currentScore);
        OnComboChanged?.Invoke(currentCombo);
    }
    
    // For save/load system
    public ScoreData GetScoreData()
    {
        return new ScoreData
        {
            score = currentScore,
            combo = currentCombo,
            lastMatchTime = lastMatchTime
        };
    }
    
    public void LoadScoreData(ScoreData data)
    {
        currentScore = data.score;
        currentCombo = data.combo;
        lastMatchTime = data.lastMatchTime;
        
        OnScoreChanged?.Invoke(currentScore);
        OnComboChanged?.Invoke(currentCombo);
    }
}

[System.Serializable]
public class ScoreData
{
    public int score;
    public int combo;
    public float lastMatchTime;
}
