using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CalibrationRequirement : MonoBehaviour
{
    public static CalibrationRequirement Instance { get; private set; }

    [Header("Button Reference")]
    public Button startButton; // Reference to the Start button on HomeScreen

    // Event that other components can subscribe to
    public event Action<bool> OnCalibrationStateChanged;

    // This flag stores whether calibration data exists - but doesn't mean it's valid this session
    private bool calibrationDataExists = false;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Check if calibration data exists in persistent storage
        calibrationDataExists = PlayerPrefs.GetInt("IsCalibrated", 0) == 1;

        // Subscribe to scene loaded events to update buttons in new scenes
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Subscribe to session calibration changes
        if (SessionCalibrationHandler.Instance != null)
        {
            SessionCalibrationHandler.Instance.OnSessionCalibrationStateChanged += OnSessionCalibrationChanged;
        }
    }

    void Start()
    {
        // Initial update of buttons based on session calibration status
        UpdateStartButtonState();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the start button in newly loaded scenes if needed
        if (startButton == null)
        {
            startButton = GameObject.Find("StartButton")?.GetComponent<Button>();
        }

        // Update button state after a short delay to ensure all components are initialized
        Invoke("UpdateStartButtonState", 0.2f);
    }

    // This is modified to use the session calibration state
    public bool IsCalibrated()
    {
        if (SessionCalibrationHandler.Instance != null)
        {
            return SessionCalibrationHandler.Instance.IsSessionCalibrated();
        }
        else
        {
            // Fallback to old behavior if session handler isn't available
            return calibrationDataExists;
        }
    }

    // This now sets both the persistent calibration and the session calibration
    public void SetCalibrated(bool isCalibrated)
    {
        // Set the persistent data
        calibrationDataExists = isCalibrated;
        PlayerPrefs.SetInt("IsCalibrated", isCalibrated ? 1 : 0);
        PlayerPrefs.Save();

        // Set the session state (this is new)
        if (SessionCalibrationHandler.Instance != null)
        {
            SessionCalibrationHandler.Instance.SetSessionCalibrated(isCalibrated);
        }

        // Notify listeners
        OnCalibrationStateChanged?.Invoke(isCalibrated);

        // Update UI
        UpdateStartButtonState();

        Debug.Log($"Calibration requirement status changed: {(isCalibrated ? "Calibrated" : "Not Calibrated")}");
    }

    // New method to handle session calibration updates
    private void OnSessionCalibrationChanged(bool isSessionCalibrated)
    {
        UpdateStartButtonState();
    }

    // In CalibrationRequirement.cs - Update UpdateStartButtonState method

    private void UpdateStartButtonState()
    {
        // Find start button if it's not assigned
        if (startButton == null)
        {
            startButton = GameObject.Find("StartButton")?.GetComponent<Button>();

            // Try alternative naming schemes if not found
            if (startButton == null)
            {
                startButton = GameObject.Find("Start")?.GetComponent<Button>();
            }
            if (startButton == null)
            {
                startButton = GameObject.Find("btnStart")?.GetComponent<Button>();
            }
        }

        // Update start button interactability based on BOTH login AND session calibration status
        if (startButton != null)
        {
            // Only change interactability if the button is currently active
            if (startButton.gameObject.activeInHierarchy)
            {
                bool isCurrentlyCalibrated = IsCalibrated();

                // Check login state too - find auth validator if needed
                ButtonAuthValidator authValidator = ButtonAuthValidator.Instance;
                bool isLoggedIn = true; // Default to true if we can't find auth validator

                if (authValidator != null)
                {
                    // Get a reference to PlayFabAuthManager
                    var authManagerField = typeof(ButtonAuthValidator).GetField("authManager",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (authManagerField != null)
                    {
                        var authManager = authManagerField.GetValue(authValidator);
                        if (authManager != null)
                        {
                            // Call IsUserLoggedIn method via reflection
                            var isLoggedInMethod = authManager.GetType().GetMethod("IsUserLoggedIn");
                            if (isLoggedInMethod != null)
                            {
                                isLoggedIn = (bool)isLoggedInMethod.Invoke(authManager, null);
                            }
                        }
                    }
                }

                // Button is only enabled if BOTH conditions are met
                startButton.interactable = isCurrentlyCalibrated && isLoggedIn;

                Debug.Log($"Updated start button interactable: {startButton.interactable} (Calibrated: {isCurrentlyCalibrated}, LoggedIn: {isLoggedIn})");
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up event subscriptions
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (SessionCalibrationHandler.Instance != null)
        {
            SessionCalibrationHandler.Instance.OnSessionCalibrationStateChanged -= OnSessionCalibrationChanged;
        }
    }
}