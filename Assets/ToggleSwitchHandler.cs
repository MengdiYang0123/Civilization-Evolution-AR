using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.UI;

public class ToggleSwitchHandler : MonoBehaviour
{
    // 引用到 ToggleSwitch 对应的 Interactable 组件
    private Interactable toggleSwitchInteractable;

    // 创建一个 UnityEvent，用于值变化事件
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnValueChanged;

    private bool previousValue;

    void Start()
    {
        // 获取组件
        toggleSwitchInteractable = GetComponent<Interactable>();

        // 检查是否成功获取组件
        if (toggleSwitchInteractable == null)
        {
            Debug.LogError("Missing Interactable component.");
            return;
        }

        // 初始化 previousValue
        previousValue = toggleSwitchInteractable.IsToggled;

        // 绑定 OnClick 事件
        toggleSwitchInteractable.OnClick.AddListener(OnToggleSwitchClicked);
    }

    void OnDestroy()
    {
        // 移除事件绑定
        if (toggleSwitchInteractable != null)
        {
            toggleSwitchInteractable.OnClick.RemoveListener(OnToggleSwitchClicked);
        }
    }

    // ToggleSwitch 点击事件处理
    private void OnToggleSwitchClicked()
    {
        bool isToggledOn = toggleSwitchInteractable.IsToggled;
        Debug.Log($"ToggleSwitch value changed! IsToggled: {isToggledOn}");
        // 检查值是否真的发生了变化
        if (isToggledOn != previousValue)
        {
            previousValue = isToggledOn;
            OnValueChanged.Invoke(isToggledOn);
        }
    }
}


