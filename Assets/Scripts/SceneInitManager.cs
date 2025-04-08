using UnityEngine;

public class SceneInitManager : MonoBehaviour
{
    // References to components that need initialization
    public BackgroundScroller[] backgroundScrollers;
    public ButtonHandlers buttonHandler;

    void Awake()
    {
        // Find components if not assigned in inspector
        if (backgroundScrollers == null || backgroundScrollers.Length == 0)
        {
            backgroundScrollers = FindObjectsByType<BackgroundScroller>(FindObjectsSortMode.None);
        }

        if (buttonHandler == null)
        {
            buttonHandler = FindFirstObjectByType<ButtonHandlers>();
        }
    }

    void Start()
    {
        // Initialize background scrollers
        foreach (BackgroundScroller scroller in backgroundScrollers)
        {
            if (scroller != null)
            {
                scroller.ResumeScroll();
            }
        }

        // Initialize buttons
        if (buttonHandler != null)
        {
            buttonHandler.ReinitializeButtons();
        }

        // Reset time scale to ensure everything runs properly
        Time.timeScale = 1f;
    }
}