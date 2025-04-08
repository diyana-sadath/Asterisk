using UnityEngine;
using UnityEngine.UI;

public class ButtonTransition : MonoBehaviour
{
    public string targetScene;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(TransitionToScene);
        }
        else
        {
            Debug.LogError("ButtonTransition script must be attached to a GameObject with a Button component!");
        }
    }

    public void TransitionToScene()
    {
        // Play click sound
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayButtonClick();

        // ✅ Save Sensitivity (only if SensitivityManager exists)
        if (SensitivityManager.Instance != null)
        {
            PlayerPrefs.SetFloat("GlobalSensitivity", SensitivityManager.Instance.Sensitivity);
            PlayerPrefs.Save();
            Debug.Log($"✅ Sensitivity saved: {SensitivityManager.Instance.Sensitivity}");
        }

        // Handle fade and scene change
        if (CodeOnlyFader.Instance != null)
        {
            Debug.Log("Button clicked, transitioning to: " + targetScene);
            CodeOnlyFader.Instance.FadeToScene(targetScene);
        }
        else
        {
            Debug.LogError("No CodeOnlyFader instance found in the scene! Make sure one exists in your starting scene.");
        }
    }
}
