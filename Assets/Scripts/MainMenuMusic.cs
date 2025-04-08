using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMusic : MonoBehaviour
{
    private static MainMenuMusic instance;
    private AudioSource audioSource;
    public static MainMenuMusic Instance { get { return instance; } }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();

            // Add a tag to identify this as a music source
            gameObject.tag = "MusicSource";
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Apply saved mute state
        bool isMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        if (audioSource != null)
        {
            audioSource.mute = isMuted;
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioSource == null) return;

        if (scene.name == "GameplayScreen") // Replace with your actual gameplay scene name
        {
            audioSource.Pause(); // Stop music in gameplay
        }
        else
        {
            // Apply saved mute state before playing
            bool isMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
            audioSource.mute = isMuted;

            if (!audioSource.isPlaying)
                audioSource.Play(); // Resume music in other scenes
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to prevent errors
    }

    // Public method to access audio source safely
    public bool ToggleMute()
    {
        if (audioSource != null)
        {
            audioSource.mute = !audioSource.mute;
            return audioSource.mute;
        }
        return false;
    }
}