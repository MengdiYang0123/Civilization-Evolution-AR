using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using System.Collections;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string nextSceneName = "GameScene"; // Ŀ�곡��
    public GameObject loadingIndicatorPrefab; // ����ָʾ��
    private IProgressIndicator progressIndicator; // ���� UI

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        var sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

        // �������� UI
        if (loadingIndicatorPrefab != null)
        {
            GameObject loadingUI = Instantiate(loadingIndicatorPrefab);
            progressIndicator = loadingUI.GetComponent<IProgressIndicator>();
            if (progressIndicator != null)
            {
                yield return progressIndicator.OpenAsync(); // ��ʾ���� UI
            }
        }

        // ʹ�� MRTK Scene System �����³���
        if (sceneSystem != null)
        {
            yield return sceneSystem.LoadContent(nextSceneName, LoadSceneMode.Single);
        }

        // �رս��� UI
        if (progressIndicator != null)
        {
            yield return progressIndicator.CloseAsync();
        }
    }
}

