using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleMusicToggle : MonoBehaviour
{
    public TMP_Text statusText; // Text to show "Music Muted" message
    private bool isMuted;

    void Start()
    {
        // Load saved mute state
        isMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        UpdateStatusText();

        // Add button click listener
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ToggleMusic);
        }
    }

    public void ToggleMusic()
    {
        // Toggle mute state
        isMuted = !isMuted;

        // Save preference
        PlayerPrefs.SetInt("MusicMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        // Update all music sources
        UpdateAudioSources();

        // Update UI text
        UpdateStatusText();
    }

    void UpdateAudioSources()
    {
        // Find the MainMenuMusic instance and use it
        MainMenuMusic musicManager = MainMenuMusic.Instance;
        if (musicManager != null)
        {
            // This calls the existing toggle method in your MainMenuMusic class
            musicManager.ToggleMute();
        }

        // Find the GameplayMusic instance and use it (if it exists)
        GameplayMusic gameplayMusic = GameplayMusic.Instance;
        if (gameplayMusic != null)
        {
            gameplayMusic.ToggleMute();
        }

        // Update any music sources tagged as "MusicSource"
        AudioSource[] audioSources = GameObject.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in audioSources)
        {
            if (source != null && source.gameObject.CompareTag("MusicSource"))
            {
                source.mute = isMuted;
            }
        }
    }

    void UpdateStatusText()
    {
        if (statusText != null)
        {
            statusText.text = isMuted ? "Music Muted" : "";
        }
    }
}