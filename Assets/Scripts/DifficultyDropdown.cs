using UnityEngine;
using TMPro;

public class DifficultyDropdown : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;
    public TMP_Text infoText; // UI text for messages
    public TMP_Text lockStatusText;

    private bool easyCompleted;
    private bool mediumCompleted;

    void Start()
    {
        if (difficultyDropdown != null)
        {
            // Populate dropdown with difficulty options
            difficultyDropdown.ClearOptions();
            difficultyDropdown.AddOptions(new System.Collections.Generic.List<string> { "Easy", "Medium", "Hard" });

            // Find the DifficultyUnlockManager to check completion status
            DifficultyUnlockManager unlockManager = FindAnyObjectByType<DifficultyUnlockManager>();

            if (unlockManager == null)
            {
                // Create new GameObject with DifficultyUnlockManager if it doesn't exist
                GameObject managerObject = new GameObject("DifficultyUnlockManager");
                unlockManager = managerObject.AddComponent<DifficultyUnlockManager>();
                DontDestroyOnLoad(managerObject);
                Debug.Log("Created new DifficultyUnlockManager from DifficultyDropdown");
            }

            // Get completion status from the manager
            easyCompleted = unlockManager.IsDifficultyUnlocked(1); // Check if Medium (1) is unlocked
            mediumCompleted = unlockManager.IsDifficultyUnlocked(2); // Check if Hard (2) is unlocked

            // Load saved difficulty selection
            difficultyDropdown.value = PlayerPrefs.GetInt("SelectedDifficulty", 0); // Default to Easy (0)

            Debug.Log($"DifficultyDropdown loaded status: Easy completed: {easyCompleted}, Medium completed: {mediumCompleted}");

            // Lock unavailable difficulties
            LockUnavailableDifficulties();

            // Add listener for dropdown changes
            difficultyDropdown.onValueChanged.AddListener(ValidateSelection);

            // Update lock status text
            UpdateLockStatus();

            // Start scrolling text if all difficulties are unlocked
            if (easyCompleted && mediumCompleted)
            {
                StartCoroutine(ShowUnlockMessage());
            }
        }
    }

    void LockUnavailableDifficulties()
    {
        difficultyDropdown.options[1].text = easyCompleted ? "Medium" : "Medium (Locked)";
        difficultyDropdown.options[2].text = mediumCompleted ? "Hard" : "Hard (Locked)";

        difficultyDropdown.RefreshShownValue();
    }

    void UpdateLockStatus()
    {
        if (lockStatusText != null)
        {
            if (!easyCompleted && !mediumCompleted)
            {
                lockStatusText.text = "Complete Easy mode to unlock Medium, then Medium to unlock Hard!";
            }
            else if (easyCompleted && !mediumCompleted)
            {
                lockStatusText.text = "Medium unlocked! Complete Medium to unlock Hard.";
            }
            else if (easyCompleted && mediumCompleted)
            {
                lockStatusText.text = "All difficulty levels unlocked!";
            }
        }
    }

    void ValidateSelection(int selectedIndex)
    {
        // Store the original selection to avoid infinite recursion
        int originalSelection = selectedIndex;

        if (selectedIndex == 1 && !easyCompleted)
        {
            difficultyDropdown.value = 0; // Reset to Easy
            ShowMessage("Complete Easy mode to unlock Medium!");
        }
        else if (selectedIndex == 2 && !mediumCompleted)
        {
            difficultyDropdown.value = easyCompleted ? 1 : 0; // Reset to Medium if unlocked, otherwise Easy
            ShowMessage("Complete Medium mode to unlock Hard!");
        }
        else
        {
            // Save the selected difficulty if it's valid
            PlayerPrefs.SetInt("SelectedDifficulty", selectedIndex);
            PlayerPrefs.Save();
            Debug.Log($"Saved difficulty selection: {selectedIndex}");
        }
    }

    void ShowMessage(string message)
    {
        if (infoText != null)
        {
            infoText.text = message;
            CancelInvoke("ClearMessage");
            Invoke("ClearMessage", 2.5f); // Clear message after 2.5 seconds
        }
    }

    void ClearMessage()
    {
        if (infoText != null)
        {
            infoText.text = "";
        }
    }

    System.Collections.IEnumerator ShowUnlockMessage()
    {
        while (true)
        {
            if (infoText != null)
            {
                infoText.text = "Congratulations! All difficulties are unlocked.";
                yield return new WaitForSeconds(1.5f);
                infoText.text = "";
                yield return new WaitForSeconds(1.5f);
            }
        }
    }

    public void CompleteEasyMode()
    {
        Debug.Log("CompleteEasyMode() called - About to unlock Medium difficulty");

        // Find the manager and use it to unlock
        DifficultyUnlockManager unlockManager = FindAnyObjectByType<DifficultyUnlockManager>();
        if (unlockManager != null)
        {
            unlockManager.CompleteEasyMode();
        }
        else
        {
            // Fallback to direct PlayerPrefs if no manager found
            PlayerPrefs.SetInt("EasyCompleted", 1);
            PlayerPrefs.Save();
        }

        // Update our local state
        easyCompleted = true;

        // Immediately update the UI
        difficultyDropdown.options[1].text = "Medium";
        difficultyDropdown.RefreshShownValue();

        // Update lock status and show message
        UpdateLockStatus();
        ShowMessage("Medium difficulty unlocked!");

        Debug.Log("Medium difficulty should now be unlocked!");
    }

    public void CompleteMediumMode()
    {
        Debug.Log("CompleteMediumMode() called - About to unlock Hard difficulty");

        // Find the manager and use it to unlock
        DifficultyUnlockManager unlockManager = FindAnyObjectByType<DifficultyUnlockManager>();
        if (unlockManager != null)
        {
            unlockManager.CompleteMediumMode();
        }
        else
        {
            // Fallback to direct PlayerPrefs if no manager found
            PlayerPrefs.SetInt("MediumCompleted", 1);
            PlayerPrefs.Save();
        }

        // Update our local state
        mediumCompleted = true;

        // Immediately update the UI
        difficultyDropdown.options[2].text = "Hard";
        difficultyDropdown.RefreshShownValue();

        // Update lock status and show message
        UpdateLockStatus();
        ShowMessage("Hard difficulty unlocked!");

        Debug.Log("Hard difficulty should now be unlocked!");
    }
}