using UnityEngine;
using System;

public class SessionCalibrationHandler : MonoBehaviour
{
    public static SessionCalibrationHandler Instance { get; private set; }

    // This is the key flag that indicates if calibration has been completed in THIS session
    private bool calibratedThisSession = false;

    // Event that other components can subscribe to
    public event Action<bool> OnSessionCalibrationStateChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Always start a new session uncalibrated
        calibratedThisSession = false;
    }

    public bool IsSessionCalibrated()
    {
        return calibratedThisSession;
    }

    public void SetSessionCalibrated(bool calibrated)
    {
        // Only fire event if there's a change
        if (calibratedThisSession != calibrated)
        {
            calibratedThisSession = calibrated;

            // Notify listeners about the session calibration state change
            OnSessionCalibrationStateChanged?.Invoke(calibrated);

            Debug.Log($"Session calibration state changed: {(calibrated ? "Calibrated" : "Not Calibrated")}");
        }
    }
}