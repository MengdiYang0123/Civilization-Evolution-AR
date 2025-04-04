using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;
using System.Collections;

public class SpatialMeshHandler : MonoBehaviour
{
    private IMixedRealitySpatialAwarenessMeshObserver meshObserver;

    void Start()
    {
        // ��ʼһ��Э�������ÿռ�������ʾ
        StartCoroutine(DisableSpatialMeshDisplayWithDelay());
    }

    IEnumerator DisableSpatialMeshDisplayWithDelay()
    {
        // �ȴ� 2 ����ȷ��ϵͳ��ʼ�����
        yield return new WaitForSeconds(2.0f);

        // ��ȡ�ռ��֪����۲���
        meshObserver = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

        // ȷ���۲����ѳ�ʼ��
        if (meshObserver != null)
        {
            Debug.Log("Disabling Spatial Mesh Display");
            meshObserver.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
        }
        else
        {
            Debug.Log("Mesh Observer Not Found");
        }

        // ���ڼ�鲢����Ӧ������
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
            yield return new WaitForSeconds(5.0f); // ÿ 5 ����һ��
        }
    }
}




