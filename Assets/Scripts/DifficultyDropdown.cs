using UnityEngine;
using TMPro;
using System.Collections;

public class DifficultyDropdown : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;
    public TMP_Text infoText; // UI text for messages
    public TMP_Text lockStatusText;

    private bool easyCompleted;
    private bool mediumCompleted;
    private Coroutine congratsCoroutine;
    private DifficultyUnlockManager unlockManager;

    void Start()
    {
        // Wait one frame to ensure DifficultyUnlockManager is fully initialized
        StartCoroutine(InitializeDropdownAfterDelay());
    }

    IEnumerator InitializeDropdownAfterDelay()
    {
        // Wait a frame to ensure DifficultyUnlockManager is initialized
        yield return null;
        
        if (difficultyDropdown != null)
        {
            // Populate dropdown with difficulty options
            difficultyDropdown.ClearOptions();
            difficultyDropdown.AddOptions(new System.Collections.Generic.List<string> { "Easy", "Medium", "Hard" });

            // Find the manager instance
            unlockManager = DifficultyUnlockManager.Instance;
            
            if (unlockManager == null)
            {
                Debug.LogError("DifficultyUnlockManager instance not found! Creating a new manager.");
                
                // Create new GameObject with DifficultyUnlockManager if it doesn't exist
                GameObject managerObject = new GameObject("DifficultyUnlockManager");
                unlockManager = managerObject.AddComponent<DifficultyUnlockManager>();
                DontDestroyOnLoad(managerObject);
                unlockManager.InitializeDifficultyStatus();
            }

            // Subscribe to difficulty status change events
            unlockManager.OnDifficultyStatusChanged += UpdateDifficultyDisplay;
            
            // Get initial completion status
            RefreshCompletionStatus();
            
            // Load saved difficulty selection
            difficultyDropdown.value = PlayerPrefs.GetInt("SelectedDifficulty", 0); // Default to Easy (0)
            
            // Lock unavailable difficulties
            LockUnavailableDifficulties();

            // Add listener for dropdown changes
            difficultyDropdown.onValueChanged.AddListener(ValidateSelection);

            // Update lock status text
            UpdateLockStatus();

            // Start scrolling text if all difficulties are unlocked
            UpdateCongratulationsMessage();
            
            Debug.Log($"DifficultyDropdown initialized with: Easy: {easyCompleted}, Medium: {mediumCompleted}");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (unlockManager != null)
        {
            unlockManager.OnDifficultyStatusChanged -= UpdateDifficultyDisplay;
        }
        
        // Stop any running coroutines
        if (congratsCoroutine != null)
        {
            StopCoroutine(congratsCoroutine);
        }
    }

    public void UpdateDifficultyDisplay()
    {
        RefreshCompletionStatus();
        LockUnavailableDifficulties();
        UpdateLockStatus();
        UpdateCongratulationsMessage();
        
        // Ensure selected difficulty is valid
        ValidateSelection(difficultyDropdown.value);
        
        Debug.Log("Difficulty display updated");
    }
    
    private void RefreshCompletionStatus()
    {
        if (unlockManager != null)
        {
            easyCompleted = unlockManager.IsDifficultyUnlocked(1);
            mediumCompleted = unlockManager.IsDifficultyUnlocked(2);
            Debug.Log($"Refreshed completion status: Easy: {easyCompleted}, Medium: {mediumCompleted}");
        }
    }

    void LockUnavailableDifficulties()
    {
        if (difficultyDropdown != null && difficultyDropdown.options.Count >= 3)
        {
            difficultyDropdown.options[1].text = easyCompleted ? "Medium" : "Medium (Locked)";
            difficultyDropdown.options[2].text = mediumCompleted ? "Hard" : "Hard (Locked)";
            difficultyDropdown.RefreshShownValue();
        }
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
        // Check if we need to override the selection
        if (selectedIndex == 1 && !easyCompleted)
        {
            difficultyDropdown.value = 0; // Reset to Easy
            ShowMessage("Complete Easy mode to unlock Medium!");
            return;
        }
        else if (selectedIndex == 2 && !mediumCompleted)
        {
            difficultyDropdown.value = easyCompleted ? 1 : 0; // Reset to Medium if unlocked, otherwise Easy
            ShowMessage("Complete Medium mode to unlock Hard!");
            return;
        }
        
        // If we reach here, the selection is valid
        PlayerPrefs.SetInt("SelectedDifficulty", selectedIndex);
        PlayerPrefs.Save();
        Debug.Log($"Saved difficulty selection: {selectedIndex}");
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

    void UpdateCongratulationsMessage()
    {
        // Stop any existing congratulation message
        if (congratsCoroutine != null)
        {
            StopCoroutine(congratsCoroutine);
            congratsCoroutine = null;
        }
        
        // Start scrolling text if all difficulties are unlocked
        if (easyCompleted && mediumCompleted)
        {
            congratsCoroutine = StartCoroutine(ShowUnlockMessage());
        }
    }

    IEnumerator ShowUnlockMessage()
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
            else
            {
                yield return null;
            }
        }
    }

    public void CompleteEasyMode()
    {
        Debug.Log("CompleteEasyMode() called - About to unlock Medium difficulty");

        if (unlockManager != null)
        {
            unlockManager.CompleteEasyMode();
            Debug.Log("Medium difficulty should now be unlocked!");
        }
        else
        {
            Debug.LogError("Cannot complete Easy mode - DifficultyUnlockManager not found!");
        }
    }

    public void CompleteMediumMode()
    {
        Debug.Log("CompleteMediumMode() called - About to unlock Hard difficulty");

        if (unlockManager != null)
        {
            unlockManager.CompleteMediumMode();
            Debug.Log("Hard difficulty should now be unlocked!");
        }
        else
        {
            Debug.LogError("Cannot complete Medium mode - DifficultyUnlockManager not found!");
        }
    }
}