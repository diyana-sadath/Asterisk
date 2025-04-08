using UnityEngine;
using UnityEngine.UI;

public class AudioSliderController : MonoBehaviour
{
    // Slider References
    public Slider musicSlider;
    public Slider soundSlider;

    // Saved preference keys
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SOUND_VOLUME_KEY = "SoundVolume";

    void Start()
    {
        // Initialize music slider
        if (musicSlider != null)
        {
            // Load saved volume or default to 1.0f
            float savedMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1.0f);
            musicSlider.value = savedMusicVolume;

            // Add listener for when slider value changes
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

            // Apply initial volume
            ApplyMusicVolume(savedMusicVolume);
        }

        // Initialize sound slider
        if (soundSlider != null)
        {
            // Load saved volume or default to 1.0f
            float savedSoundVolume = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, 1.0f);
            soundSlider.value = savedSoundVolume;

            // Add listener for when slider value changes
            soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);

            // Apply initial volume
            ApplySoundVolume(savedSoundVolume);
        }
    }

    void OnMusicSliderChanged(float value)
    {
        // Save the new value
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        PlayerPrefs.Save();

        // Apply volume change
        ApplyMusicVolume(value);
    }

    void OnSoundSliderChanged(float value)
    {
        // Save the new value
        PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, value);
        PlayerPrefs.Save();

        // Apply volume change
        ApplySoundVolume(value);
    }

    void ApplyMusicVolume(float volume)
    {
        // Apply to MainMenuMusic
        MainMenuMusic menuMusic = MainMenuMusic.Instance;
        if (menuMusic != null && menuMusic.GetComponent<AudioSource>() != null)
        {
            menuMusic.GetComponent<AudioSource>().volume = volume;
        }

        // Apply to GameplayMusic
        GameplayMusic gameplayMusic = GameplayMusic.Instance;
        if (gameplayMusic != null && gameplayMusic.GetComponent<AudioSource>() != null)
        {
            gameplayMusic.GetComponent<AudioSource>().volume = volume;
        }

        // Apply to all music sources (as backup)
        AudioSource[] audioSources = GameObject.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in audioSources)
        {
            if (source != null && source.gameObject.CompareTag("MusicSource"))
            {
                source.volume = volume;
            }
        }
    }

    void ApplySoundVolume(float volume)
    {
        // Apply to SoundManager
        SoundManager soundManager = SoundManager.Instance;
        if (soundManager != null && soundManager.sfxSource != null)
        {
            soundManager.sfxSource.volume = volume;
        }
    }
}