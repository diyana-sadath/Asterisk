using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Pause Menu UI")]
    public Button resumeButton;
    public Button optionsButton;
    public Button exitButton;
    public GameObject pausePanel;

    private void Start()
    {
        // Make sure the pause panel is hidden at start
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Add click listeners to buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitToMainMenu);
    }

    public void TogglePauseMenu()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.TogglePause();
            UpdatePauseUI();
        }
    }

    // Update UI to match game state
    public void UpdatePauseUI()
    {
        if (pausePanel != null && GameController.Instance != null)
        {
            pausePanel.SetActive(GameController.Instance.IsGamePaused());
        }
    }

    public void ResumeGame()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.ResumeGame();
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }
    }


    public void ExitToMainMenu()
    {
        // Reset time scale before switching scenes
        Time.timeScale = 1f;

        // Load the main menu scene
        SceneManager.LoadScene("HomeScreen"); // Replace with your actual main menu scene name
    }

    // Method to show the pause menu (can be called from a button click)
    public void ShowPauseMenu()
    {
        if (GameController.Instance != null && !GameController.Instance.IsGamePaused())
        {
            GameController.Instance.PauseGame();
        }

        // Always show the pause panel
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }
}