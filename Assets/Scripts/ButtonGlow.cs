using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlowingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color normalColor = new Color(0.2f, 0.2f, 0.3f);
    public Color hoverColor = new Color(0.4f, 0.4f, 0.8f);
    public float glowIntensity = 1.5f;
    public float animationSpeed = 3.0f;

    private Image buttonImage;
    private Text buttonText;
    private Outline glowEffect;
    private bool isHovering = false;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>();

        // Add outline component for glow if it doesn't exist
        glowEffect = buttonText.GetComponent<Outline>();
        if (glowEffect == null)
            glowEffect = buttonText.gameObject.AddComponent<Outline>();

        glowEffect.effectColor = hoverColor;
        glowEffect.effectDistance = new Vector2(1, 1);
        glowEffect.enabled = false;

        buttonImage.color = normalColor;
    }

    void Update()
    {
        if (isHovering)
        {
            // Pulsing glow effect
            float glow = 0.7f + Mathf.PingPong(Time.time * animationSpeed, 0.6f);
            glowEffect.effectDistance = new Vector2(glow, glow);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        buttonImage.color = hoverColor;
        glowEffect.enabled = true;

        // Optional scale effect
        transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        buttonImage.color = normalColor;
        glowEffect.enabled = false;

        transform.localScale = Vector3.one;
    }
}