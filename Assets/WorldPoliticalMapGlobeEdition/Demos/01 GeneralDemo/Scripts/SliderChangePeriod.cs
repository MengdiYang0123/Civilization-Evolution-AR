using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;


namespace WPM
{


    public class SliderChangePeriod : MonoBehaviour
    {

        WorldMapGlobe map;
        Text textComponent;
        public Slider slider;
        string activeScene;
        public ProgressIndicatorOrbsRotator progressIndicator;//新
        private ISceneTransitionService sceneTransitionService;//新


        private List<string> options = new List<string>() { "1901-1950", "1951-2000", "2001-2021" };

        // Start is called before the first frame update

        private void Awake()
        {
            //DontDestroyOnLoad(slider);
        }

        void Start()
        {
            map = WorldMapGlobe.instance;
            textComponent = GetComponentInChildren<Text>(slider);
            activeScene = SceneManager.GetActiveScene().name;
            //Debug.Log("activeScene = " + activeScene);
            SetSliderValue(slider);

            sceneTransitionService = MixedRealityToolkit.Instance.GetService<ISceneTransitionService>();
            if (sceneTransitionService == null)
            {
                Debug.LogError("SceneTransitionService is NULL! Make sure it is properly registered.");
            }


            if (MixedRealityToolkit.Instance == null)
            {
                Debug.Log("SceneTransitionService is NULL! 请检查 MRTK 配置！");
            }
            else
            {
                    sceneTransitionService = MixedRealityToolkit.Instance.GetService<ISceneTransitionService>();
                if (sceneTransitionService == null)
                {
                    Debug.LogError("SceneTransitionService is NULL! Make sure it is properly registered.");
                }
                else
                {
                    Debug.Log("获取成功");
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetSliderValue(Slider slider)
        {

            for (int k = 0; k < options.Count || k < (int)slider.maxValue; k++)
            {
                if (activeScene == options[k])
                {
                    slider.value = k;
                    textComponent.text = activeScene;
                }
            }

        }

        public void changeDate(Slider slider)
        {
            int numericSliderValue = (int)slider.value;

            if (activeScene != options[numericSliderValue])
            {
                switch (options[numericSliderValue])
                {
                    case "1901-1950":
                        textComponent.text = options[numericSliderValue];
                        //SceneManager.LoadSceneAsync("1901-1950");
                        LoadScene("1901-1950");
                        break;
                    case "1951-2000":
                        textComponent.text = options[numericSliderValue];
                        //SceneManager.LoadSceneAsync("1951-2000");
                        LoadScene("1951-2000");
                        break;
                    case "2001-2021":
                        textComponent.text = options[numericSliderValue];
                        //SceneManager.LoadSceneAsync("2001-2021");
                        LoadScene("2001-2021");
                        break;

                }
            }
        }
        /*public void changeDate(Slider slider)
        {
            int numericSliderValue = (int)slider.value;
            if (activeScene != options[numericSliderValue])
            {
                textComponent.text = options[numericSliderValue];
                LoadScene(options[numericSliderValue]);
            }
        }*/


        public void OnSliderValueChanged(float value)
        {
            changeDate(slider);
        }

        //new

        /*public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }*/


        public void LoadScene(string sceneName)
        {
            if (sceneTransitionService.TransitionInProgress)
            {
                Debug.Log("Scene transition already in progress...");
                return;
            }
            StartCoroutine(LoadSceneWithTransition(sceneName));

            //await sceneTransitionService.DoSceneTransition(() => LoadSceneAsync(sceneName));

            //StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (progressIndicator != null)
            {
                Debug.Log("Opening progress indicator...");
                var task = progressIndicator.OpenAsync();
                while (!task.IsCompleted) { yield return null; }
            }
            yield return new WaitForSeconds(0.5f);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
            {
                if (progressIndicator != null)
                {
                    progressIndicator.Progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);

            if (progressIndicator != null)
            {
                Debug.Log("Closing progress indicator...");
                var task = progressIndicator.CloseAsync();
                while (!task.IsCompleted) { yield return null; }
            }

            activeScene = sceneName;
            Debug.Log("Scene Loaded: " + sceneName);
        }

        /*private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (progressIndicator != null)
            {
                Debug.Log("Opening progress indicator...");
                var task = progressIndicator.OpenAsync();
                while (!task.IsCompleted) { yield return null; }
            }
            yield return new WaitForSeconds(0.5f);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                if (progressIndicator != null)
                {
                    progressIndicator.Progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);

            Scene newScene = SceneManager.GetSceneByName(sceneName);
            if (newScene.IsValid())
            {
                SceneManager.SetActiveScene(newScene);
            }

            Scene oldScene = SceneManager.GetActiveScene();
            if (oldScene.name != sceneName)
            {
                yield return SceneManager.UnloadSceneAsync(oldScene);
            }

            if (progressIndicator != null)
            {
                Debug.Log("Closing progress indicator...");
                var task = progressIndicator.CloseAsync();
                while (!task.IsCompleted) { yield return null; }
            }

            activeScene = sceneName;
            Debug.Log("Scene Loaded: " + sceneName);
        }*/

        private IEnumerator LoadSceneWithTransition(string sceneName)
        {
            // 1. 启动进度指示器，确保它可见
            if (progressIndicator != null)
            {
                Debug.Log("Opening progress indicator...");
                var task = progressIndicator.OpenAsync();
                while (!task.IsCompleted) { yield return null; }
            }
            yield return new WaitForSeconds(0.5f);

            // 2. 进行场景转换
            yield return sceneTransitionService.DoSceneTransition(
                async () =>
                {
                    Debug.Log("Loading Scene: " + sceneName);
                    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
                    while (!asyncLoad.isDone)
                    {
                        if (progressIndicator != null)
                        {
                            progressIndicator.Progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                        }
                        await Task.Yield();
                    }
                }
            );
            yield return new WaitForSeconds(0.5f);

            // 3. 关闭进度指示器
            if (progressIndicator != null)
            {
                Debug.Log("Closing progress indicator...");
                var task = progressIndicator.CloseAsync();
                while (!task.IsCompleted) { yield return null; }
            }

            // 4. 更新 activeScene
            activeScene = sceneName;
            Debug.Log("Scene Loaded: " + sceneName);
        }


       

        



    }

}
