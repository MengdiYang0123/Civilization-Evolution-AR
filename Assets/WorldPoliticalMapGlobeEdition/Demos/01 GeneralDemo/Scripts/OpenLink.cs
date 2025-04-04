using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace WPM
{ 
     public class OpenLink : MonoBehaviour
     {
          public string linkString;
          public void OpenOnBrowser(Text categoryLink)
          {
               linkString = categoryLink.text;
               
               //Debug.Log("Link String -> " + linkString);
               
               Application.OpenURL(linkString);
               
          }

     }
    
}
    
