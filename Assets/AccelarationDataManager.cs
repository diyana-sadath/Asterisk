using UnityEngine;

public static class AccelerationDataManager
{
    private const string CalibrationXKey = "CalibrationX";
    private const string CalibrationYKey = "CalibrationY";
    private const string CalibrationZKey = "CalibrationZ";
    private const string IsCalibratedKey = "IsCalibrated";

    public static Vector3 NeutralAcceleration { get; private set; } = Vector3.zero;
    public static bool IsCalibrated { get; private set; } = false;

    public static void SaveCalibrationData(Vector3 calibration)
    {
        NeutralAcceleration = calibration;
        IsCalibrated = true;

        PlayerPrefs.SetFloat(CalibrationXKey, calibration.x);
        PlayerPrefs.SetFloat(CalibrationYKey, calibration.y);
        PlayerPrefs.SetFloat(CalibrationZKey, calibration.z);
        PlayerPrefs.SetInt(IsCalibratedKey, 1);
        PlayerPrefs.Save();

        Debug.Log($"Calibration data saved: {NeutralAcceleration}");
    }

    public static Vector3 LoadCalibrationData()
    {
        if (!HasCalibrationData())
        {
            Debug.LogWarning("⚠️ No calibration data found, returning default Vector3.zero.");
            NeutralAcceleration = Vector3.zero;
            IsCalibrated = false;
            return NeutralAcceleration;
        }

        NeutralAcceleration = new Vector3(
            PlayerPrefs.GetFloat(CalibrationXKey, 0f),
            PlayerPrefs.GetFloat(CalibrationYKey, 0f),
            PlayerPrefs.GetFloat(CalibrationZKey, 0f)
        );
        IsCalibrated = PlayerPrefs.GetInt(IsCalibratedKey, 0) == 1;

        Debug.Log($"Calibration data loaded: {NeutralAcceleration}");
        return NeutralAcceleration;
    }

    public static bool HasCalibrationData()
    {
        return PlayerPrefs.HasKey(CalibrationXKey) &&
               PlayerPrefs.HasKey(CalibrationYKey) &&
               PlayerPrefs.HasKey(CalibrationZKey);
    }

    public static void ClearCalibration()
    {
        NeutralAcceleration = Vector3.zero;
        IsCalibrated = false;

        PlayerPrefs.DeleteKey(CalibrationXKey);
        PlayerPrefs.DeleteKey(CalibrationYKey);
        PlayerPrefs.DeleteKey(CalibrationZKey);
        PlayerPrefs.DeleteKey(IsCalibratedKey);
        PlayerPrefs.Save();

        Debug.Log("Calibration data cleared.");
    }
}
