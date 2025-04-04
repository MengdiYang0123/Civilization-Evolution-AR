using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WPM {

    public enum LANGUAGES_OPTIONS
    {
        ENGLISH = 0,
        ITALIAN = 1
    }

    public class Language : MonoBehaviour
        {

           // public string name;
            public LANGUAGES_OPTIONS languagesOptions;
            public bool isSet;


        public Language(string name, LANGUAGES_OPTIONS languagesOptions)
        {
            this.name = name;
            this.languagesOptions = languagesOptions;
        }
   
    }
}