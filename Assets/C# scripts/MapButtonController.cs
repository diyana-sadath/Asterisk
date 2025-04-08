using UnityEngine;
using UnityEngine.UI;

public class MapButtonController : MonoBehaviour
{
    // Reference to the Coming Soon panel game object
    [SerializeField] private GameObject comingSoonPanel;
    
    // Reference to the Map button
    [SerializeField] private Button mapButton;
    
    // Reference to the Back button on the Coming Soon panel
    [SerializeField] private Button backButton;

    void Start()
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
    }

    // This method is called when the Map button is clicked
    public void OnMapButtonClicked()
    {
        // Show the Coming Soon panel overlay
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
}