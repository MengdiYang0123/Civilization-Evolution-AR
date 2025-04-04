using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Toggle = UnityEngine.UIElements.Toggle;

namespace WPM {
      
      
      public partial class Language_FilterCat : MonoBehaviour
      { 
            
          //public LANGUAGES_OPTIONS _setLanguage = LANGUAGES_OPTIONS.ENGLISH;
          
          public int prevLanguage = 0;

          WorldMapGlobe map;

          private void Start()
          {
                map = WorldMapGlobe.instance;
          }

          /*
          public bool showBattles {
                get {
                      return _showBattles;
                }
                set {
                      if (_showBattles != value) {
                            _showBattles = value;
                            isDirty = true;
                            if (battleLayer != null) {
                                  battleLayer.SetActive(_showBattles);
                            } else if (_showCategories) {
                                  battleLayer.SetActive(_showBattles);
                            }
                      }

                }
          }
          public bool showArts {
                get {
                      return _showArts;
                }
                set {
                      if (_showArts != value) {
                            _showArts = value;
                            isDirty = true;
                            if (artLayer != null) {
                                  artLayer.SetActive(_showArts);
                            } else if (_showCategories) {
                                  artLayer.SetActive(_showArts);
                            }
                      }

                }
          }
          

          
          public void FilterBattle(bool value) 
          {
                if(battleLayer!=null)
                  battleLayer.SetActive(value);
          }
          public void FilterArt(bool value) 
          {
                if(artLayer!=null)
                   artLayer.SetActive(value);
          }
          public void FilterDiscovery(bool value) 
          {
                if(discoveryLayer!=null)
                   discoveryLayer.SetActive(value);
          }
          public void FilterScience(bool value) 
          {
                if(scienceLayer!=null)
                  scienceLayer.SetActive(value);
          }
          public void FilterTreaty(bool value) 
          {
                if(treatyLayer!=null)
                  treatyLayer.SetActive(value);
          }
          
          */

          public void ChangeLanguage(int language)
          {
                if (prevLanguage != language)
                {
                      prevLanguage = language;
                      map._setLanguage = (LANGUAGES_OPTIONS) language;
                      map.ReloadData(map._setLanguage);
                      /*
                      switch (language)
                      {
                            case 0: //English
                                  map.ReloadData(map._setLanguage);
                                  //map.ReadCategoriesPackedString(SceneManager.GetActiveScene().name,LANGUAGES_OPTIONS.ENGLISH);
                                  Debug.Log("Ok prima leggo la lingua ");
                                  //ReadCategoriesPackedString(SceneManager.GetActiveScene().name, LANGUAGES_OPTIONS.ENGLISH);
                                  //map.Redraw(); //Chiamando la redraw posso riscrivere anche i nomi di città e stati
                                  break;
                            case 1: //Italian
                                  Debug.Log("-> Leggo l'italiano");
                                  //map.ReadCategoriesPackedString(SceneManager.GetActiveScene().name,
                                        LANGUAGES_OPTIONS.ITALIAN);
                                  //map.Redraw(); //chiamo la redraw? così lo faccio su tutto
                                  break;
                      }
                      */
                }

          }
          


          
      }

}