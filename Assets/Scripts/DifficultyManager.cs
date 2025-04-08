using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public GameObject easySpawner;
    public GameObject mediumSpawner;
    public GameObject hardSpawner;

    private void Start()
    {
        // Get saved difficulty from PlayerPrefs (default to Medium if not set)
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "Medium");

        // Enable the correct spawner
        SetDifficulty(difficulty);
    }

    public void SetDifficulty(string difficulty)
    {
        // Disable all spawners first
        easySpawner.SetActive(false);
        mediumSpawner.SetActive(false);
        hardSpawner.SetActive(false);

        // Enable the correct spawner
        switch (difficulty)
        {
            case "Easy":
                easySpawner.SetActive(true);
                break;
            case "Medium":
                mediumSpawner.SetActive(true);
                break;
            case "Hard":
                hardSpawner.SetActive(true);
                break;
        }

        // Save the selection
        PlayerPrefs.SetString("SelectedDifficulty", difficulty);
        PlayerPrefs.Save();
    }
}
