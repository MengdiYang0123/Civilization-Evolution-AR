using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Change_text : MonoBehaviour
{
    public TMP_Text alliesText;

    public TMP_Text enemiesText;
    
    static Change_text _instance;

    public static Change_text instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<Change_text> ();
                if (_instance == null) {
                    Debug.LogWarning ("'Change_text' GameObject could not be found in the scene. Make sure it's created with this name before using any map functionality.");
                }
            }
            return _instance;
        }
    }

    void Start()
    {
        alliesText.text = "";
        enemiesText.text = "";
    }

    public void textChanger1()
    {
        alliesText.text = "Allied Powers";
        enemiesText.text = "Central Powers";
    }
        
    public void textChanger2()
    {
        alliesText.text = "Allies";
        enemiesText.text = "Axes";
    }
}


