using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

// This class will persist between scenes and manage the difficulty unlocking
public class DifficultyUnlockManager : MonoBehaviour
{
    public static DifficultyUnlockManager Instance { get; private set; }
    
    // Events to notify other components about unlock changes
    public delegate void DifficultyUnlockChanged();
    public event DifficultyUnlockChanged OnDifficultyStatusChanged;

    // Reference to the Coming Soon panel game object
    [SerializeField] private GameObject comingSoonPanel;
    
    // Reference to the Map button
    [SerializeField] private UnityEngine.UI.Button mapButton;
    
    // Reference to the Back button on the Coming Soon panel
    [SerializeField] private UnityEngine.UI.Button backButton;

    private bool easyCompleted = false;
    private bool mediumCompleted = false;
    private string currentPlayFabId = "";
    private bool isInitialized = false;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("DifficultyUnlockManager: Instance created and set to DontDestroyOnLoad");
        }
        else if (Instance != this)
        {
            Debug.Log("DifficultyUnlockManager: Destroying duplicate instance");
            Destroy(gameObject);
            return;
        }

        // Load initial state from PlayerPrefs
        easyCompleted = PlayerPrefs.GetInt("EasyCompleted", 0) == 1;
        mediumCompleted = PlayerPrefs.GetInt("MediumCompleted", 0) == 1;
        Debug.Log($"Initial state from PlayerPrefs: Easy: {easyCompleted}, Medium: {mediumCompleted}");
    }

    private void Start()
    {
        // Ensure the Coming Soon panel is hidden at start
        if (comingSoonPanel != null)
        {
            comingSoonPanel.SetActive(false);
        }
        
        // Add click listener to the Map button
        if (mapButton != null)
        {
            mapButton.onClick.AddListener(OnMapButtonClicked);
        }
        
        // Add click listener to the Back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        // Find and subscribe to the auth manager's events
        FindAndSubscribeToAuthManager();
        
        // Initialize difficulty status
        InitializeDifficultyStatus();
    }

    private void FindAndSubscribeToAuthManager()
    {
        PlayFabAuthManager authManager = FindObjectOfType<PlayFabAuthManager>();
        if (authManager != null)
        {
            authManager.OnUserLoggedIn += LoadUserCompletionStatus;
            authManager.OnUserLoggedOut += ResetCompletionStatus;
            
            // If user is already logged in when this component starts
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.Log("User already logged in, loading PlayFab difficulty data");
                LoadUserCompletionStatus();
            }
            
            Debug.Log("Successfully subscribed to PlayFabAuthManager events");
        }
        else
        {
            Debug.LogWarning("PlayFabAuthManager not found. User-specific difficulty progress won't work.");
        }
    }

    public void InitializeDifficultyStatus()
    {
        if (isInitialized) return;
        
        // If logged in to PlayFab, load from there, otherwise use PlayerPrefs data
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            LoadUserCompletionStatus();
        }
        else
        {
            // Already loaded from PlayerPrefs in Awake
            NotifyUnlockStatusChanged();
        }
        
        isInitialized = true;
    }

    // This method is called when the Map button is clicked
    public void OnMapButtonClicked()
    {
        // Show the Coming Soon panel
        if (comingSoonPanel != null)
        {
            comingSoonPanel.SetActive(true);
        }
        
        Debug.Log("Map button clicked, showing Coming Soon panel");
    }

    // This method is called when the Back button is clicked
    public void OnBackButtonClicked()
    {
        // Hide the Coming Soon panel
        if (comingSoonPanel != null)
        {
            comingSoonPanel.SetActive(false);
        }
        
        Debug.Log("Back button clicked, returning to home screen");
    }

    // Load user-specific completion status from PlayFab
    public void LoadUserCompletionStatus()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.Log("User not logged in. Cannot load difficulty data from PlayFab.");
            return;
        }

        // Get user data from PlayFab
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result => {
                Debug.Log("Fetching user difficulty data from PlayFab");

                // Get the PlayFab ID
                PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
                    accountInfo => {
                        currentPlayFabId = accountInfo.AccountInfo.PlayFabId;
                        Debug.Log("PlayFab ID: " + currentPlayFabId);
                    },
                    error => {
                        Debug.LogError("Error getting account info: " + error.ErrorMessage);
                    }
                );

                bool previousEasyStatus = easyCompleted;
                bool previousMediumStatus = mediumCompleted;

                // For a new user, we want to start with everything locked except Easy
                easyCompleted = false;
                mediumCompleted = false;

                // Only override defaults if data exists
                if (result.Data != null && result.Data.Count > 0)
                {
                    if (result.Data.TryGetValue("EasyCompleted", out var easyValue))
                    {
                        easyCompleted = easyValue.Value == "1";
                    }

                    if (result.Data.TryGetValue("MediumCompleted", out var mediumValue))
                    {
                        mediumCompleted = mediumValue.Value == "1";
                    }
                }

                Debug.Log($"Loaded difficulty status from PlayFab: Easy completed: {easyCompleted}, Medium completed: {mediumCompleted}");

                // Also update PlayerPrefs to keep them in sync
                PlayerPrefs.SetInt("EasyCompleted", easyCompleted ? 1 : 0);
                PlayerPrefs.SetInt("MediumCompleted", mediumCompleted ? 1 : 0);
                PlayerPrefs.Save();

                // Notify listeners if status changed
                if (previousEasyStatus != easyCompleted || previousMediumStatus != mediumCompleted)
                {
                    NotifyUnlockStatusChanged();
                }
            },
            error => {
                Debug.LogError("Error loading user data: " + error.ErrorMessage);

                // On error, we still need to ensure proper initial state and notify listeners
                easyCompleted = PlayerPrefs.GetInt("EasyCompleted", 0) == 1;
                mediumCompleted = PlayerPrefs.GetInt("MediumCompleted", 0) == 1;
                NotifyUnlockStatusChanged();
            }
        );
    }

    // Reset completion status when user logs out
    public void ResetCompletionStatus()
    {
        bool statusChanged = easyCompleted || mediumCompleted;
        
        easyCompleted = false;
        mediumCompleted = false;
        currentPlayFabId = "";
        
        // Update PlayerPrefs
        PlayerPrefs.SetInt("EasyCompleted", 0);
        PlayerPrefs.SetInt("MediumCompleted", 0);
        PlayerPrefs.Save();
        
        Debug.Log("Reset difficulty completion status due to logout");
        
        if (statusChanged)
        {
            NotifyUnlockStatusChanged();
        }
    }

    // Call this when Easy mode is completed
    public void CompleteEasyMode()
    {
        Debug.Log("DifficultyUnlockManager: Completing Easy mode!");
        if (easyCompleted) return; // Already completed
        
        easyCompleted = true;
        
        // Save to PlayerPrefs for offline use
        PlayerPrefs.SetInt("EasyCompleted", 1);
        PlayerPrefs.Save();
        
        // If user is logged in, also save to PlayFab
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            SaveUserCompletionStatus();
        }
        
        NotifyUnlockStatusChanged();
    }

    // Call this when Medium mode is completed
    public void CompleteMediumMode()
    {
        Debug.Log("DifficultyUnlockManager: Completing Medium mode!");
        if (mediumCompleted) return; // Already completed
        
        mediumCompleted = true;
        
        // Save to PlayerPrefs for offline use
        PlayerPrefs.SetInt("MediumCompleted", 1);
        PlayerPrefs.Save();
        
        // If user is logged in, also save to PlayFab
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            SaveUserCompletionStatus();
        }
        
        NotifyUnlockStatusChanged();
    }
    
    // Save completion status to PlayFab
    private void SaveUserCompletionStatus()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Cannot save difficulty data to PlayFab. User not logged in.");
            return;
        }

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "EasyCompleted", easyCompleted ? "1" : "0" },
                { "MediumCompleted", mediumCompleted ? "1" : "0" }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Successfully saved user difficulty data to PlayFab"),
            error => Debug.LogError("Error saving user data to PlayFab: " + error.ErrorMessage)
        );
    }

    // Check if a difficulty level is unlocked
    public bool IsDifficultyUnlocked(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 0: // Easy
                return true; // Always unlocked
            case 1: // Medium
                return easyCompleted;
            case 2: // Hard
                return mediumCompleted;
            default:
                return false;
        }
    }
    
    // Notify all listeners that unlock status has changed
    private void NotifyUnlockStatusChanged()
    {
        Debug.Log($"Notifying listeners of difficulty status change: Easy: {easyCompleted}, Medium: {mediumCompleted}");
        OnDifficultyStatusChanged?.Invoke();
    }
    
    // For testing and debugging
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey("EasyCompleted");
        PlayerPrefs.DeleteKey("MediumCompleted");
        PlayerPrefs.Save();
        
        easyCompleted = false;
        mediumCompleted = false;
        
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            SaveUserCompletionStatus();
        }
        
        NotifyUnlockStatusChanged();
        Debug.Log("All difficulty progress reset");
    }
}