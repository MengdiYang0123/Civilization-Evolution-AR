/*
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Serialization;

namespace WPM {
      public partial class WorldMapGlobe : MonoBehaviour
      {

            // LANGUAGES_OPTIONS _setLanguage = LANGUAGES_OPTIONS.ENGLISH;

          public bool _setLanguage = true;
          /* public LANGUAGES_OPTIONS setLanguage
           {
                 get
                 {
                       return _setLanguage;
                 }
                 set
                 {
                       if (_setLanguage != LANGUAGES_OPTIONS.ENGLISH)
                       {
                             switch (_setLanguage)
                             {
                                   case LANGUAGES_OPTIONS.ITALIAN:
                                         ReadCategoriesPackedString("categories10_ita.txt");
                                         break;
                             } 
                             
                             //ReadCategoriesPackedString();
                             //Rileggo il file chiamo la funzione che legge il file
                             //e richiamo la DRAW con il la lingua nuova
                             
                       }
                 }
           }
      
          public int prevLanguage = 0;
          
          public bool setLanguage
          {
                get
                {
                      return _setLanguage;
                }
                set
                {
                      if (_setLanguage!= value)
                      {
                            _setLanguage = value;
                            switch (_setLanguage)
                            {
                                  case true:
                                        ReadCategoriesPackedString("categories10_ita");
                                        Redraw();
                                        break;
                                  case false:
                                        ReadCategoriesPackedString("categories10");
                                        Redraw();
                                        break;
                            }
                            
                      }
                }
          }
          
          public void ChangeLanguage(int language)
          {
                if (prevLanguage != language)
                {
                      switch (language)
                      {
                            case 0: //English
                                  ReadCategoriesPackedString("categories10");
                                  Redraw();
                                  break;
                            case 1: //Italian
                                  ReadCategoriesPackedString("categories10_ita");
                                  Redraw();
                                  break;
                      
                      }
                }
                
          }

      }

}
*/