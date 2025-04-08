using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HyperIMUReceiverOptions : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform phoneDot;
    public InputField ipInputField;
    public Slider sensitivitySlider;

    [Header("Movement Settings")]
    public float movementScale = 200f;
    public float smoothing = 5f;
    public float defaultSensitivity = 1f;

    private Vector3 currentAcceleration = Vector3.zero;
    private Vector3 neutralAcceleration = Vector3.zero;
    private Vector2 targetPosition = Vector2.zero;
    private bool isCalibrated = false;

    void Start()
    {
        if (phoneDot == null)
        {
            Debug.LogError("PhoneDot is not assigned! Assign a UI Image's RectTransform in the Unity Inspector.");
            return;
        }

        phoneDot.gameObject.SetActive(true);

        // Load calibration from CalibrationSessionManager
        if (CalibrationSessionManager.Instance != null && CalibrationSessionManager.Instance.IsCalibrated)
        {
            neutralAcceleration = CalibrationSessionManager.Instance.NeutralAcceleration;
            isCalibrated = true;
            Debug.Log("✅ Loaded calibration from session manager.");
        }
        // Fallback to PlayerPrefs, only if calibration was saved
        else if (AccelerationDataManager.IsCalibrated)
        {
            neutralAcceleration = AccelerationDataManager.LoadCalibrationData();
            isCalibrated = true;
            Debug.Log("✅ Loaded calibration from PlayerPrefs.");
        }
        else
        {
            Debug.LogWarning("⚠️ No saved calibration found. Dot will not move until calibration is complete.");
        }

        if (ipInputField != null)
        {
            ipInputField.text = "127.0.0.1";
            ipInputField.onEndEdit.AddListener(ip => Debug.Log($"New IP: {ip}"));
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 1f;
            sensitivitySlider.maxValue = 5f;
            sensitivitySlider.value = SensitivityManager.Instance != null ? SensitivityManager.Instance.Sensitivity : defaultSensitivity;
            sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        }
    }

    void Update()
    {
        if (!isCalibrated || IMUReceiverManager.Instance == null)
            return;

        currentAcceleration = IMUReceiverManager.Instance.LatestAcceleration;

        float sensitivity = sensitivitySlider != null ? sensitivitySlider.value : defaultSensitivity;

        Vector3 relativeMovement = currentAcceleration - neutralAcceleration;

        float tiltFactor = Mathf.Lerp(0.1f, 1f, (sensitivity - 1f) / 4f);
        Vector2 movement = new Vector2(
            -relativeMovement.x * movementScale * tiltFactor,
            relativeMovement.y * movementScale * tiltFactor
        );

        targetPosition = Vector2.Lerp(targetPosition, movement, Time.deltaTime * smoothing);
        phoneDot.anchoredPosition = targetPosition;

        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"[Options Scene] Tilt: {relativeMovement}, Sensitivity: {sensitivity}, Dot: {movement}");
        }
    }

    void UpdateSensitivity(float value)
    {
        if (SensitivityManager.Instance != null)
        {
            SensitivityManager.Instance.SetSensitivity(value);
        }
    }
}
