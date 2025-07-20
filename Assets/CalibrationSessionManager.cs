using UnityEngine;
using UnityEngine.UI;
using System;

public class CalibrationSessionManager : MonoBehaviour
{
    public static CalibrationSessionManager Instance { get; private set; }

    public Vector3 NeutralAcceleration { get; private set; }
    public bool IsCalibrated { get; private set; }

    // Add events for calibration state changes
    public event Action OnCalibrationComplete;

    [Header("Button References")]
    public Button calibrateButton; // The button that initiates calibration
    public Button startButtonOnCalibrationScreen; // The start button on calibration screen

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Try to load calibration data from PlayerPrefs
        if (PlayerPrefs.HasKey("CalibrationX") &&
            PlayerPrefs.HasKey("CalibrationY") &&
            PlayerPrefs.HasKey("CalibrationZ"))
        {
            float x = PlayerPrefs.GetFloat("CalibrationX");
            float y = PlayerPrefs.GetFloat("CalibrationY");
            float z = PlayerPrefs.GetFloat("CalibrationZ");

            NeutralAcceleration = new Vector3(x, y, z);
            IsCalibrated = true;

            // Notify any listeners
            NotifyCalibrationComplete();
        }
    }

    private void Start()
    {
        // If CalibrationRequirement exists, sync it with our state on startup
        if (CalibrationRequirement.Instance != null)
        {
            // This will trigger any UI updates needed
            CalibrationRequirement.Instance.SetCalibrated(IsCalibrated);
        }
    }

    public void SetCalibration(Vector3 neutral)
    {
        NeutralAcceleration = neutral;
        IsCalibrated = true;

        // Save calibration to PlayerPrefs
        PlayerPrefs.SetFloat("CalibrationX", neutral.x);
        PlayerPrefs.SetFloat("CalibrationY", neutral.y);
        PlayerPrefs.SetFloat("CalibrationZ", neutral.z);
        PlayerPrefs.SetInt("IsCalibrated", 1);  // Add this line to explicitly track calibration state
        PlayerPrefs.Save();

        // Update the CalibrationRequirement component
        if (CalibrationRequirement.Instance != null)
        {
            CalibrationRequirement.Instance.SetCalibrated(true);
        }

        // Notify any listeners
        NotifyCalibrationComplete();

        Debug.Log("Calibration completed and saved");
    }

    public void ClearCalibration()
    {
        IsCalibrated = false;
        NeutralAcceleration = Vector3.zero;

        // Clear from PlayerPrefs
        PlayerPrefs.DeleteKey("CalibrationX");
        PlayerPrefs.DeleteKey("CalibrationY");
        PlayerPrefs.DeleteKey("CalibrationZ");
        PlayerPrefs.DeleteKey("IsCalibrated");  // Also clear this key
        PlayerPrefs.Save();

        // Update the CalibrationRequirement component
        if (CalibrationRequirement.Instance != null)
        {
            CalibrationRequirement.Instance.SetCalibrated(false);
        }

        Debug.Log("Calibration cleared");
    }

    private void NotifyCalibrationComplete()
    {
        OnCalibrationComplete?.Invoke();
    }
}