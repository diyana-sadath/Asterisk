using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

// This class will persist between scenes and manage the difficulty unlocking
public class DifficultyUnlockManager : MonoBehaviour
{
    public static DifficultyUnlockManager Instance { get; private set; }

    // Reference to the Coming Soon panel game object
    [SerializeField] private GameObject comingSoonPanel;
    
    // Reference to the Map button
    [SerializeField] private UnityEngine.UI.Button mapButton;
    
    // Reference to the Back button on the Coming Soon panel
    [SerializeField] private UnityEngine.UI.Button backButton;

    private bool easyCompleted = false;
    private bool mediumCompleted = false;
    private string currentPlayFabId = "";

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
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

        // Find the authentication manager and subscribe to events
        PlayFabAuthManager authManager = FindObjectOfType<PlayFabAuthManager>();
        if (authManager != null)
        {
            // Add event listeners if PlayFabAuthManager has been updated
            if (HasEvents(authManager))
            {
                authManager.OnUserLoggedIn += LoadUserCompletionStatus;
                authManager.OnUserLoggedOut += ResetCompletionStatus;
            }
            else
            {
                Debug.LogWarning("PlayFabAuthManager doesn't have login/logout events. User-specific difficulty progress won't work.");
            }
        }
    }

    // Check if the auth manager has the required events
    private bool HasEvents(PlayFabAuthManager authManager)
    {
        // Using reflection to check if events exist
        var eventInfo = authManager.GetType().GetEvent("OnUserLoggedIn");
        return eventInfo != null;
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

    // Load completion status from PlayerPrefs or PlayFab
    private void LoadCompletionStatus()
    {
        // If logged in to PlayFab, load from there
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            LoadUserCompletionStatus();
        }
        else
        {
            // Otherwise, load from PlayerPrefs
            easyCompleted = PlayerPrefs.GetInt("EasyCompleted", 0) == 1;
            mediumCompleted = PlayerPrefs.GetInt("MediumCompleted", 0) == 1;
            
            Debug.Log($"Loaded difficulty status from PlayerPrefs: Easy completed: {easyCompleted}, Medium completed: {mediumCompleted}");
        }
    }

    // Load user-specific completion status from PlayFab
    private void LoadUserCompletionStatus()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.Log("User not logged in. Cannot load difficulty data from PlayFab.");
            LoadCompletionStatus(); // Fall back to PlayerPrefs
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
                    },
                    error => {
                        Debug.LogError("Error getting account info: " + error.ErrorMessage);
                    }
                );
                
                // Default to false if the key doesn't exist
                if (result.Data != null)
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
            },
            error => {
                Debug.LogError("Error loading user data: " + error.ErrorMessage);
                LoadCompletionStatus(); // Fall back to PlayerPrefs
            }
        );
    }

    // Reset completion status when user logs out
    private void ResetCompletionStatus()
    {
        easyCompleted = false;
        mediumCompleted = false;
        currentPlayFabId = "";
        Debug.Log("Reset difficulty completion status due to logout");
        
        // Reload from PlayerPrefs for non-logged-in state
        LoadCompletionStatus();
    }

    // Call this when Easy mode is completed
    public void CompleteEasyMode()
    {
        Debug.Log("DifficultyUnlockManager: Completing Easy mode!");
        easyCompleted = true;
        
        // Save to PlayerPrefs for offline use
        PlayerPrefs.SetInt("EasyCompleted", 1);
        PlayerPrefs.Save();
        
        // If user is logged in, also save to PlayFab
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            SaveUserCompletionStatus();
        }
    }

    // Call this when Medium mode is completed
    public void CompleteMediumMode()
    {
        Debug.Log("DifficultyUnlockManager: Completing Medium mode!");
        mediumCompleted = true;
        
        // Save to PlayerPrefs for offline use
        PlayerPrefs.SetInt("MediumCompleted", 1);
        PlayerPrefs.Save();
        
        // If user is logged in, also save to PlayFab
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            SaveUserCompletionStatus();
        }
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
}