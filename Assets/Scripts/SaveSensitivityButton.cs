using UnityEngine;
using UnityEngine.UI;

public class SensitivitySaveButton : MonoBehaviour
{
    public Button saveButton;
    public Slider sensitivitySlider;
    public Text feedbackText; // Optional: UI text to show save confirmation

    private void Start()
    {
        // Make sure we have a button
        if (saveButton == null)
        {
            saveButton = GetComponent<Button>();
            if (saveButton == null)
            {
                Debug.LogError("No save button assigned or found on this GameObject!");
                return;
            }
        }

        // Find slider if not assigned
        if (sensitivitySlider == null)
        {
            sensitivitySlider = FindObjectOfType<Slider>();
            if (sensitivitySlider == null)
            {
                Debug.LogError("No sensitivity slider assigned or found in scene!");
                return;
            }
        }

        // Add click listener to the button
        saveButton.onClick.AddListener(SaveSensitivity);
    }

    public void SaveSensitivity()
    {
        // Get the current slider value
        float sliderValue = sensitivitySlider.value;

        // Convert to actual sensitivity value (1-5 range)
        float actualSensitivity = Mathf.Clamp(sliderValue, 1f, 5f);

        Debug.Log($"Saving sensitivity - Slider Value: {sliderValue}, Actual Sensitivity: {actualSensitivity}");

        // 1. Save to PlayerPrefs (always do this as the basic fallback)
        PlayerPrefs.SetFloat("SensitivityLevel", sliderValue);
        PlayerPrefs.Save();

        // 2. Save to SensitivityManager if it exists
        if (SensitivityManager.Instance != null)
        {
            SensitivityManager.Instance.SetSensitivity(sliderValue);
            Debug.Log("Saved to SensitivityManager");
        }

        // 3. Save to GameSettingsManager if it exists
        if (GameSettingsManager.Instance != null)
        {
            GameSettingsManager.Instance.SaveSensitivity(sliderValue);
            Debug.Log("Saved to GameSettingsManager");
        }

        // 4. Optional: Show feedback to the user
        if (feedbackText != null)
        {
            feedbackText.text = "Sensitivity Saved!";
            StartCoroutine(HideFeedbackAfterDelay(1.5f));
        }
    }

    private System.Collections.IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    private void OnDestroy()
    {
        // Remove listener when destroyed to prevent memory leaks
        if (saveButton != null)
        {
            saveButton.onClick.RemoveListener(SaveSensitivity);
        }
    }
}