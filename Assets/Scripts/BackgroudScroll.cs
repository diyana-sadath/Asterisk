using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [Header("Background Settings")]
    public RawImage backgroundImage;  // Assign your Raw Image from Canvas
    public float scrollSpeed = 0.1f;  // Adjust speed as needed
    private bool isPaused = false;

    private void Start()
    {
        // Initialize if not set in inspector
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<RawImage>();
        }
    }

    private void Update()
    {
        // Check if game is paused or component is valid
        if (isPaused || backgroundImage == null) return;

        // Scroll texture UV vertically 
        backgroundImage.uvRect = new Rect(
            backgroundImage.uvRect.x,
            backgroundImage.uvRect.y + scrollSpeed * Time.deltaTime,
            backgroundImage.uvRect.width,
            backgroundImage.uvRect.height
        );
    }

    public void PauseScroll()
    {
        isPaused = true;
    }

    public void ResumeScroll()
    {
        isPaused = false;
    }

    private void OnDisable()
    {
        // Make sure we don't leave things in a bad state when disabled
        isPaused = false;
    }

    private void OnEnable()
    {
        // Resume when re-enabled
        isPaused = false;
    }
}