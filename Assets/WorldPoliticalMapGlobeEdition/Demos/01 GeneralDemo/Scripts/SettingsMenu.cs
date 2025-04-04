using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace WPM
{
    

public partial class SettingsMenu : MonoBehaviour
{


    //[SerializeField] TMP_Dropdown dropdown;


    
    public GameObject settingsMenu;


    void Start()
    {
        //settingsMenu.SetActive(false);
    }
    private void Update()
    {
        

    }
    
    
    
    public void QuitApplication()
    {
        Debug.Log("Applicazione Chiusa");
        Application.Quit();
    }
    
/*
    void AddDropdownOptions(TMP_Dropdown dropdown, List<string> options)
    {

        dropdown.ClearOptions();
        Debug.Log("");
        //AGGIUNGO La scena attuale come prima opzione
        string activeScene = SceneManager.GetActiveScene().name;
        
        
        dropdown.options.Add(new TMP_Dropdown.OptionData(activeScene));

        for (int k = 0; k < options.Count; k++ )
        {
            if (activeScene!= options[k])
                dropdown.options.Add(new TMP_Dropdown.OptionData(options[k]));
        }
        dropdown.RefreshShownValue();
    }
    
*/
    /*
    public void changeDate()
    {
        string activeScene = SceneManager.GetActiveScene().name;
        string option = dateDrop.options[dateDrop.value].text;

            if (activeScene!= option)
            {
                switch (option)
                {
                    case "1801-1850":
                        SceneManager.LoadScene("1801-1850");
                        break;
                    case "1851-1900":
                        SceneManager.LoadScene("1851-1900");
                        break;
                    case "1901-1950":
                        SceneManager.LoadScene("1901-1950");
                        break;
                }
            }
    }
    */

}
}