using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.UI;

public class ToggleSwitchHandler : MonoBehaviour
{
    // ���õ� ToggleSwitch ��Ӧ�� Interactable ���
    private Interactable toggleSwitchInteractable;

    // ����һ�� UnityEvent������ֵ�仯�¼�
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnValueChanged;

    private bool previousValue;

    void Start()
    {
        // ��ȡ���
        toggleSwitchInteractable = GetComponent<Interactable>();

        // ����Ƿ�ɹ���ȡ���
        if (toggleSwitchInteractable == null)
        {
            Debug.LogError("Missing Interactable component.");
            return;
        }

        // ��ʼ�� previousValue
        previousValue = toggleSwitchInteractable.IsToggled;

        // �� OnClick �¼�
        toggleSwitchInteractable.OnClick.AddListener(OnToggleSwitchClicked);
    }

    void OnDestroy()
    {
        // �Ƴ��¼���
        if (toggleSwitchInteractable != null)
        {
            toggleSwitchInteractable.OnClick.RemoveListener(OnToggleSwitchClicked);
        }
    }

    // ToggleSwitch ����¼�����
    private void OnToggleSwitchClicked()
    {
        bool isToggledOn = toggleSwitchInteractable.IsToggled;
        Debug.Log($"ToggleSwitch value changed! IsToggled: {isToggledOn}");
        // ���ֵ�Ƿ���ķ����˱仯
        if (isToggledOn != previousValue)
        {
            previousValue = isToggledOn;
            OnValueChanged.Invoke(isToggledOn);
        }
    }
}


