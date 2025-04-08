using UnityEngine;
using UnityEngine.UI;

public class HighContrastManager : MonoBehaviour
{
    public Button highContrastOnButton;
    public Button highContrastOffButton;
    public GameObject settingsPanel; // Reference to "Settings Panel"
    public GameObject panel;         // Reference to "Panel" inside "Settings Panel"

    void Start()
    {
        // Add button listeners
        highContrastOnButton.onClick.AddListener(EnableHighContrast);
        highContrastOffButton.onClick.AddListener(DisableHighContrast);

        // Load saved preference
        bool isHighContrast = PlayerPrefs.GetInt("HighContrast", 0) == 1;
        ApplyHighContrast(isHighContrast);
    }

    void EnableHighContrast()
    {
        ApplyHighContrast(true);
        PlayerPrefs.SetInt("HighContrast", 1);
        PlayerPrefs.Save();
    }

    void DisableHighContrast()
    {
        ApplyHighContrast(false);
        PlayerPrefs.SetInt("HighContrast", 0);
        PlayerPrefs.Save();
    }

    void ApplyHighContrast(bool enable)
    {
        // Change background color
        Color bgColor = enable ? Color.black : new Color(0.85f, 0.85f, 0.85f);
        if (settingsPanel) settingsPanel.GetComponent<Image>().color = bgColor;
        if (panel) panel.GetComponent<Image>().color = bgColor;

        // Change text colors
        Color textColor = enable ? Color.white : Color.black;
        foreach (Text text in settingsPanel.GetComponentsInChildren<Text>(true))
        {
            text.color = textColor;
        }

        // Change button colors
        foreach (Button button in settingsPanel.GetComponentsInChildren<Button>(true))
        {
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage)
            {
                buttonImage.color = button.name == "HELP" || button.name == "Tutorial button"
                    ? Color.clear 
                    : bgColor;
            }
        }
    }
}
