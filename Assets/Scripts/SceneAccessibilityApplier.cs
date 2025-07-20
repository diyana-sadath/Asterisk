using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneAccessibilityApplier : MonoBehaviour
{
    [Header("Scene-Specific High Contrast Elements")]
    public Image[] backgroundImages;
    public TextMeshProUGUI[] textElements;
    public GameObject[] elementsToHideInHighContrast;

    [Header("Custom Colors (Optional)")]
    public bool useCustomColors = false;
    public Color customHighContrastBackground = Color.black;
    public Color customHighContrastText = Color.white;
    public Color customNormalBackground = new Color(0, 0, 0, 0.5f);
    public Color customNormalText = new Color(0.8f, 0.8f, 0.8f);

    void Start()
    {
        // Apply current settings when this scene loads
        ApplyCurrentSettings();
    }

    public void ApplyCurrentSettings()
    {
        // Check if the AccessibilityManager exists
        if (AccessibilityManager.Instance == null) return;

        bool highContrastEnabled = AccessibilityManager.Instance.IsHighContrastEnabled();

        // Get colors to use
        Color bgColor, textColor;

        if (useCustomColors)
        {
            bgColor = highContrastEnabled ? customHighContrastBackground : customNormalBackground;
            textColor = highContrastEnabled ? customHighContrastText : customNormalText;
        }
        else
        {
            bgColor = highContrastEnabled ?
                AccessibilityManager.Instance.highContrastBackgroundColor :
                AccessibilityManager.Instance.normalBackgroundColor;

            textColor = highContrastEnabled ?
                AccessibilityManager.Instance.highContrastTextColor :
                AccessibilityManager.Instance.normalTextColor;
        }

        // Apply to specifically defined elements
        foreach (Image img in backgroundImages)
        {
            if (img != null) img.color = bgColor;
        }

        foreach (TextMeshProUGUI text in textElements)
        {
            if (text != null) text.color = textColor;
        }

        // Show/hide elements based on mode
        foreach (GameObject obj in elementsToHideInHighContrast)
        {
            if (obj != null) obj.SetActive(!highContrastEnabled);
        }
    }
}