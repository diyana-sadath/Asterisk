using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // Original button size
    private Vector3 originalScale;

    // Scale factors
    [Range(1.0f, 1.5f)]
    public float hoverScaleFactor = 1.1f;

    [Range(0.8f, 1.0f)]
    public float clickScaleFactor = 0.95f;

    // Color settings
    public bool changeColor = true;
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.9f, 0.9f, 1.0f);
    public Color clickColor = new Color(0.8f, 0.8f, 0.9f);

    // Components
    private Image buttonImage;

    void Start()
    {
        // Store the original scale
        originalScale = transform.localScale;

        // Get button image component
        buttonImage = GetComponent<Image>();

        // Set normal color
        if (buttonImage != null && changeColor)
        {
            buttonImage.color = normalColor;
        }
    }

    // Mouse hover enter
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScaleFactor;

        if (buttonImage != null && changeColor)
        {
            buttonImage.color = hoverColor;
        }
    }

    // Mouse hover exit
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;

        if (buttonImage != null && changeColor)
        {
            buttonImage.color = normalColor;
        }
    }

    // Mouse down
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * clickScaleFactor;

        if (buttonImage != null && changeColor)
        {
            buttonImage.color = clickColor;
        }
    }

    // Mouse up
    public void OnPointerUp(PointerEventData eventData)
    {
        // Check if we're still hovering
        if (eventData.position.x >= buttonImage.rectTransform.position.x - buttonImage.rectTransform.rect.width / 2 &&
            eventData.position.x <= buttonImage.rectTransform.position.x + buttonImage.rectTransform.rect.width / 2 &&
            eventData.position.y >= buttonImage.rectTransform.position.y - buttonImage.rectTransform.rect.height / 2 &&
            eventData.position.y <= buttonImage.rectTransform.position.y + buttonImage.rectTransform.rect.height / 2)
        {
            transform.localScale = originalScale * hoverScaleFactor;

            if (buttonImage != null && changeColor)
            {
                buttonImage.color = hoverColor;
            }
        }
        else
        {
            transform.localScale = originalScale;

            if (buttonImage != null && changeColor)
            {
                buttonImage.color = normalColor;
            }
        }
    }
}
