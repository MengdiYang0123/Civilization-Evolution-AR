using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.Events;
using WPM;
using static ToggleSwitchHandler;


namespace WPM
{


    public class FilterMenuToggles : MonoBehaviour
    {
        WorldMapGlobe map;
        string activeScene;
        Change_text text;


        //private Text text;
        //Toggle t;
        // Start is called before the first frame update

        void Start()
        {
            map = WorldMapGlobe.instance;  // PERCHé devo copiarlo in tutte????
            activeScene = SceneManager.GetActiveScene().name;
            text = Change_text.instance;
            //text.gameObject.SetActive(false);
        }




        public void ShowFrontiers(bool value)
        {
            map = WorldMapGlobe.instance;
            map.showFrontiers = value;
        }

        public void ShowCountryNames(bool value)
        {
            map = WorldMapGlobe.instance;
            map.showCountryNames = value;
        }

        public void ColorizeEUButton()
        {

            if (activeScene.Equals("1951-2000"))
            {
                map.HideCountrySurfaces();
                map = WorldMapGlobe.instance;
                map.FlyToCountry("Italy");
                for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
                {
                    if (map.countries[colorizeIndex].continent.Equals("EU"))
                    {
                        Color color = new Color(0, 0, 1);
                        map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                    }
                }
            }
            else
            {
                map.HideCountrySurfaces();
                map = WorldMapGlobe.instance;
                map.FlyToCountry("Italy");
                for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
                {
                    if (map.countries[colorizeIndex].continent.Equals("Europe"))
                    {
                        Color color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
                        map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                    }
                }
            }
        }
        //UnityEngine.Random.Range(0.0f, 1.0f)

        public void ColorizeFranceButton()
        {
            map = WorldMapGlobe.instance;
            map.FlyToCity("Paris");
            for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
            {
                if (map.countries[colorizeIndex].domain.Equals("France"))
                {
                    Color color = new Color(0, 0, 255);
                    map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                }
            }
        }

        public void ColorizeUkButton()
        {
            map = WorldMapGlobe.instance;
            map.FlyToCity("Paris");
            for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
            {
                if (map.countries[colorizeIndex].domain.Equals("France"))
                {
                    Color color = new Color(0, 0, 255);
                    map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                }
            }
        }
        public void HandleInputData(int index)
        {

            switch (index)
            {
                case 0:
                    map.HideCountrySurfaces();
                    break;
                case 1:
                    map.HideCountrySurfaces();
                    map = WorldMapGlobe.instance;
                    map.FlyToCountry("France");
                    for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
                    {
                        if (map.countries[colorizeIndex].domain.Equals("France"))
                        {
                            Color color = new Color(0, (float)0.45, (float)0.73);
                            map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                        }
                    }

                    break;

                case 2:
                    map.HideCountrySurfaces();
                    map = WorldMapGlobe.instance;
                    map.FlyToCountry("United Kingdom");
                    for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
                    {
                        if (map.countries[colorizeIndex].domain.Equals("Uk"))
                        {
                            Color color = new Color((float)0.784, (float)0.063, (float)0.18);
                            map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                        }
                    }

                    break;
                case 3:
                    map.HideCountrySurfaces();
                    map = WorldMapGlobe.instance;
                    map.FlyToCountry("Spain");
                    for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
                    {
                        if (map.countries[colorizeIndex].domain.Equals("Spain"))
                        {
                            Color color = new Color((float)0.902, 0, (float)0.15);
                            map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                        }
                    }

                    break;
                case 4:
                    map.HideCountrySurfaces();
                    map = WorldMapGlobe.instance;
                    map.FlyToCountry("Portugal");
                    for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
                    {
                        if (map.countries[colorizeIndex].domain.Equals("Portugal"))
                        {
                            Color color = new Color(1, 0, 0);
                            map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                        }
                    }

                    break;
                case 5:
                    map.HideCountrySurfaces();
                    map = WorldMapGlobe.instance;
                    map.FlyToCountry("Belgium");
                    for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
                    {
                        if (map.countries[colorizeIndex].domain.Equals("Belgium"))
                        {
                            Color color = new Color(1, (float)0.804, 0);
                            map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                        }
                    }

                    break;
                case 6:
                    map.HideCountrySurfaces();
                    map = WorldMapGlobe.instance;
                    map.FlyToCountry("Germany Empire");
                    for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
                    {
                        if (map.countries[colorizeIndex].domain.Equals("Germany"))
                        {
                            Color color = new Color(0, 0, 0);
                            map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                        }
                    }

                    break;
                case 7:
                    map.HideCountrySurfaces();
                    map = WorldMapGlobe.instance;
                    map.FlyToCountry("Italy");
                    for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
                    {
                        if (map.countries[colorizeIndex].domain.Equals("Italy"))
                        {
                            Color color = new Color(0, (float)0.55, (float)0.27);
                            map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                        }
                    }
                    break;
            }
        }

        public void FlyToRandomCountry()
        {
            int countryIndex = UnityEngine.Random.Range(0, map.countries.Length);

            Color color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
            map.ToggleCountrySurface(countryIndex, true, color);
            map.FlyToCountry(countryIndex);

        }

        public void ResetCountries()
        {
            map.HideCountrySurfaces();
        }

