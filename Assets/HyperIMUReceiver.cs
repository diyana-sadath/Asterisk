using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HyperIMUReceiver : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform phoneDot;

    [Header("Movement Settings")]
    public float movementScale = 200f;
    public float smoothing = 5f;
    public float defaultSensitivity = 1f;

    private Vector2 targetPosition = Vector2.zero;
    private Vector3 neutralAcceleration = Vector3.zero;
    private Vector3 currentAcceleration = Vector3.zero;
    private bool isCalibrated = false;
    private int frameCount = 0;

    void Start()
    {
        if (phoneDot == null)
        {
            Debug.LogError("PhoneDot is not assigned! Please assign a UI Image's RectTransform in the Unity Inspector.");
            return;
        }

        phoneDot.gameObject.SetActive(false);

        if (IMUReceiverManager.Instance == null)
        {
            Debug.LogError("No IMUReceiverManager found in the scene! Please ensure it's added to your project.");
        }
        
        // Listen for scene changes to handle references properly
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the phoneDot reference in the new scene if needed
        if (phoneDot == null)
        {
            // Try to find the reference by name or tag - customize this to your scene setup
            GameObject dotObj = GameObject.FindGameObjectWithTag("PhoneDot");
            if (dotObj != null)
            {
                phoneDot = dotObj.GetComponent<RectTransform>();
                
                if (isCalibrated && phoneDot != null)
                {
                    phoneDot.gameObject.SetActive(true);
                    phoneDot.anchoredPosition = targetPosition;
                }
            }
        }
    }

    void Update()
    {
        if (IMUReceiverManager.Instance == null)
            return;

        currentAcceleration = IMUReceiverManager.Instance.LatestAcceleration;

        if (!isCalibrated || phoneDot == null)
            return;

        float sensitivity = defaultSensitivity;
        if (SensitivityManager.Instance != null)
        {
            sensitivity = SensitivityManager.Instance.Sensitivity;
        }

        Vector3 relativeMovement = currentAcceleration - neutralAcceleration;

        Vector2 movement = new Vector2(
            -relativeMovement.x * movementScale / sensitivity,
            relativeMovement.y * movementScale / sensitivity
        );

        targetPosition = Vector2.Lerp(targetPosition, movement, Time.deltaTime * smoothing);
        
        // Check if phoneDot still exists before using it
        if (phoneDot != null)
        {
            phoneDot.anchoredPosition = targetPosition;
        }

        if (frameCount++ % 60 == 0)
        {
            Debug.Log($"[Calibration Scene] Tilt: {relativeMovement}, Sensitivity: {sensitivity}, Dot: {movement}");
        }
    }

    public void Calibrate()
    {
        if (IMUReceiverManager.Instance == null)
        {
            Debug.LogError("Cannot calibrate without IMUReceiverManager.");
            return;
        }

        neutralAcceleration = IMUReceiverManager.Instance.LatestAcceleration;
        isCalibrated = true;

        // Add null check before accessing phoneDot
        if (phoneDot != null)
        {
            phoneDot.gameObject.SetActive(true);
            phoneDot.anchoredPosition = Vector2.zero;
        }
        
        targetPosition = Vector2.zero;

        // Save calibration globally
        CalibrationSessionManager.Instance?.SetCalibration(neutralAcceleration);
        AccelerationDataManager.SaveCalibrationData(neutralAcceleration);

        Debug.Log($"✅ Calibration complete: {neutralAcceleration}");
    }

    public void GoToOptionsScene()
    {
        SceneManager.LoadScene("OptionScreen");
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
    void OnDestroy()
    {
        // Clean up the event subscription
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}