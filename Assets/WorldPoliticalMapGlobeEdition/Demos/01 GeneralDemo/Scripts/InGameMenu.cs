using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace WPM
{
    
//DA SPOSTARE TUTTO IN DEMO -> OVVERO "GAMEPLAYMANAGER"
public partial class InGameMenu : MonoBehaviour
{
    
    WorldMapGlobe map;
    
    
    public GameObject inGameMenuPrefab;

    

    void Awake()
    {
        //DontDestroyOnLoad(inGameMenuPrefab);
    }

    void Start()
       {
           Instantiate(inGameMenuPrefab);
           inGameMenuPrefab.gameObject.SetActive(false);
           map = WorldMapGlobe.instance;
       }
    
}
    
}