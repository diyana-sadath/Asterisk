using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicToggle : MonoBehaviour
{
    public AudioSource mainMenuMusic; // Assign the AudioSource from MainMenuMusic GameObject
    public TMP_Text statusText; // Assign the UI Text that displays "Muted/Unmuted"
    public Image buttonImage; // Assign the button image component
    public Sprite musicOnSprite; // Sprite to show when music is on
    public Sprite musicOffSprite; // Sprite to show when music is off
    private bool isMuted = false;

    void Start()
    {
        // Load saved preference
        isMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        UpdateMusicState();

        // Add listener to button if not already added
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ToggleMusic);
        }
    }

    public void ToggleMusic()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("MusicMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        UpdateMusicState();
    }

    void UpdateMusicState()
    {
        // Update the directly assigned audio source
        if (mainMenuMusic != null)
        {
            mainMenuMusic.mute = isMuted;
        }

        // Update all audio sources with the "MusicSource" tag
        AudioSource[] audioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in audioSources)
        {
            if (source != null && source.gameObject.CompareTag("MusicSource"))
            {
                source.mute = isMuted;
            }
        }

        // Update text display
        if (statusText != null)
        {
            statusText.text = isMuted ? "Music Muted" : "";
        }

        // Update button visual
        if (buttonImage != null && musicOnSprite != null && musicOffSprite != null)
        {
            buttonImage.sprite = isMuted ? musicOffSprite : musicOnSprite;
        }
    }
}