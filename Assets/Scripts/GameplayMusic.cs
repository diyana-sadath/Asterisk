using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayMusic : MonoBehaviour
{
    private static GameplayMusic instance;
    private AudioSource audioSource;
    public static GameplayMusic Instance { get { return instance; } }

    // References to listen for game state changes
    private GameOverManager gameOverManager;
    private GameWonManager gameWonManager;
    private GameController gameController;

    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
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

        // Initially stop the gameplay music
        if (audioSource != null)
        {
            bool isMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
            audioSource.mute = isMuted;
            audioSource.Stop();
        }
    }

    // Add this method to GameplayMusic.cs
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
    void Update()
    {
        // We'll rely on panel activation instead of checking isGameOver
        // since it's not accessible
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioSource == null) return;

        // Reset state variables
        isGameOver = false;

        // Clear references when scene changes
        gameOverManager = null;
        gameWonManager = null;
        gameController = null;

        // Only play in gameplay scene, stop in all other scenes
        if (scene.name == "GameplayScreen")
        {
            Debug.Log("GameplayMusic: Starting gameplay music");

            // Apply saved mute state before playing
            bool isMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
            audioSource.mute = isMuted;

            // Start the music
            audioSource.Play();

            // Find the managers in the new scene
            gameOverManager = FindFirstObjectByType<GameOverManager>();
            gameWonManager = FindFirstObjectByType<GameWonManager>();


            // Register event listeners for panels
            RegisterEvents();
        }
        else
        {
            Debug.Log("GameplayMusic: Stopping gameplay music for scene: " + scene.name);
            StopMusic();
        }
    }

    void RegisterEvents()
    {
        // Monitor the GameOver panel
        if (gameOverManager != null && gameOverManager.gameOverPanel != null)
        {
            MonitorPanel(gameOverManager.gameOverPanel);
        }

        // Monitor the GameWon panel
        if (gameWonManager != null && gameWonManager.gameWonPanel != null)
        {
            MonitorPanel(gameWonManager.gameWonPanel);
        }
    }

    void MonitorPanel(GameObject panel)
    {
        // Add our monitor component if it doesn't already exist
        if (!panel.GetComponent<PanelStateMonitor>())
        {
            var monitor = panel.AddComponent<PanelStateMonitor>();
            monitor.OnPanelShown += StopMusic;
        }
    }

    void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            Debug.Log("GameplayMusic: Stopping due to game end state");
            audioSource.Stop();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

// Helper class to monitor panel activation
public class PanelStateMonitor : MonoBehaviour
{
    public delegate void PanelEvent();
    public event PanelEvent OnPanelShown;

    private bool wasActive = false;

    void Update()
    {
        // Check if the panel just became active
        if (!wasActive && gameObject.activeSelf)
        {
            OnPanelShown?.Invoke();
        }

        wasActive = gameObject.activeSelf;
    }
}