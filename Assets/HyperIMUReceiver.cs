using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HyperIMUReceiver : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform phoneDot;
    
    // Name to look for if the reference is lost
    public string phoneDotObjectName = "PhoneDot"; 

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
        // Find phoneDot reference if needed
        TryFindPhoneDot();
        
        if (phoneDot != null)
        {
            phoneDot.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("PhoneDot is not assigned! Visual feedback will not be available.");
        }

        if (IMUReceiverManager.Instance == null)
        {
            Debug.LogError("No IMUReceiverManager found in the scene! Please ensure it's added to your project.");
        }
        
        // Listen for scene changes to handle references properly
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void TryFindPhoneDot()
    {
        if (phoneDot == null)
        {
            // Try multiple approaches to find the phone dot
            
            // Method 1: Find by tag (if it exists)
            try 
            {
                GameObject dotObj = GameObject.FindWithTag("PhoneDot");
                if (dotObj != null)
                {
                    phoneDot = dotObj.GetComponent<RectTransform>();
                    return;
                }
            }
            catch (UnityException)
            {
                // Tag doesn't exist, continue to next approach
                Debug.Log("PhoneDot tag not defined, trying alternative methods");
            }
            
            // Method 2: Find by name
            GameObject namedObj = GameObject.Find(phoneDotObjectName);
            if (namedObj != null)
            {
                phoneDot = namedObj.GetComponent<RectTransform>();
                return;
            }
            
            // Method 3: Find any object with suitable component structure
            // This is a last resort approach
            RectTransform[] allRects = FindObjectsOfType<RectTransform>();
            foreach (RectTransform rect in allRects)
            {
                if (rect.name.Contains("Dot") || rect.name.Contains("Cursor"))
                {
                    phoneDot = rect;
                    Debug.Log($"Found potential phone dot: {rect.name}");
                    return;
                }
            }
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the phoneDot reference in the new scene if needed
        TryFindPhoneDot();
        
        // Only show the dot if we're already calibrated
        if (isCalibrated && phoneDot != null)
        {
            phoneDot.gameObject.SetActive(true);
            phoneDot.anchoredPosition = targetPosition;
        }
    }

    void Update()
    {
        if (IMUReceiverManager.Instance == null)
        {
            return;
        }

        currentAcceleration = IMUReceiverManager.Instance.LatestAcceleration;

        if (!isCalibrated)
        {
            return;
        }

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

        // Make sure we have a phoneDot reference
        TryFindPhoneDot();

        // Add null check before accessing phoneDot
        if (phoneDot != null)
        {
            phoneDot.gameObject.SetActive(true);
            phoneDot.anchoredPosition = Vector2.zero;
        }
        else
        {
            Debug.LogWarning("Calibration completed but phoneDot is missing - visual feedback will not be available");
        }

        targetPosition = Vector2.zero;

        // Save calibration globally
        if (CalibrationSessionManager.Instance != null)
        {
            CalibrationSessionManager.Instance.SetCalibration(neutralAcceleration);
        }

        AccelerationDataManager.SaveCalibrationData(neutralAcceleration);

        // Notify any calibration requirement monitors
        var requirement = CalibrationRequirement.Instance;
        if (requirement != null)
        {
            requirement.SetCalibrated(true);
        }

        // NEW: Update session calibration state
        if (SessionCalibrationHandler.Instance != null)
        {
            SessionCalibrationHandler.Instance.SetSessionCalibrated(true);
        }

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