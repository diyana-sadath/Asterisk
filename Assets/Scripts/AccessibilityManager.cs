using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AccessibilityManager : MonoBehaviour
{
    // Singleton instance
    public static AccessibilityManager Instance { get; private set; }

    [Header("High Contrast Settings")]
    public Button onButton;
    public Button offButton;

    // Color settings
    [Header("High Contrast Colors")]
    public Color highContrastBackgroundColor = Color.black;
    public Color highContrastTextColor = Color.white;
    public Color normalBackgroundColor = new Color(0, 0, 0, 0.5f);
    public Color normalTextColor = new Color(0.8f, 0.8f, 0.8f);

    // Track current state
    private bool highContrastEnabled;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved preference
            highContrastEnabled = PlayerPrefs.GetInt("HighContrast", 0) == 1;

            // Subscribe to scene loading event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetupButtonListeners();

        // Apply current preference to this scene
        ApplyHighContrastSettings();
    }

    private void SetupButtonListeners()
    {
        // Set up button listeners if we're in the accessibility scene
        if (onButton != null)
            onButton.onClick.AddListener(EnableHighContrast);

        if (offButton != null)
            offButton.onClick.AddListener(DisableHighContrast);

        // Update button visuals based on current state
        UpdateButtonVisuals();
    }

    private void UpdateButtonVisuals()
    {
        if (onButton == null || offButton == null) return;

        // Update button visuals to show selection
        if (onButton.GetComponent<Image>() != null)
            onButton.GetComponent<Image>().color = highContrastEnabled ? Color.green : Color.white;

        if (offButton.GetComponent<Image>() != null)
            offButton.GetComponent<Image>().color = highContrastEnabled ? Color.white : Color.green;
    }

    public void EnableHighContrast()
    {
        highContrastEnabled = true;
        ApplyHighContrastSettings();
        UpdateButtonVisuals();

        // Save preference
        PlayerPrefs.SetInt("HighContrast", 1);
        PlayerPrefs.Save();
    }

    public void DisableHighContrast()
    {
        highContrastEnabled = false;
        ApplyHighContrastSettings();
        UpdateButtonVisuals();

        // Save preference
        PlayerPrefs.SetInt("HighContrast", 0);
        PlayerPrefs.Save();
    }

    // Called when a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the buttons in the new scene if we're in accessibility screen
        FindAccessibilityButtons();

        // Apply high contrast settings to the new scene
        ApplyHighContrastSettings();

        // Update button visuals if they exist
        UpdateButtonVisuals();
    }

    void FindAccessibilityButtons()
    {
        // Try to find ON and OFF buttons in current scene
        GameObject onObj = GameObject.Find("OnButton");
        GameObject offObj = GameObject.Find("OffButton");

        if (onObj != null) onButton = onObj.GetComponent<Button>();
        if (offObj != null) offButton = offObj.GetComponent<Button>();

        // Re-attach listeners if buttons were found
        SetupButtonListeners();
    }

    public void ApplyHighContrastSettings()
    {
        // Find all affected UI elements in the current scene
        ApplyToImages();
        ApplyToTexts();
        ToggleFadePanel();
    }

    void ApplyToImages()
    {
        // Find and modify all images in the scene
        Image[] allImages = FindObjectsOfType<Image>(true);
        foreach (Image img in allImages)
        {
            // Skip certain UI elements if needed (e.g., buttons)
            if (ShouldSkipUIElement(img.gameObject))
                continue;

            img.color = highContrastEnabled ? highContrastBackgroundColor : normalBackgroundColor;
        }
    }

    void ApplyToTexts()
    {
        // Find and modify all texts in the scene
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI text in allTexts)
        {
            // Skip certain text elements if needed
            if (ShouldSkipUIElement(text.gameObject))
                continue;

            text.color = highContrastEnabled ? highContrastTextColor : normalTextColor;
        }
    }

    void ToggleFadePanel()
    {
        // Find and toggle fade panel if it exists
        GameObject fadePanel = GameObject.Find("FadePanel");
        if (fadePanel != null)
        {
            fadePanel.SetActive(!highContrastEnabled);
        }
    }

    bool ShouldSkipUIElement(GameObject obj)
    {
        // Add logic to skip certain UI elements if needed
        // For example, you might want to skip buttons or specific named objects
        string name = obj.name.ToLower();
        return name.Contains("button") || name.Contains("icon") || name.Contains("logo");
    }

    void OnDestroy()
    {
        // Unsubscribe from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Public API to check current state
    public bool IsHighContrastEnabled()
    {
        return highContrastEnabled;
    }
}