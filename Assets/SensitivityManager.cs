using UnityEngine;

public class SensitivityManager : MonoBehaviour
{
    public static SensitivityManager Instance { get; private set; }

    [SerializeField] private float sensitivity = 3.0f; // Now true 1–5 scale

    private const string SENSITIVITY_KEY = "SensitivityLevel"; // UI uses 1–5 now

    public float Sensitivity
    {
        get => sensitivity;
        set
        {
            sensitivity = Mathf.Clamp(value, 1f, 5f); // Clamp to 1–5 range
            PlayerPrefs.SetFloat(SENSITIVITY_KEY, sensitivity);
            PlayerPrefs.Save();
            Debug.Log($"SensitivityManager saved sensitivity: {sensitivity}");
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, 3.0f); // Default to 3/5
        Debug.Log($"SensitivityManager loaded: {sensitivity}");
    }

    public void SetSensitivity(float newSensitivity)
    {
        Sensitivity = newSensitivity;
    }

    public float GetSensitivity()
    {
        return sensitivity;
    }

    public static SensitivityManager EnsureExists()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("SensitivityManager");
            Instance = obj.AddComponent<SensitivityManager>();
            Debug.Log("SensitivityManager created via EnsureExists");
        }
        return Instance;
    }
}
