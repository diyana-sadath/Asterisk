using UnityEngine;
using UnityEngine.UI;

public class ResumeButtonFix : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            // Clear any existing listeners and add our own
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnResumeClicked);
            Debug.Log("Resume button listener attached");
        }
        else
        {
            Debug.LogError("No Button component found on ResumeButton!");
        }
    }

    void OnResumeClicked()
    {
        Debug.Log("Resume button clicked!");
        if (GameController.Instance != null)
        {
            GameController.Instance.ResumeGame();
        }
        else
        {
            Debug.LogError("GameController.Instance is null!");
        }
    }
}