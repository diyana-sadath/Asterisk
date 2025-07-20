using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonHoverInfo : MonoBehaviour
{
    public static ButtonHoverInfo Instance;

    [Header("References")]
    public GameObject infoPanel;
    public TextMeshProUGUI infoText;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Make sure panel is initially hidden
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    public void ShowButtonInfo(string buttonText)
    {
        if (infoPanel != null && infoText != null)
        {
            infoPanel.SetActive(true);
            infoText.text = buttonText;
        }
    }

    public void HideButtonInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}