using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    private static GameSettingsManager _instance;
    public static GameSettingsManager Instance { get { return _instance; } }

    // Store the current settings
    public float sensitivity;
    public float musicVolume;
    public float soundVolume;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Load values from PlayerPrefs
        sensitivity = PlayerPrefs.GetFloat("SensitivityLevel", 2);
        musicVolume = PlayerPrefs.GetFloat("MusicLevel", 2);
        soundVolume = PlayerPrefs.GetFloat("SoundLevel", 2);

        Debug.Log($"GameSettingsManager initialized with sensitivity: {sensitivity}");
    }

    public void SaveSensitivity(float value)
    {
        sensitivity = value;
        PlayerPrefs.SetFloat("SensitivityLevel", value);
        PlayerPrefs.Save();
        Debug.Log($"Saved sensitivity: {value}");
    }

    public void SaveMusicVolume(float value)
    {
        musicVolume = value;
        PlayerPrefs.SetFloat("MusicLevel", value);
        PlayerPrefs.Save();
    }

    public void SaveSoundVolume(float value)
    {
        soundVolume = value;
        PlayerPrefs.SetFloat("SoundLevel", value);
        PlayerPrefs.Save();
    }
}