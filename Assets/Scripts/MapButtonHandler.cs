using System.Collections;
using UnityEngine;
using TMPro;

public class MapButtonHandler : MonoBehaviour
{
    [Header("Assign the TMP Text object here")]
    public TextMeshProUGUI comingSoonText;

    [Header("Fade Timing Settings")]
    public float fadeDuration = 0.5f;
    public float visibleDuration = 1f;

    public void OnMapButtonClicked()
    {
        if (comingSoonText == null)
        {
            Debug.LogError("ComingSoonText is not assigned in the inspector!");
            return;
        }

        StartCoroutine(FadeInAndOut(comingSoonText, fadeDuration, visibleDuration));
    }

    private IEnumerator FadeInAndOut(TextMeshProUGUI text, float fadeTime, float stayTime)
    {
        text.gameObject.SetActive(true);

        // Fade In
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float alpha = t / fadeTime;
            SetTextAlpha(text, alpha);
            yield return null;
        }
        SetTextAlpha(text, 1f);

        // Wait
        yield return new WaitForSeconds(stayTime);

        // Fade Out
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float alpha = 1f - (t / fadeTime);
            SetTextAlpha(text, alpha);
            yield return null;
        }
        SetTextAlpha(text, 0f);

        text.gameObject.SetActive(false);
    }

    private void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        if (text != null)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
    }
}
