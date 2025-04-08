using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider[] sliders;
    public Slider sensitivitySlider;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public int totalLevels = 5;

    // Optional references to audio managers
    public MonoBehaviour musicManager;
    public MonoBehaviour sfxManager;

    private int currentIndex = 0;
    private float[] levelValues;

    private readonly string[] sliderKeys = { "SensitivityLevel", "MusicLevel", "SoundLevel" };

    void Start()
    {
        Debug.Log("SliderController Start called");

        if (sliders == null || sliders.Length == 0)
        {
            Debug.LogError("Sliders array is null or empty!");
            return;
        }

        levelValues = new float[sliders.Length];
        LoadSavedValues();
        UpdateSliderVisuals();
        
        // Apply initial volumes to make sure they take effect
        if (levelValues.Length > 1)
            ApplyVolume(musicSource, levelValues[1]);

        if (levelValues.Length > 2)
            ApplyVolume(sfxSource, levelValues[2]);
            
        // Add listeners to sliders to update in real-time
        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i] != null)
            {
                int index = i; // Capture the index for the lambda
                sliders[i].onValueChanged.AddListener((value) => OnSliderValueChanged(index, value));
            }
        }
    }
    
    private void OnSliderValueChanged(int index, float value)
    {
        levelValues[index] = value;
        
        if (index == 0) ApplySensitivity(value);
        else if (index == 1) ApplyVolume(musicSource, value);
        else if (index == 2) ApplyVolume(sfxSource, value);
        
        PlayerPrefs.SetFloat(sliderKeys[index], value);
        PlayerPrefs.Save();
    }

    void LoadSavedValues()
    {
        Debug.Log("LoadSavedValues called");

        if (sliders == null)
        {
            Debug.LogError("Sliders array is null in LoadSavedValues!");
            return;
        }

        if (levelValues == null)
        {
            Debug.LogError("levelValues array is null in LoadSavedValues!");
            levelValues = new float[sliders.Length];
        }

        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i] == null)
            {
                Debug.LogError($"Slider at index {i} is null!");
                continue;
            }

            float savedLevel = PlayerPrefs.GetFloat(sliderKeys[i], totalLevels / 2);
            Debug.Log($"Loaded {sliderKeys[i]}: {savedLevel}");

            levelValues[i] = savedLevel;
            sliders[i].minValue = 1;
            sliders[i].maxValue = 5;
            sliders[i].wholeNumbers = true;
            sliders[i].value = savedLevel;
        }

        if (levelValues.Length > 0)
        {
            ApplySensitivity(levelValues[0]);

            if (levelValues.Length > 1)
                ApplyVolume(musicSource, levelValues[1]);

            if (levelValues.Length > 2)
                ApplyVolume(sfxSource, levelValues[2]);
        }
    }

    public void ApplySensitivity(float level)
    {
        float sensitivity = Mathf.Clamp(level, 1f, 5f);

        if (SensitivityManager.Instance != null)
        {
            SensitivityManager.Instance.SetSensitivity(sensitivity);
        }
        else
        {
            Debug.LogWarning("SensitivityManager.Instance is null! Creating one...");
            GameObject managerObj = new GameObject("SensitivityManager");
            SensitivityManager manager = managerObj.AddComponent<SensitivityManager>();
            DontDestroyOnLoad(managerObj);
            manager.SetSensitivity(sensitivity);
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = level;
        }
    }

    void ApplyVolume(AudioSource source, float level)
    {
        if (source != null)
        {
            float volume;

            switch ((int)level)
            {
                case 1: volume = 0f; break;
                case 2: volume = 0.25f; break;
                case 3: volume = 0.5f; break;
                case 4: volume = 0.75f; break;
                case 5: volume = 1.0f; break;
                default: volume = 0.5f; break;
            }

            source.volume = volume;
            Debug.Log($"Set {source.name} volume to {volume} based on level {level}");
            
            // Update music manager if available
            if (source == musicSource && musicManager != null)
            {
                // Try to call SetVolume through reflection to avoid direct dependency
                var method = musicManager.GetType().GetMethod("SetVolume");
                if (method != null)
                {
                    method.Invoke(musicManager, new object[] { volume });
                    Debug.Log($"Updated music manager volume to {volume}");
                }
            }
            // Update SFX manager if available
            else if (source == sfxSource && sfxManager != null)
            {
                // Try to call SetVolume through reflection to avoid direct dependency
                var method = sfxManager.GetType().GetMethod("SetVolume");
                if (method != null)
                {
                    method.Invoke(sfxManager, new object[] { volume });
                    Debug.Log($"Updated SFX manager volume to {volume}");
                }
            }
            
            // Handle global audio settings
            if (source == musicSource)
            {
                AudioListener.volume = volume; // This affects all audio in the scene
                Debug.Log($"Set global AudioListener volume to {volume}");
            }
        }
        else
        {
            Debug.LogWarning("AudioSource is null in ApplyVolume!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentIndex = (currentIndex + 1) % sliders.Length;
            UpdateSliderVisuals();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLevel(-1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLevel(1);
        }
    }

    void ChangeLevel(int delta)
    {
        if (levelValues == null || currentIndex >= levelValues.Length)
        {
            Debug.LogError("Invalid levelValues array in ChangeLevel!");
            return;
        }

        int currentLevel = (int)levelValues[currentIndex];
        int newLevel = Mathf.Clamp(currentLevel + delta, 1, 5);

        if (newLevel != currentLevel)
        {
            levelValues[currentIndex] = newLevel;

            if (sliders != null && currentIndex < sliders.Length && sliders[currentIndex] != null)
            {
                sliders[currentIndex].value = newLevel;
            }

            PlayerPrefs.SetFloat(sliderKeys[currentIndex], newLevel);
            Debug.Log($"Saving {sliderKeys[currentIndex]}: {newLevel}");
            PlayerPrefs.Save();

            if (currentIndex == 0) ApplySensitivity(newLevel);
            else if (currentIndex == 1)
            {
                ApplyVolume(musicSource, newLevel);
                if (GameSettingsManager.Instance != null)
                    GameSettingsManager.Instance.SaveMusicVolume(newLevel);
            }
            else if (currentIndex == 2)
            {
                ApplyVolume(sfxSource, newLevel);
                if (GameSettingsManager.Instance != null)
                    GameSettingsManager.Instance.SaveSoundVolume(newLevel);
            }
        }
    }

    void UpdateSliderVisuals()
    {
        if (sliders == null)
        {
            Debug.LogError("Sliders array is null in UpdateSliderVisuals!");
            return;
        }

        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i] == null)
            {
                Debug.LogWarning($"Slider at index {i} is null in UpdateSliderVisuals!");
                continue;
            }

            ColorBlock colors = sliders[i].colors;
            if (i == currentIndex)
            {
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.yellow;
            }
            else
            {
                colors.normalColor = Color.gray;
                colors.highlightedColor = Color.gray;
            }
            sliders[i].colors = colors;
        }
    }

    void OnEnable()
    {
        Debug.Log("SliderController OnEnable called");

        if (sliders != null && sliders.Length > 0)
        {
            if (levelValues == null || levelValues.Length != sliders.Length)
            {
                levelValues = new float[sliders.Length];
            }
            LoadSavedValues();
        }
        else
        {
            Debug.LogError("Sliders array is null or empty in OnEnable!");
        }
    }

    public void SaveSensitivityFromSlider()
    {
        float value = sensitivitySlider.value;
        ApplySensitivity(value);
        GameSettingsManager.Instance?.SaveSensitivity(value);
    }

    public void SaveMusicVolumeFromSlider()
    {
        if (sliders.Length > 1 && sliders[1] != null)
        {
            float value = sliders[1].value;
            ApplyVolume(musicSource, value);
            GameSettingsManager.Instance?.SaveMusicVolume(value);
            Debug.Log($"Saved music volume from button: {value}");
        }
    }

    public void SaveSoundVolumeFromSlider()
    {
        if (sliders.Length > 2 && sliders[2] != null)
        {
            float value = sliders[2].value;
            ApplyVolume(sfxSource, value);
            GameSettingsManager.Instance?.SaveSoundVolume(value);
            Debug.Log($"Saved sound volume from button: {value}");
        }
    }
    
    void OnDestroy()
    {
        // Remove listeners when the component is destroyed
        for (int i = 0; i < sliders.Length; i++)
        {
            if (sliders[i] != null)
            {
                sliders[i].onValueChanged.RemoveAllListeners();
            }
        }
    }
}