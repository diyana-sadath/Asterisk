using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class PlayFabAuthManager : MonoBehaviour
{
    // Add these events for difficulty manager integration
    public event Action OnUserLoggedIn;
    public event Action OnUserLoggedOut;

    [Header("UI Panels")]
    public GameObject homePanel;
    public GameObject signUpPanel;
    public GameObject signInPanel;
    

    [Header("Sign-Up Fields")]
    public TMP_InputField signUpEmailInputField;
    public TMP_InputField signUpUsernameInputField;
    public TMP_InputField signUpPasswordInputField;
    public Text signUpStatusText; // Legacy Text component

    [Header("Sign-In Fields")]
    public TMP_InputField signInEmailInputField;
    public TMP_InputField signInPasswordInputField;
    public Text signInStatusText; // Legacy Text component

    [Header("Home Panel Buttons")]
    public Button homeSignInButton;
    public Button homeSignUpButton;
    public Button homeBackButton;

    [Header("Sign-Up Panel Buttons")]
    public Button signUpRegisterButton;
    public Button signUpBackButton;

    [Header("Sign-In Panel Buttons")]
    public Button signInLoginButton;
    public Button signInBackButton;

    [Header("Sign Out")]
    public Button signOutButton;

    [Header("Home Panel UI")]
    public Text homeStatusText;

    private DifficultyUnlockManager difficultyManager;
    private bool initialPanelSetupDone = false;

    private void Start()
    {
        // Initially hide all panels until we determine what to show
        if (!initialPanelSetupDone)
        {
            homePanel.SetActive(false);
            signUpPanel.SetActive(false);
            signInPanel.SetActive(false);
            signOutButton.gameObject.SetActive(false);
            initialPanelSetupDone = true;
        }

        homeSignInButton.onClick.AddListener(ShowSignInPanel);
        homeSignUpButton.onClick.AddListener(ShowSignUpPanel);
        homeBackButton.onClick.AddListener(HideAllPanels);

        signUpRegisterButton.onClick.AddListener(OnRegisterButtonClicked);
        signUpBackButton.onClick.AddListener(() => {
            signUpPanel.SetActive(false);
            homePanel.SetActive(true);
        });

        signInLoginButton.onClick.AddListener(OnLoginButtonClicked);
        signInBackButton.onClick.AddListener(() => {
            signInPanel.SetActive(false);
            homePanel.SetActive(true);
        });

        signOutButton.onClick.AddListener(OnSignOutClicked);
        
        // Find the difficulty manager to communicate with it
        difficultyManager = FindObjectOfType<DifficultyUnlockManager>();
        
        // Auto-login check if we have cached credentials
        CheckForCachedLogin();
    }
    
    private void CheckForCachedLogin()
    {
        // This will attempt to use cached credentials if they exist
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.Log("Found cached PlayFab login, getting account info");
            GetDisplayName();
            // Update UI for logged-in state, but don't change panel visibility
            signOutButton.gameObject.SetActive(true);
            // Notify listeners that user has logged in
            OnUserLoggedIn?.Invoke();
        }
        else
        {
            Debug.Log("No cached PlayFab login found");
            // Don't change panel visibility - leave it as is
            signOutButton.gameObject.SetActive(false);
            homeStatusText.text = "Not signed in";
        }
    }

    public void ShowHomePanel()
    {
        homePanel.SetActive(true);
        signUpPanel.SetActive(false);
        signInPanel.SetActive(false);
        
        // Only show sign out button if logged in
        signOutButton.gameObject.SetActive(PlayFabClientAPI.IsClientLoggedIn());

        if (PlayerPrefs.HasKey("DisplayName") && PlayFabClientAPI.IsClientLoggedIn())
        {
            homeStatusText.text = "Logged in as: " + PlayerPrefs.GetString("DisplayName");
        }
        else
        {
            homeStatusText.text = "Not signed in";
        }
    }

    public void ShowSignUpPanel()
    {
        signUpPanel.SetActive(true);
        signInPanel.SetActive(false);
        homePanel.SetActive(false);
    }

    public void ShowSignInPanel()
    {
        signInPanel.SetActive(true);
        signUpPanel.SetActive(false);
        homePanel.SetActive(false);
    }

    public void HideAllPanels()
    {
        homePanel.SetActive(false);
        signUpPanel.SetActive(false);
        signInPanel.SetActive(false);
        signOutButton.gameObject.SetActive(false);
        homeStatusText.text = "Not signed in";
    }

    public void OnRegisterButtonClicked()
    {
        string email = signUpEmailInputField.text;
        string username = signUpUsernameInputField.text;
        string password = signUpPasswordInputField.text;

        if (!IsValidEmail(email))
        {
            signUpStatusText.text = "Invalid email format.";
            return;
        }

        RegisterUser(email, username, password);
    }

    private void RegisterUser(string email, string username, string password)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = username,
            RequireBothUsernameAndEmail = true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, result =>
        {
            signUpStatusText.text = "Registration successful!";
            // Update display name on PlayFab
            UpdateDisplayName(username);
            Debug.Log("User registered successfully.");
            ShowHomePanel();
            
            // Notify listeners that user has logged in
            OnUserLoggedIn?.Invoke();
        }, OnRegisterError);
    }

    private void UpdateDisplayName(string displayName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result => {
                Debug.Log("Display name updated successfully to: " + result.DisplayName);
                PlayerPrefs.SetString("DisplayName", result.DisplayName);
                homeStatusText.text = "Logged in as: " + result.DisplayName;
                signOutButton.gameObject.SetActive(true);
            },
            error => {
                Debug.LogError("Error updating display name: " + error.ErrorMessage);
            });
    }

    private void OnRegisterError(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable)
        {
            signUpStatusText.text = "Email is already registered.";
        }
        else if (error.Error == PlayFabErrorCode.UsernameNotAvailable)
        {
            signUpStatusText.text = "Username already taken.";
        }
        else
        {
            signUpStatusText.text = "Error: " + error.ErrorMessage;
        }

        Debug.LogError("Registration failed: " + error.GenerateErrorReport());
    }

    public void OnLoginButtonClicked()
    {
        string email = signInEmailInputField.text;
        string password = signInPasswordInputField.text;

        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        signInStatusText.text = "Login successful!";
        Debug.Log("User logged in successfully.");
        GetDisplayName();
        ShowHomePanel();
        
        // Notify listeners that user has logged in
        OnUserLoggedIn?.Invoke();
    }

    private void GetDisplayName()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
            result =>
            {
                string username = result.AccountInfo.Username;
                string displayName = result.AccountInfo.TitleInfo?.DisplayName;
                
                // If display name is empty or null, update it with username
                if (string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(username))
                {
                    UpdateDisplayName(username);
                }
                else if (!string.IsNullOrEmpty(displayName))
                {
                    PlayerPrefs.SetString("DisplayName", displayName);
                    homeStatusText.text = "Logged in as: " + displayName;
                    signOutButton.gameObject.SetActive(true);
                    Debug.Log("Display Name: " + displayName);
                }
                else
                {
                    Debug.LogWarning("Both username and display name are empty");
                }
            },
            error =>
            {
                Debug.LogWarning("Failed to get display name: " + error.GenerateErrorReport());
            });
    }

    private void OnLoginError(PlayFabError error)
    {
        signInStatusText.text = "Error: " + error.ErrorMessage;
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
    }

    private void OnSignOutClicked()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PlayerPrefs.DeleteKey("DisplayName");
        signOutButton.gameObject.SetActive(false);
        homeStatusText.text = "Not signed in";
        Debug.Log("User signed out.");
        
        // Keep the home panel active but update its UI
        // Don't call HideAllPanels() as it would hide the currently active panel
        
        // Notify listeners that user has logged out
        OnUserLoggedOut?.Invoke();
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private void CheckIfUsernameExists(string username, System.Action<bool> callback)
    {
        var request = new GetAccountInfoRequest
        {
            TitleDisplayName = username
        };

        PlayFabClientAPI.GetAccountInfo(request,
            result => callback(true),
            error => callback(false)
        );
    }

    private void CheckIfEmailExists(string email, System.Action<bool> callback)
    {
        var request = new GetAccountInfoRequest
        {
            Email = email
        };

        PlayFabClientAPI.GetAccountInfo(request,
            result => callback(true),
            error => callback(false)
        );
    }

    // Helper method to check if user is logged in
    public bool IsUserLoggedIn()
    {
        return PlayFabClientAPI.IsClientLoggedIn();
    }
}