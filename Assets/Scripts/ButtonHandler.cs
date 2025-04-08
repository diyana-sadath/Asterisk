using UnityEngine;
using UnityEngine.UI;

public class ButtonHandlers : MonoBehaviour
{
    // Reference to buttons that need to be re-initialized
    public Button[] sceneButtons;

    void OnEnable()
    {
        // Re-initialize all buttons when this GameObject is enabled
        ReinitializeButtons();
    }

    public void ReinitializeButtons()
    {
        // If no buttons assigned in inspector, find all buttons in children
        if (sceneButtons == null || sceneButtons.Length == 0)
        {
            sceneButtons = GetComponentsInChildren<Button>(true);
        }

        foreach (Button button in sceneButtons)
        {
            if (button != null)
            {
                // Get the ButtonTransition component
                ButtonTransition transition = button.GetComponent<ButtonTransition>();
                if (transition != null)
                {
                    // Remove all listeners and re-add the transition listener
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(transition.TransitionToScene);
                }
            }
        }
    }
}