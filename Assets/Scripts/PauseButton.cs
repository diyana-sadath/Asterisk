using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseButton : MonoBehaviour
{
    private Button button;
    private PauseMenuController pauseMenuController;

    void Start()
    {
        button = GetComponent<Button>();
        pauseMenuController = FindFirstObjectByType<PauseMenuController>();

        if (button != null && pauseMenuController != null)
        {
            button.onClick.AddListener(OnPauseButtonClicked);
        }
        else
        {
            Debug.LogError("PauseButton: Missing Button component or PauseMenuController in scene");
        }
    }

    void OnPauseButtonClicked()
    {
        if (pauseMenuController != null)
        {
            pauseMenuController.TogglePauseMenu(); // Use the toggle method instead
        }
        else if (GameController.Instance != null)
        {
            // Fallback if controller isn't found
            GameController.Instance.PauseGame();
        }
    }
}