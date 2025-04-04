using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace WPM {
      public partial class WorldMapGlobe : MonoBehaviour
      {

            // LANGUAGES_OPTIONS _setLanguage = LANGUAGES_OPTIONS.ENGLISH;
            
          
            public void ChangeFilter(string filter, bool value)
            {
                //string name = toggle.name;
                
                switch (filter)
                {
                    case "BATTLE":
                        battleLayer.SetActive(value);
                        break;
                    case "ART":
                        artLayer.SetActive(value);
                        break;
                    case "DISCOVERY":
                        discoveryLayer.SetActive(value);
                        break;
                    case "SCIENCE":
                        scienceLayer.SetActive(value);
                        break;
                    case "TREATY":
                        treatyLayer.SetActive(value);
                        break;
                    case "FUN FACT":
                        funFactLayer.SetActive(value);
                        break;
                    case "HUMAN RIGHTS":
                        humanRightsLayer.SetActive(value);
                        break;
                    case "TRAGEDY":
                        tragedyLayer.SetActive(value);
                        break;
                    case "EVENT":
                        eventLayer.SetActive(value);
                        break;
                }
            }


      }

}