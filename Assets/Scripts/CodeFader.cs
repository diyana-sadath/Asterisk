using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Place this script on a ROOT GameObject that contains your fade panel
public class CodeOnlyFader : MonoBehaviour
{
    // The panel GameObject instead of direct Image reference
    public GameObject fadePanel;

    private Image fadePanelImage;

    public float fadeSpeed = 1.5f;
    public Color fadeColor = Color.black;
    private string sceneToLoad;

    private bool isTransitioning = false;

    // Static instance for singleton pattern with public access
    public static CodeOnlyFader Instance { get; private set; }

    void Awake()
    {
        // Singleton pattern to ensure only one fader exists
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroying duplicate CodeOnlyFader");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // THIS OBJECT MUST BE A ROOT OBJECT
        DontDestroyOnLoad(gameObject);

        if (fadePanel == null)
        {
            Debug.LogError("Fade Panel GameObject reference is missing! Please assign it in the inspector.");
            return;
        }

        // Get the Image component from the fade panel
        fadePanelImage = fadePanel.GetComponent<Image>();
        if (fadePanelImage == null)
        {
            Debug.LogError("Fade Panel must contain an Image component!");
            return;
        }

        // Ensure the panel covers the entire screen
        RectTransform rt = fadePanelImage.rectTransform;
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
        }

        // Initially transparent and inactive
        fadePanelImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        fadePanel.SetActive(false);

        // Set canvas to screen space overlay and high sort order
        Canvas canvas = fadePanel.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Make sure it's the highest sorting order
        }
    }

    void OnEnable()
    {
        // Register for scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unregister from event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Public method to call from button clicks
    public void FadeToScene(string sceneName)
    {
        Debug.Log("FadeToScene called for: " + sceneName);
        if (isTransitioning)
        {
            Debug.Log("Already transitioning, ignoring request");
            return;
        }

        sceneToLoad = sceneName;
        isTransitioning = true;

        fadePanel.SetActive(true);
        fadePanel.transform.SetAsLastSibling(); // Make sure it's on top

        StartCoroutine(FadeOutAndLoadScene());
    }

    IEnumerator FadeOutAndLoadScene()
    {
        Debug.Log("Starting fade out to scene: " + sceneToLoad);

        // Make sure we start from fully transparent
        fadePanelImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);

        // Fade to black
        float alpha = 0f;
        while (alpha < 1.0f)
        {
            alpha += Time.unscaledDeltaTime * fadeSpeed;
            fadePanelImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, Mathf.Clamp01(alpha));
            yield return null;
        }

        // Ensure we're fully black
        fadePanelImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);

        // Critical fix: Hold at black for a frame to prevent flashing
        yield return new WaitForEndOfFrame();

        Debug.Log("Fade complete, loading scene: " + sceneToLoad);

        // Begin asynchronous loading but PREVENT automatic activation
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        // Wait until the scene is 90% loaded
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Scene is loaded but not activated yet - we're still at black screen
        // Now activate the scene
        asyncLoad.allowSceneActivation = true;

        // Wait until activation is complete
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Scene loaded, beginning fade in");

        // Scene is now fully loaded and active, but we're still faded to black
        // Add a very small delay to ensure everything is initialized
        yield return new WaitForSeconds(0.05f);

        // Start fading in
        StartCoroutine(FadeIn());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);

        // Make sure panel is on top in the new scene
        if (fadePanel != null)
        {
            fadePanel.transform.SetAsLastSibling();

            // Ensure Canvas settings are still correct
            Canvas canvas = fadePanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;
            }
        }
    }

    IEnumerator FadeIn()
    {
        // Ensure panel is active and visible
        fadePanel.SetActive(true);
        fadePanelImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);

        // Fade from black to transparent
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.unscaledDeltaTime * fadeSpeed;
            fadePanelImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, Mathf.Clamp01(alpha));
            yield return null;
        }

        // Ensure we're fully transparent
        fadePanelImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        fadePanel.SetActive(false);

        isTransitioning = false;
        Debug.Log("Fade in complete, transition finished");
    }
}