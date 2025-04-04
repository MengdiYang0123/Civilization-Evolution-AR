using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using System.Collections;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string nextSceneName = "GameScene"; // 目标场景
    public GameObject loadingIndicatorPrefab; // 进度指示器
    private IProgressIndicator progressIndicator; // 进度 UI

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        var sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

        // 创建进度 UI
        if (loadingIndicatorPrefab != null)
        {
            GameObject loadingUI = Instantiate(loadingIndicatorPrefab);
            progressIndicator = loadingUI.GetComponent<IProgressIndicator>();
            if (progressIndicator != null)
            {
                yield return progressIndicator.OpenAsync(); // 显示进度 UI
            }
        }

        // 使用 MRTK Scene System 加载新场景
        if (sceneSystem != null)
        {
            yield return sceneSystem.LoadContent(nextSceneName, LoadSceneMode.Single);
        }

        // 关闭进度 UI
        if (progressIndicator != null)
        {
            yield return progressIndicator.CloseAsync();
        }
    }
}

