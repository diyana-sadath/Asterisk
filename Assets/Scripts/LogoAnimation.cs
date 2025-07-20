using UnityEngine;
using UnityEngine.UI;

public class LogoAnimation : MonoBehaviour
{
    public float glitchInterval = 2.0f;
    public float glitchDuration = 0.1f;
    public float letterSpacing = 0.2f;

    private Text logoText;
    private string originalText;
    private float nextGlitchTime;

    void Start()
    {
        logoText = GetComponent<Text>();
        originalText = logoText.text;
        nextGlitchTime = Time.time + Random.Range(1.0f, glitchInterval);

        // Create individual letter objects for more advanced animations
        CreateLetterObjects();
    }

    void Update()
    {
        // Simple glitch effect
        if (Time.time > nextGlitchTime)
        {
            StartCoroutine(GlitchEffect());
            nextGlitchTime = Time.time + Random.Range(glitchInterval * 0.5f, glitchInterval * 1.5f);
        }
    }

    System.Collections.IEnumerator GlitchEffect()
    {
        // Store original text
        string original = logoText.text;

        // Generate glitched version
        char[] glitchedChars = original.ToCharArray();
        int glitchCount = Random.Range(1, 3);

        for (int i = 0; i < glitchCount; i++)
        {
            int charIndex = Random.Range(0, glitchedChars.Length);
            if (glitchedChars[charIndex] != ' ')
                glitchedChars[charIndex] = (char)Random.Range(33, 126); // Random ASCII character
        }

        logoText.text = new string(glitchedChars);

        yield return new WaitForSeconds(glitchDuration);

        // Restore original
        logoText.text = original;
    }

    void CreateLetterObjects()
    {
        // Advanced implementation would create individual Text objects
        // for each letter to enable more complex animations
        // This is a simplified placeholder

        // For full implementation, you would:
        // 1. Create a child object for each letter
        // 2. Position them with proper spacing
        // 3. Apply individual animations to each
    }
}