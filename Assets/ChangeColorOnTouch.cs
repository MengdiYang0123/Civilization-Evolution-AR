using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class ChangeColorOnTouch : MonoBehaviour, IMixedRealityPointerHandler
{
    public Color highlightedColor = Color.red;
    private Renderer objectRenderer;
    private Color originalColor;
    private bool isTouching = false;
    public GameObject popupWindow;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {

        isTouching = true;
        SetHighlightColor();

        if (popupWindow != null)
        {
            popupWindow.SetActive(true);
        }
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        isTouching = false;
        ResetColor();
        popupWindow.SetActive(false);
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
    }

    private void SetHighlightColor()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = highlightedColor;
        }
    }

    private void ResetColor()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }
    }
}
