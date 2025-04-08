using UnityEngine;

public class CalibrationSessionManager : MonoBehaviour
{
    public static CalibrationSessionManager Instance { get; private set; }

    public Vector3 NeutralAcceleration { get; private set; }
    public bool IsCalibrated { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCalibration(Vector3 neutral)
    {
        NeutralAcceleration = neutral;
        IsCalibrated = true;
    }
}
