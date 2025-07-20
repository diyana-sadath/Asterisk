using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class LeaderboardScrollView : MonoBehaviour
{
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContent;
    public ScrollRect scrollRect;
    public TMP_Dropdown modeDropdown; // Reference to your Dropdown

    // Updated dictionary to map dropdown indices to statistic names
    private string[] leaderboardStatistics = new string[] 
    {
        "EasyScore",
        "MediumScore",
        "HardScore"  // Changed from HighScore to HardScore
    };

    private void Start()
    {
        // Check if user is logged in to PlayFab
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("User not logged in. Leaderboards may not display correctly.");
        }

        modeDropdown.onValueChanged.AddListener(OnModeChanged);
        OnModeChanged(0); // Load the default leaderboard (Easy)
    }

    public void OnModeChanged(int modeIndex)
    {
        if (modeIndex < 0 || modeIndex >= leaderboardStatistics.Length)
        {
            Debug.LogError($"Invalid mode index: {modeIndex}");
            return;
        }

        string selectedStatistic = leaderboardStatistics[modeIndex];
        GetLeaderboard(selectedStatistic);
    }

    public void GetLeaderboard(string statisticName)
    {
        Debug.Log($"Fetching leaderboard for {statisticName}");

        var request = new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = 0,
            MaxResultsCount = 10,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true
            }
        };

        PlayFabClientAPI.GetLeaderboard(request,
            result => DisplayLeaderboard(result.Leaderboard, statisticName),
            error => Debug.LogError($"Error retrieving leaderboard: {error.GenerateErrorReport()}"));
    }

    private void DisplayLeaderboard(List<PlayerLeaderboardEntry> leaderboard, string statisticName)
    {
        Debug.Log($"Displaying {leaderboard.Count} entries for {statisticName}");

        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // If no entries, create a "No scores yet" entry
        if (leaderboard.Count == 0)
        {
            GameObject emptyEntry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            TMP_Text[] texts = emptyEntry.GetComponentsInChildren<TMP_Text>();
            texts[0].text = "No scores yet";
            texts[1].text = "";
            return;
        }

        // Populate new entries
        for (int i = 0; i < leaderboard.Count; i++)
        {
            var entry = leaderboard[i];
            GameObject newEntry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            TMP_Text[] texts = newEntry.GetComponentsInChildren<TMP_Text>();
            
            // Set the rank text (position + 1)
            if (texts.Length > 2)
                texts[0].text = (i + 1).ToString();
                
            // Set the player name
            texts[texts.Length > 2 ? 1 : 0].text = entry.DisplayName ?? "Anonymous";
            
            // Set the score
            texts[texts.Length > 2 ? 2 : 1].text = entry.StatValue.ToString();
        }

        // Scroll to top
        StartCoroutine(ScrollToTop());
    }

    private System.Collections.IEnumerator ScrollToTop()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 1f;
    }
}