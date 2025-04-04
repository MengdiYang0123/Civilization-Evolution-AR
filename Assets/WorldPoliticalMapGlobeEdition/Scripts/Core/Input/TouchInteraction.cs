using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using UnityEngine;

public class TouchInteraction : MonoBehaviour, IMixedRealityPointerHandler
{
    public GameObject quadObject;
    public TextMeshPro textDisplay;

    private void Start()
    {
        textDisplay.gameObject.SetActive(false);
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (eventData.InputSource.Pointers[0].Result.CurrentPointerTarget == quadObject)
        {
            // �������¼�������Quad��ʱ��ʾ����
            textDisplay.gameObject.SetActive(true);

            Debug.Log("Image Touched!");

            // ʾ�������ı���ʾ�������ʾ��Ϣ
            textDisplay.text = "Hello, HoloLens!";
        }
    }

    #region Unused Interface Methods

    public void OnPointerClicked(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    #endregion
}

