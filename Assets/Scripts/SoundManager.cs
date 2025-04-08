using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource; // For one-shot sound effects

    [Header("UI Sounds")]
    public AudioClip buttonClickSound;

    [Header("Gameplay Sounds")]
    public AudioClip starCollectSound;
    public AudioClip asteroidHitSound;
    public AudioClip lifeLostSound;

    [Header("Game State Sounds")]
    public AudioClip gameOverSound;
    public AudioClip gameWinSound;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSoundVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }
    public void PlayButtonClick()
    {
        if (buttonClickSound != null && sfxSource != null)
            sfxSource.PlayOneShot(buttonClickSound);
    }

    public void PlayStarCollect()
    {
        if (starCollectSound != null && sfxSource != null)
            sfxSource.PlayOneShot(starCollectSound);
    }

    public void PlayAsteroidHit()
    {
        if (asteroidHitSound != null && sfxSource != null)
            sfxSource.PlayOneShot(asteroidHitSound);
    }

    public void PlayLifeLost()
    {
        if (lifeLostSound != null && sfxSource != null)
            sfxSource.PlayOneShot(lifeLostSound);
    }

    public void PlayGameOver()
    {
        if (gameOverSound != null && sfxSource != null)
            sfxSource.PlayOneShot(gameOverSound);
    }

    public void PlayGameWin()
    {
        if (gameWinSound != null && sfxSource != null)
            sfxSource.PlayOneShot(gameWinSound);
    }

    // Utility method to play any sound
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }
}