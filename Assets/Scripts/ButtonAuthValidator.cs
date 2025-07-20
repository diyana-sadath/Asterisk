using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ButtonAuthValidator : MonoBehaviour
{
    public static ButtonAuthValidator Instance;

    [Header("References")]
    public ButtonHoverInfo hoverInfo;

    [Header("Authentication-Required Buttons")]
    public List<Button> restrictedButtons = new List<Button>();

    [Header("Unrestricted Buttons")]
    public List<Button> allowedButtons = new List<Button>(); // Login button and other allowed buttons

    [Header("Messages")]
    public string authRequiredMessage = "Sign in required to use this feature";
    public string hoverSuffix = " (Sign in required)";

    private PlayFabAuthManager authManager;
    private Dictionary<Button, Button.ButtonClickedEvent> originalButtonEvents =
        new Dictionary<Button, Button.ButtonClickedEvent>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Find components if not assigned
        if (hoverInfo == null)
            hoverInfo = FindFirstObjectByType<ButtonHoverInfo>();

        authManager = FindFirstObjectByType<PlayFabAuthManager>();

        if (authManager == null)
        {
            Debug.LogError("ButtonAuthValidator: PlayFabAuthManager not found!");
            return;
        }
    }

    private void Start()
    {
        // Subscribe to auth events
        authManager.OnUserLoggedIn += OnUserLoggedIn;
        authManager.OnUserLoggedOut += OnUserLoggedOut;

        // Store original click events and add validators
        SetupButtonValidators();

        // Initial button state update
        UpdateButtonStates();
    }

    public bool IsButtonRestricted(Button button)
    {
        if (button == null) return false;

        // Check if button is in the allowed list (never restricted)
        if (allowedButtons.Contains(button)) return false;

        // Check if button is in the restricted list
        return restrictedButtons.Contains(button);
    }

    private void OnDestroy()
    {
        if (authManager != null)
        {
            authManager.OnUserLoggedIn -= OnUserLoggedIn;
            authManager.OnUserLoggedOut -= OnUserLoggedOut;
        }
    }

    private void SetupButtonValidators()
    {
        // Process all restricted buttons
        foreach (var button in restrictedButtons)
        {
            if (button == null) continue;

            // Skip buttons that are in the allowed list
            if (allowedButtons.Contains(button)) continue;

            // Create a copy of the original events
            Button.ButtonClickedEvent originalEvent = new Button.ButtonClickedEvent();

            // Store current listeners (we can't directly access them)
            var currentEvent = button.onClick;

            // Store the original event for later use
            originalButtonEvents[button] = currentEvent;

            // Clear and replace with our validator
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(() => ValidateButtonClick(button));

            // Add hover component if needed
            EnsureHoverComponent(button);

            Debug.Log($"Set up validation for button: {button.name}");
        }
    }

    private void EnsureHoverComponent(Button button)
    {
        // Add a UnifiedButtonHover component if it doesn't exist
        var buttonObj = button.gameObject;
        var hoverComponent = buttonObj.GetComponent<UnifiedButtonHover>();
        if (hoverComponent == null)
        {
            hoverComponent = buttonObj.AddComponent<UnifiedButtonHover>();
            // Set default description to button name if needed
            hoverComponent.buttonDescription = button.name;
        }
    }

    public void ValidateButtonClick(Button button)
    {
        if (authManager.IsUserLoggedIn())
        {
            // User is logged in, invoke original click events
            if (originalButtonEvents.ContainsKey(button))
            {
                originalButtonEvents[button].Invoke();
            }
        }
        else
        {
            // User is not logged in, show auth message but DON'T redirect
            Debug.Log("Authentication required to use this feature");

            // Show temporary message in hover info
            if (hoverInfo != null)
            {
                hoverInfo.ShowButtonInfo(authRequiredMessage);

                // Reset the hover info after a delay
                CancelInvoke("ClearHoverInfo");
                Invoke("ClearHoverInfo", 3.0f);
            }

            // Don't redirect to login screen automatically
            // authManager.ShowSignInPanel(); -- Remove this so it doesn't auto-redirect
        }
    }

    private void ClearHoverInfo()
    {
        if (hoverInfo != null)
        {
            hoverInfo.HideButtonInfo();
        }
    }

    private void OnUserLoggedIn()
    {
        UpdateButtonStates();
    }

    private void OnUserLoggedOut()
    {
        UpdateButtonStates();
    }

    // In ButtonAuthValidator.cs - Update the UpdateButtonStates method

    private void UpdateButtonStates()
    {
        bool isLoggedIn = authManager.IsUserLoggedIn();

        foreach (var button in restrictedButtons)
        {
            if (button != null && !allowedButtons.Contains(button))
            {
                // Special handling for Start button - need to check both login AND calibration
                if (button.name.Contains("StartButton") || button.name == "Start")
                {
                    // Don't enable the button here - let CalibrationRequirement control its state
                    // Just update the visual feedback
                    ColorBlock colors = button.colors;
                    if (isLoggedIn)
                    {
                        colors.disabledColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, 0.5f);
                    }
                    else
                    {
                        colors.disabledColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, 0.3f);
                    }
                    button.colors = colors;
                }
                else
                {
                    // Normal behavior for other buttons
                    button.interactable = isLoggedIn;

                    // Update colors for visual feedback
                    ColorBlock colors = button.colors;
                    if (isLoggedIn)
                    {
                        colors.disabledColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, 0.5f);
                    }
                    else
                    {
                        colors.disabledColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, 0.3f);
                    }
                    button.colors = colors;
                }
            }
        }
    }

    // This method will be called by UnifiedButtonHover components to modify hover text
    public string ModifyHoverText(string originalText, Button button)
    {
        // For restricted buttons when not logged in, replace with auth message
        if (!authManager.IsUserLoggedIn() && IsButtonRestricted(button) && !allowedButtons.Contains(button))
        {
            return authRequiredMessage;
        }

        return originalText;
    }
}