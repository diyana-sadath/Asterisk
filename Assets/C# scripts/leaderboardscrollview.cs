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

    private Dictionary<string, string> statisticToLeaderboard = new Dictionary<string, string>
    {
        { "HighScore", "HardLeaderboard" },
        { "EasyScore", "EasyLeaderboard" },
        { "MediumScore", "MediumLeaderboard" }
    };

    private void Start()
    {
        modeDropdown.onValueChanged.AddListener(OnModeChanged);
        OnModeChanged(0); // Load the default leaderboard
    }

    public void OnModeChanged(int modeIndex)
    {
        string[] statistics = { "EasyScore", "MediumScore", "HighScore" };
        string selectedStatistic = statistics[modeIndex];
        GetLeaderboard(selectedStatistic);
    }

    public void GetLeaderboard(string statisticName)
    {
        if (!statisticToLeaderboard.TryGetValue(statisticName, out string leaderboardName))
        {
            Debug.LogError($"Leaderboard name not found for statistic: {statisticName}");
            return;
        }

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
            result => DisplayLeaderboard(result.Leaderboard, leaderboardName),
            error => Debug.LogError($"Error retrieving leaderboard: {error.GenerateErrorReport()}"));
    }

    private void DisplayLeaderboard(List<PlayerLeaderboardEntry> leaderboard, string leaderboardName)
    {
        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Populate new entries
        foreach (var entry in leaderboard)
        {
            GameObject newEntry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            TMP_Text[] texts = newEntry.GetComponentsInChildren<TMP_Text>();
            texts[0].text = entry.DisplayName ?? "Anonymous";
            texts[1].text = entry.StatValue.ToString();
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
