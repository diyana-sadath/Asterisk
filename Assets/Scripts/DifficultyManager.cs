using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public GameObject easySpawner;
    public GameObject mediumSpawner;
    public GameObject hardSpawner;

    private void Start()
    {
        // In DifficultyManager.cs - Start() method
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "Medium");

        // We need to also set the integer value for other components that use it
        PlayerPrefs.SetInt("SelectedDifficulty", difficulty == "Easy" ? 0 : (difficulty == "Medium" ? 1 : 2));
        PlayerPrefs.Save();


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
