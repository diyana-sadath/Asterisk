using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject accessibilityManagerPrefab;

    void Awake()
    {
        // Check if AccessibilityManager already exists
        if (AccessibilityManager.Instance == null && accessibilityManagerPrefab != null)
        {
            Instantiate(accessibilityManagerPrefab);
        }
    }
}