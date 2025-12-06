using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip cardFlipSound;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip mismatchSound;
    [SerializeField] private AudioClip gameOverSound;
    
    [Header("Audio Settings")]
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private bool enableSFX = true;
    
    private AudioSource audioSource;
    private static AudioManager instance;
    
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
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
            return;
        }
        
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure AudioSource
        audioSource.playOnAwake = false;
        audioSource.volume = sfxVolume;
    }
    
    public void PlayCardFlip()
    {
        PlaySound(cardFlipSound);
    }
    
    public void PlayMatch()
    {
        PlaySound(matchSound);
    }
    
    public void PlayMismatch()
    {
        PlaySound(mismatchSound);
    }
    
    public void PlayGameOver()
    {
        PlaySound(gameOverSound);
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (!enableSFX || clip == null || audioSource == null)
            return;
            
        audioSource.PlayOneShot(clip, sfxVolume);
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = sfxVolume;
        }
    }
    
    public void ToggleSFX(bool enabled)
    {
        enableSFX = enabled;
    }
    
    public bool IsSFXEnabled()
    {
        return enableSFX;
    }
}
