using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;
using System.Collections;

public class SpatialMeshHandler : MonoBehaviour
{
    private IMixedRealitySpatialAwarenessMeshObserver meshObserver;

    void Start()
    {
        // 开始一个协程来禁用空间网格显示
        StartCoroutine(DisableSpatialMeshDisplayWithDelay());
    }

    IEnumerator DisableSpatialMeshDisplayWithDelay()
    {
        // 等待 2 秒以确保系统初始化完成
        yield return new WaitForSeconds(2.0f);

        // 获取空间感知网格观察者
        meshObserver = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

        // 确保观察者已初始化
        if (meshObserver != null)
        {
            Debug.Log("Disabling Spatial Mesh Display");
            meshObserver.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
        }
        else
        {
            Debug.Log("Mesh Observer Not Found");
        }

        // 定期检查并重新应用设置
        StartCoroutine(CheckAndReapplyDisplayOption());
    }

    IEnumerator CheckAndReapplyDisplayOption()
    {
        while (true)
        {
            if (meshObserver != null && meshObserver.DisplayOption != SpatialAwarenessMeshDisplayOptions.None)
            {
                Debug.Log("Reapplying Spatial Mesh Display Option: None");
                meshObserver.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
            }
            yield return new WaitForSeconds(5.0f); // 每 5 秒检查一次
        }
    }
}