        public void ToggleCategory(Interactable toggle)
        {
            bool isToggledOn = toggle.IsToggled;
            //onValueChangedEvent.Invoke(isToggledOn);
            map = WorldMapGlobe.instance;

            switch (toggle.name)
            {
                case "BATTLE":
                    map.showBattles = isToggledOn;
                    break;
                case "ART":
                    map.showArts = isToggledOn;
                    break;
                case "DISCOVERY":
                    map.showDiscoveries = isToggledOn;
                    break;
                case "SCIENCE":
                    map.showSciences = isToggledOn;
                    break;
                case "TREATY":
                    map.showTreaties = isToggledOn;
                    break;
                case "FUN FACT":
                    map.showFunFact = isToggledOn;
                    break;
                case "HUMAN RIGHTS":
                    map.showHumanRights = isToggledOn;
                    break;
                case "TRAGEDY":
                    map.showTragedy = isToggledOn;
                    break;
                case "EVENT":
                    map.showEvent = isToggledOn;
                    break;
            }
        }

        public void TogglesCity(Interactable toggle)
        {
            bool isToggledOn = toggle.IsToggled;
            map = WorldMapGlobe.instance;


            switch (toggle.name)
            {

                case "countryCapitals":
                    map.showCountryCapitals = isToggledOn;
                    break;
                case "regionCapitals":
                    map.showRegionCapitals = isToggledOn;
                    break;
                case "normalCities":
                    map.showNormalCities = isToggledOn;
                    break;
            }

        }

        public void TogglesCategories(Toggle t)
        {
            map = WorldMapGlobe.instance;

            switch (t.name)
            {

                case "BATTLE":
                    map.showBattles = t.isOn;
                    break;
                case "ART":
                    map.showArts = t.isOn;
                    break;
                case "DISCOVERY":
                    map.showDiscoveries = t.isOn;
                    break;
                case "SCIENCE":
                    map.showSciences = t.isOn;
                    break;
                case "TREATY":
                    map.showTreaties = t.isOn;
                    break;
                case "FUN FACT":
                    map.showFunFact = t.isOn;
                    break;
                case "HUMAN RIGHTS":
                    map.showHumanRights = t.isOn;
                    break;
                case "TRAGEDY":
                    map.showTragedy = t.isOn;
                    break;
                case "EVENT":
                    map.showEvent = t.isOn;
                    break;
            }

        }


        public void ShowCities(bool value)
        {
            map = WorldMapGlobe.instance;
            map.showCities = value;
        }

        /*public void TogglesCities(Toggle t, bool value)
        {
            map = WorldMapGlobe.instance;
            
            switch (t.name)
            {
                
                case "countryCapitals":
                    map.showCountryCapitals = value;
                    break;
                case "regionCapitals":
                    map.showRegionCapitals = value;
                    break;
                case "normalCities":
                    map.showNormalCities = value;
                    break;
            }

        }*/

        public void TogglesCities(Toggle t)
        {
            map = WorldMapGlobe.instance;

            switch (t.name)
            {

                case "countryCapitals":
                    map.showCountryCapitals = t.isOn;
                    break;
                case "regionCapitals":
                    map.showRegionCapitals = t.isOn;
                    break;
                case "normalCities":
                    map.showNormalCities = t.isOn;
                    break;
            }

        }

        //Script per evidenziare gli schieramenti della prima guerra mondiale
        public void ColorizeWW1()
        {
            map.HideCountrySurfaces();
            map = WorldMapGlobe.instance;
            map.FlyToCountry("Italy");
            text.textChanger1();
            //text.gameObject.SetActive(true);
            for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
            {
                if (map.countries[colorizeIndex].name.Equals("France") || map.countries[colorizeIndex].name.Equals("United Kingdom") || map.countries[colorizeIndex].name.Equals("Russian Empire")
                    || map.countries[colorizeIndex].name.Equals("Japanese Empire") || map.countries[colorizeIndex].name.Equals("Italy") || map.countries[colorizeIndex].name.Equals("United States of America"))
                {
                    Color color = new Color(0, 0, 255);
                    map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                }
                else if (map.countries[colorizeIndex].name.Equals("German Empire") || map.countries[colorizeIndex].name.Equals("Austria-Hungary") || map.countries[colorizeIndex].name.Equals("Ottoman Empire") || map.countries[colorizeIndex].name.Equals("Bulgaria"))
                {
                    Color color = new Color(255, 0, 0);
                    map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                }
            }
        }

        public void ColorizeWW2()
        {
            map.HideCountrySurfaces();
            map = WorldMapGlobe.instance;
            map.FlyToCountry("Italy");
            text.textChanger2();
            for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++)
            {
                if (map.countries[colorizeIndex].name.Equals("France") || map.countries[colorizeIndex].name.Equals("United Kingdom") || map.countries[colorizeIndex].name.Equals("Russian Empire") || map.countries[colorizeIndex].name.Equals("United States of America"))
                {
                    Color color = new Color(0, 0, 255);
                    map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                }
                else if (map.countries[colorizeIndex].name.Equals("German Empire") || map.countries[colorizeIndex].name.Equals("Italy") || map.countries[colorizeIndex].name.Equals("Japanese Empire"))
                {
                    Color color = new Color(255, 0, 0);
                    map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                }
            }
        }

        public void ProvaTagButton(bool value)
        {
            map = WorldMapGlobe.instance;
            //map.FlyToCity("Paris");
            /*for (int tagIndex = 0; tagIndex < map.categories.Count; tagIndex++) {
                if (map.categories[tagIndex].tag.Equals("WW1"))
                {
                    map.showCategories = value;
                }
            }*/
            map.showCategories = value;
        }


    }

}