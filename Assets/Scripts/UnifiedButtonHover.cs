using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnifiedButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Information")]
    public string buttonDescription; // Set this in the Inspector for each button

    private ButtonAuthValidator authValidator;
    private ButtonHoverInfo hoverInfo;
    private Button buttonComponent;
    private CalibrationRequirement calibrationRequirement;
    private SessionCalibrationHandler sessionCalibration;

    private void Start()
    {
        // Find the instances if they exist in the scene
        authValidator = ButtonAuthValidator.Instance;
        hoverInfo = ButtonHoverInfo.Instance;
        buttonComponent = GetComponent<Button>();
        calibrationRequirement = CalibrationRequirement.Instance;
        sessionCalibration = SessionCalibrationHandler.Instance;

        // If buttonDescription is empty, use the GameObject name
        if (string.IsNullOrEmpty(buttonDescription))
        {
            buttonDescription = gameObject.name;
        }
    }

    // In UnifiedButtonHover.cs - Update OnPointerEnter method

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverInfo != null)
        {
            string displayText = buttonDescription;
            bool textModified = false;

            // Check for Start button which needs special handling
            if (buttonComponent != null && (gameObject.name.Contains("StartButton") || gameObject.name == "Start"))
            {
                // First check if user is logged in
                bool isLoggedIn = true;
                var authValidator = ButtonAuthValidator.Instance;
                if (authValidator != null)
                {
                    // Get auth manager via reflection
                    var authManagerField = typeof(ButtonAuthValidator).GetField("authManager",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (authManagerField != null)
                    {
                        var authManager = authManagerField.GetValue(authValidator);
                        if (authManager != null)
                        {
                            var isLoggedInMethod = authManager.GetType().GetMethod("IsUserLoggedIn");
                            if (isLoggedInMethod != null)
                            {
                                isLoggedIn = (bool)isLoggedInMethod.Invoke(authManager, null);
                            }
                        }
                    }
                }

                if (!isLoggedIn)
                {
                    displayText = "Sign in required to use this feature";
                    textModified = true;
                }
                else
                {
                    // User is logged in, check calibration
                    bool needsCalibration = true;

                    // Check session calibration first
                    if (sessionCalibration != null)
                    {
                        needsCalibration = !sessionCalibration.IsSessionCalibrated();
                        Debug.Log($"Session calibration check: needs calibration = {needsCalibration}");
                    }
                    // Fallback to regular calibration check
                    else if (calibrationRequirement != null)
                    {
                        needsCalibration = !calibrationRequirement.IsCalibrated();
                        Debug.Log($"Regular calibration check: needs calibration = {needsCalibration}");
                    }
                    else
                    {
                        Debug.LogWarning("No calibration handlers found!");
                    }

                    if (needsCalibration)
                    {
                        // Change this line to show your desired message
                        displayText = "Please calibrate!";
                        textModified = true;
                        Debug.Log("Setting text to 'Please calibrate!'");
                    }
                }
            }
            // Only check auth if not the Start button or calibration didn't modify text
            else if (!textModified && authValidator != null && buttonComponent != null)
            {
                // Let the validator handle the text modification
                displayText = authValidator.ModifyHoverText(displayText, buttonComponent);
            }

            hoverInfo.ShowButtonInfo(displayText);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverInfo != null)
        {
            hoverInfo.HideButtonInfo();
        }
    }
}