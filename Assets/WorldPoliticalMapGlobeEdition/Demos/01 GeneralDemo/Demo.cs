﻿#define LIGHTSPEED

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using TMPro;

namespace WPM
{

    public class Demo : MonoBehaviour
    {

        //public GameObject rocketPrefab;

        WorldMapGlobe map;
        GUIStyle labelStyle, labelStyleShadow, buttonStyle, sliderStyle, sliderThumbStyle;
        ColorPicker colorPicker;
        bool changingFrontiersColor;
        bool minimizeState = false;
        bool animatingField;


        public GameObject pauseMenu;
        //GameObject currentPauseMenu;

        public GameObject inGameMenu;
        //GameObject currentInGameMenu;


        //float zoomLevel = 1.0f;

        public GameObject TextObject;  //新
        public TextMeshPro textMeshPro; //新

        private void Awake()
        {

            //DontDestroyOnLoad(prefabInGameMenu);
        }

        void Start()
        {
            //Cursor.SetCursor(null, hotSpot, cursorMode);

            map = WorldMapGlobe.instance;
            map.FlyToCountry("Italy");
            map.showNormalCities = false;
            map.showRegionCapitals = false;

            //新
            GameObject parentObject = GameObject.Find("EventDescription/UGUIScrollViewContent/Scroll View");
            //cam = map.mainCamera;
            TextObject = new GameObject("MyTextObject");
            TextObject.transform.parent = parentObject.transform;
            textMeshPro = TextObject.AddComponent<TextMeshPro>();

            if (textMeshPro != null)
            {
                // 设置 TextMeshPro 的属性
                textMeshPro.fontSize = 280;
                textMeshPro.color = Color.white;
                textMeshPro.alignment = TextAlignmentOptions.Center;
                textMeshPro.enableWordWrapping = true;
                textMeshPro.fontStyle = FontStyles.Bold;
                textMeshPro.text = "Hello, World!";

                RectTransform rectTransform = TextObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(750, 200);
                rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
                rectTransform.localPosition = new Vector3(0, 40, 0);
                rectTransform.localScale = parentObject.transform.localScale * 1.0f;

                TextObject.layer = LayerMask.NameToLayer("UI");

                //TextObject.transform.localPosition = new Vector3(7, 225, -105);
                //TextObject.transform.localScale = parentObject.transform.localScale * 1.0f;
                //float distanceFromCamera = 40.0f;
                //SetDistanceFromCamera(distanceFromCamera);
            }
            else
            {
                Debug.LogError("TextMeshPro is not assigned!");
            }



            //Instatiate In Game Menu prefab and setActive
            //currentInGameMenu = Instantiate(inGameMenuPrefab);
            //currentInGameMenu.SetActive(true);

            //Instatiate Settings Menu prefab and Hide

            //currentPauseMenu = Instantiate(pauseMenu);
            //currentPauseMenu.SetActive(false);


            pauseMenu.SetActive(false);
            inGameMenu.SetActive(false);




#if LIGHTSPEED
            Camera.main.fieldOfView = 150;
            animatingField = true;
#endif
            map.earthInvertedMode = false;


            // UI Setup - non-important, only for this demo


            labelStyle = new GUIStyle();
            labelStyle.alignment = TextAnchor.LowerCenter;
            labelStyle.normal.textColor = Color.white;
            labelStyleShadow = new GUIStyle(labelStyle);
            labelStyleShadow.normal.textColor = Color.black;
            buttonStyle = new GUIStyle(labelStyle);
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            buttonStyle.normal.background = Texture2D.whiteTexture;
            buttonStyle.normal.textColor = Color.white;
            colorPicker = gameObject.GetComponent<ColorPicker>();
            sliderStyle = new GUIStyle();
            sliderStyle.normal.background = Texture2D.whiteTexture;
            sliderStyle.fixedHeight = 4.0f;
            sliderThumbStyle = new GUIStyle();
            sliderThumbStyle.normal.background = Resources.Load<Texture2D>("thumb");
            sliderThumbStyle.overflow = new RectOffset(0, 0, 8, 0);
            sliderThumbStyle.fixedWidth = 20.0f;
            sliderThumbStyle.fixedHeight = 12.0f;

            // setup GUI resizer - only for the demo
            GUIResizer.Init(800, 500);

            // Some example commands below
            //			map.ToggleCountrySurface("Brazil", true, Color.green);
            //			map.ToggleCountrySurface(35, true, Color.green);
            //			map.ToggleCountrySurface(33, true, Color.green);
            //			map.FlyToCountry(33);
            //			map.FlyToCountry("Brazil");
            //			map.navigationTime = 0; // jump instantly to next country
            //			map.FlyToCountry ("India");

            // Register events: this is optionally but allows your scripts to be informed instantly as the mouse enters or exits a country, province or city */

            map.OnCityEnter += (int cityIndex) => Debug.Log("Entered city " + map.cities[cityIndex].name);
            map.OnCityExit += (int cityIndex) => Debug.Log("Exited city " + map.cities[cityIndex].name);
            map.OnCityPointerDown += (int cityIndex) => Debug.Log("Pointer down on city " + map.cities[cityIndex].name);
            map.OnCityClick += (int cityIndex) => Debug.Log("Clicked city " + map.cities[cityIndex].name);
            map.OnCityPointerUp += (int cityIndex) => Debug.Log("Pointer up on city " + map.cities[cityIndex].name);

            //MODIFICA DEMO EVENTS
            // map.OnCategoryEnter += (int categoryIndex) => Debug.Log("Entered Event " + map.categories[categoryIndex].name);
            //map.OnCategoryExit += (int categoryIndex) => Debug.Log("Exited Event " + map.categories[categoryIndex].name);
            //map.OnCategoryPointerDown += (int categoryIndex) => Debug.Log("Pointer down on event " + map.categories[categoryIndex].name);
            //map.OnCategoryClick += (int categoryIndex) => Debug.Log("Clicked event " + map.categories[categoryIndex].name);
            //map.OnCategoryPointerUp += (int categoryIndex) => Debug.Log("Pointer up on event " + map.categories[categoryIndex].name);

            /*
                        map.OnCountryEnter += (int countryIndex, int regionIndex) => Debug.Log("Entered country (" + countryIndex + ") " + map.countries[countryIndex].name);
                        map.OnCountryExit += (int countryIndex, int r1024egionIndex) => Debug.Log("Exited country " + map.countries[countryIndex].name);
                        map.OnCountryPointerDown += (int countryIndex, int regionIndex) => Debug.Log("Pointer down on country " + map.countries[countryIndex].name);
                        map.OnCountryClick += (int countryIndex, int regionIndex) => Debug.Log("Clicked country " + map.countries[countryIndex].name);
                        map.OnCountryPointerUp += (int countryIndex, int regionIndex) => Debug.Log("Pointer up on country " + map.countries[countryIndex].name);

                        map.OnProvinceEnter += (int provinceIndex, int regionIndex) => Debug.Log("Entered province (" + provinceIndex + ") " + map.provinces[provinceIndex].name);
                        map.OnProvinceExit += (int provinceIndex, int regionIndex) => Debug.Log("Exited province " + map.provinces[provinceIndex].name);
                        map.OnProvincePointerDown += (int provinceIndex, int regionIndex) => Debug.Log("Pointer down on province " + map.provinces[provinceIndex].name);
                        map.OnProvinceClick += (int provinceIndex, int regionIndex) => Debug.Log("Clicked province " + map.provinces[provinceIndex].name);
                        map.OnProvincePointerUp += (int provinceIndex, int regionIndex) => Debug.Log("Pointer up on province " + map.provinces[provinceIndex].name);
            */


            /*
             map.OnClick += (sphereLocation, mouseButtonIndex) => {
                Vector2 latLon = Conversion.GetLatLonFromSpherePoint(sphereLocation);
                Debug.Log("Clicked on Latitude: " + latLon.x + ", Longitude: " + latLon.y);
            };
            */
            //da rivedere
            map.OnCategoryEnter += (int categoryIndex) =>
            {
                switch (map.categories[categoryIndex].categoryClass)
                {
                    case CATEGORY_CLASS.BATTLE:
                        if (WorldMapGlobe.instance.showBattles)
                        {
                            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                        }

                        break;
                    case CATEGORY_CLASS.ART:
                        if (WorldMapGlobe.instance.showArts)
                        {
                            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                        }
                        break;
                    case CATEGORY_CLASS.SCIENCE:
                        if (WorldMapGlobe.instance.showSciences)
                        {
                            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                        }
                        break;
                    case CATEGORY_CLASS.TREATY:
                        if (WorldMapGlobe.instance.showTreaties)
                        {
                            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                        }
                        break;
                    case CATEGORY_CLASS.DISCOVERY:
                        if (WorldMapGlobe.instance.showDiscoveries)
                        {
                            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                        }
                        break;

                    case CATEGORY_CLASS.FUNFACT:
                        if (WorldMapGlobe.instance.showFunFact)
                        {
                            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                        }
                        break;

                    case CATEGORY_CLASS.HUMANRIGHTS:
                        if (WorldMapGlobe.instance.showHumanRights)
                        {
                            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                        }

                        break;

                    case CATEGORY_CLASS.TRAGEDY:
                        if (WorldMapGlobe.instance.showTragedy)
                        {
                            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                        }
                        break;

                    case CATEGORY_CLASS.EVENT:
                        if (WorldMapGlobe.instance.showEvent)
                        {
                            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                        }
                        break;
                    default:
                        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                        break;
                }
                //Debug.Log("Entered Event " + map.categories[categoryIndex].name + "  " +map.categories[categoryIndex].categoryClass);
                //Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            };
            map.OnCategoryExit += (int categoryIndex) =>
            {
                switch (map.categories[categoryIndex].categoryClass)
                {
                    case CATEGORY_CLASS.BATTLE:
                        if (WorldMapGlobe.instance.showBattles)
                        {
                            Cursor.SetCursor(null, hotSpot, cursorMode);
                        }

                        break;
                    case CATEGORY_CLASS.ART:
                        if (WorldMapGlobe.instance.showArts)
                        {
                            Cursor.SetCursor(null, hotSpot, cursorMode);
                        }
                        break;
                    case CATEGORY_CLASS.SCIENCE:
                        if (WorldMapGlobe.instance.showSciences)
                        {
                            Cursor.SetCursor(null, hotSpot, cursorMode);
                        }
                        break;
                    case CATEGORY_CLASS.TREATY:
                        if (WorldMapGlobe.instance.showTreaties)
                        {
                            Cursor.SetCursor(null, hotSpot, cursorMode);
                        }
                        break;
                    case CATEGORY_CLASS.DISCOVERY:
                        if (WorldMapGlobe.instance.showDiscoveries)
                        {
                            Cursor.SetCursor(null, hotSpot, cursorMode);
                        }
                        break;

                    case CATEGORY_CLASS.FUNFACT:
                        if (WorldMapGlobe.instance.showFunFact)
                        {
                            Cursor.SetCursor(null, hotSpot, cursorMode);
                        }
                        break;

                    case CATEGORY_CLASS.HUMANRIGHTS:
                        if (WorldMapGlobe.instance.showHumanRights)
                        {
                            Cursor.SetCursor(null, hotSpot, cursorMode);
                        }

                        break;

                    case CATEGORY_CLASS.TRAGEDY:
                        if (WorldMapGlobe.instance.showTragedy)
                        {
                            Cursor.SetCursor(null, hotSpot, cursorMode);
                        }
                        break;

                    case CATEGORY_CLASS.EVENT:
                        if (WorldMapGlobe.instance.showEvent)
                        {
                            Cursor.SetCursor(null, hotSpot, cursorMode);
                        }
                        break;
                    default:
                        Cursor.SetCursor(null, hotSpot, cursorMode);
                        break;
                }
                //Debug.Log("Exit Event " + map.categories[categoryIndex].name);

                //Cursor.SetCursor(null, hotSpot, cursorMode);
            };

            map.OnCategoryClick += (int categoryIndex) =>
            {
                Category category = map.GetCategory(categoryIndex);
                Application.OpenURL(category.categoryLink);
            };


        }



        bool avoidGUI;
        GameObject button;

        public Texture2D cursorTexture;
        public CursorMode cursorMode = CursorMode.Auto;
        public Texture2D normalTexture;
        public Vector2 hotSpot = Vector2.zero;


        // press SPACE during gameplay to hide GUI
        void OnGUI()
        {

            if (avoidGUI)
                return;

            // Do autoresizing of GUI layer
            GUIResizer.AutoResize();


            // Check whether a country or city or category event is selected, then show a label
            if (map.mouseIsOver)
            {


                string text = "";
                string stringToEdit = "";

                Vector3 mousePos = Input.mousePosition;
                float x, y;
                x = GUIResizer.authoredScreenWidth * (mousePos.x / Screen.width) - 100;
                y = GUIResizer.authoredScreenHeight - (GUIResizer.authoredScreenHeight * (mousePos.y / Screen.height)) - 120 * (Input.touchSupported ? 3 : 1); // slightly up for touch devices
                //to modify to hide the descripion of a city or an event if the toggle is off
                if (map.countryHighlighted != null && map.cityHighlighted != null || map.provinceHighlighted != null || map.categoryHighlighted != null)
                {
                    City city = map.cityHighlighted;
                    Category category = map.categoryHighlighted;
                    //da rivedere
                    if (city != null)
                    {
                        switch (city.cityClass)
                        {
                            case CITY_CLASS.COUNTRY_CAPITAL:
                                if (WorldMapGlobe.instance.showCountryCapitals)
                                {
                                    if (city.province != null && city.province.Length > 0)
                                    {
                                        text = "City: " + map.cityHighlighted.name + " (" + city.province + ", " +
                                               map.countries[map.cityHighlighted.countryIndex].name + ")";
                                    }
                                    else
                                    {
                                        text = "City: " + map.cityHighlighted.name + " (" +
                                               map.countries[map.cityHighlighted.countryIndex].name + ")";
                                    }
                                    textMeshPro.text = text;//新
                                }

                                break;
                            case CITY_CLASS.REGION_CAPITAL:
                                if (WorldMapGlobe.instance.showRegionCapitals)
                                {
                                    if (city.province != null && city.province.Length > 0)
                                    {
                                        text = "City: " + map.cityHighlighted.name + " (" + city.province + ", " +
                                               map.countries[map.cityHighlighted.countryIndex].name + ")";
                                    }
                                    else
                                    {
                                        text = "City: " + map.cityHighlighted.name + " (" +
                                               map.countries[map.cityHighlighted.countryIndex].name + ")";
                                    }
                                    textMeshPro.text = text;//新
                                }

                                break;
                            case CITY_CLASS.CITY:
                                if (WorldMapGlobe.instance.showNormalCities)
                                {
                                    if (city.province != null && city.province.Length > 0)
                                    {
                                        text = "City: " + map.cityHighlighted.name + " (" + city.province + ", " +
                                               map.countries[map.cityHighlighted.countryIndex].name + ")";
                                    }
                                    else
                                    {
                                        text = "City: " + map.cityHighlighted.name + " (" +
                                               map.countries[map.cityHighlighted.countryIndex].name + ")";
                                    }
                                    textMeshPro.text = text;//新
                                }

                                break;
                        }

                        /*if (city.province != null && city.province.Length > 0) {
                            text = "City: " + map.cityHighlighted.name + " (" + city.province + ", " + map.countries[map.cityHighlighted.countryIndex].name + ")";
                        } else {
                            text = "City: " + map.cityHighlighted.name + " (" + map.countries[map.cityHighlighted.countryIndex].name + ")";
                        }*/
                    } //CATEGORIA da rivedere
                    else if (category != null)
                    {
                        switch (category.categoryClass)
                        {
                            case CATEGORY_CLASS.BATTLE:
                                if (WorldMapGlobe.instance.showBattles)
                                {
                                    if (category.description != null || !string.Equals(category.description, "") || category.year != -1)
                                    {
                                        //mettere un if se c'è il link
                                        stringToEdit = "Event: " + map.categoryHighlighted.name + ";\nYear: " + map.categoryHighlighted.year + ";\nDescription: " + map.categoryHighlighted.description; //+ ";\nLink: " + map.categoryHighlighted.categoryLink;
                                        //stringToEdit = GUI.TextArea(new Rect(x, y, 250, 150), stringToEdit, 300);
                                        textMeshPro.text = stringToEdit;//新
                                    }
                                    else
                                    {
                                        stringToEdit = "";
                                    }
                                }

                                break;
                            case CATEGORY_CLASS.ART:
                                if (WorldMapGlobe.instance.showArts)
                                {
                                    if (category.description != null || !string.Equals(category.description, "") || category.year != -1)
                                    {
                                        //mettere un if se c'è il link
                                        stringToEdit = "Event: " + map.categoryHighlighted.name + ";\nYear: " + map.categoryHighlighted.year + ";\nDescription: " + map.categoryHighlighted.description; //+ ";\nLink: " + map.categoryHighlighted.categoryLink;
                                        //stringToEdit = GUI.TextArea(new Rect(x, y, 250, 150), stringToEdit, 300);
                                        textMeshPro.text = stringToEdit;//新
                                    }
                                    else
                                    {
                                        stringToEdit = "";
                                    }
                                }
                                break;
                            case CATEGORY_CLASS.SCIENCE:
                                if (WorldMapGlobe.instance.showSciences)
                                {
                                    if (category.description != null || !string.Equals(category.description, "") || category.year != -1)
                                    {
                                        //mettere un if se c'è il link
                                        stringToEdit = "Event: " + map.categoryHighlighted.name + ";\nYear: " + map.categoryHighlighted.year + ";\nDescription: " + map.categoryHighlighted.description; //+ ";\nLink: " + map.categoryHighlighted.categoryLink;
                                        //stringToEdit = GUI.TextArea(new Rect(x, y, 250, 150), stringToEdit, 300);
                                        textMeshPro.text = stringToEdit;//新
                                    }
                                    else
                                    {
                                        stringToEdit = "";
                                    }
                                }
                                break;
                            case CATEGORY_CLASS.TREATY:
                                if (WorldMapGlobe.instance.showTreaties)
                                {
                                    if (category.description != null || !string.Equals(category.description, "") || category.year != -1)
                                    {
                                        //mettere un if se c'è il link
                                        stringToEdit = "Event: " + map.categoryHighlighted.name + ";\nYear: " + map.categoryHighlighted.year + ";\nDescription: " + map.categoryHighlighted.description; //+ ";\nLink: " + map.categoryHighlighted.categoryLink;
                                        //stringToEdit = GUI.TextArea(new Rect(x, y, 250, 150), stringToEdit, 300);
                                        textMeshPro.text = stringToEdit;//新
                                    }
                                    else
                                    {
                                        stringToEdit = "";
                                    }
                                }
                                break;
                            case CATEGORY_CLASS.DISCOVERY:
                                if (WorldMapGlobe.instance.showDiscoveries)
                                {
                                    if (category.description != null || !string.Equals(category.description, "") || category.year != -1)
                                    {
                                        //mettere un if se c'è il link
                                        stringToEdit = "Event: " + map.categoryHighlighted.name + ";\nYear: " + map.categoryHighlighted.year + ";\nDescription: " + map.categoryHighlighted.description; //+ ";\nLink: " + map.categoryHighlighted.categoryLink;
                                        //stringToEdit = GUI.TextArea(new Rect(x, y, 250, 150), stringToEdit, 300);
                                        textMeshPro.text = stringToEdit;//新
                                    }
                                    else
                                    {
                                        stringToEdit = "";
                                    }
                                }
                                break;

                            case CATEGORY_CLASS.FUNFACT:
                                if (WorldMapGlobe.instance.showFunFact)
                                {
                                    if (category.description != null || !string.Equals(category.description, "") || category.year != -1)
                                    {
                                        //mettere un if se c'è il link
                                        stringToEdit = "Event: " + map.categoryHighlighted.name + ";\nYear: " + map.categoryHighlighted.year + ";\nDescription: " + map.categoryHighlighted.description; //+ ";\nLink: " + map.categoryHighlighted.categoryLink;
                                        //stringToEdit = GUI.TextArea(new Rect(x, y, 250, 150), stringToEdit, 300);
                                        textMeshPro.text = stringToEdit;//新
                                    }
                                    else
                                    {
                                        stringToEdit = "";
                                    }
                                }
                                break;

                            case CATEGORY_CLASS.HUMANRIGHTS:
                                if (WorldMapGlobe.instance.showHumanRights)
                                {
                                    if (category.description != null || !string.Equals(category.description, "") || category.year != -1)
                                    {
                                        //mettere un if se c'è il link
                                        stringToEdit = "Event: " + map.categoryHighlighted.name + ";\nYear: " + map.categoryHighlighted.year + ";\nDescription: " + map.categoryHighlighted.description; //+ ";\nLink: " + map.categoryHighlighted.categoryLink;
                                        //stringToEdit = GUI.TextArea(new Rect(x, y, 250, 150), stringToEdit, 300);
                                        textMeshPro.text = stringToEdit;//新
                                    }
                                    else
                                    {
                                        stringToEdit = "";
                                    }
                                }

                                break;

                            case CATEGORY_CLASS.TRAGEDY:
                                if (WorldMapGlobe.instance.showTragedy)
                                {
                                    if (category.description != null || !string.Equals(category.description, "") || category.year != -1)
                                    {
                                        //mettere un if se c'è il link
                                        stringToEdit = "Event: " + map.categoryHighlighted.name + ";\nYear: " + map.categoryHighlighted.year + ";\nDescription: " + map.categoryHighlighted.description; //+ ";\nLink: " + map.categoryHighlighted.categoryLink;
                                        //stringToEdit = GUI.TextArea(new Rect(x, y, 250, 150), stringToEdit, 300);
                                        textMeshPro.text = stringToEdit;//新
                                    }
                                    else
                                    {
                                        stringToEdit = "";
                                    }
                                }
                                break;

                            case CATEGORY_CLASS.EVENT:
                                if (WorldMapGlobe.instance.showEvent)
                                {
                                    if (category.description != null || !string.Equals(category.description, "") || category.year != -1)
                                    {
                                        //mettere un if se c'è il link
                                        stringToEdit = "Event: " + map.categoryHighlighted.name + ";\nYear: " + map.categoryHighlighted.year + ";\nDescription: " + map.categoryHighlighted.description; //+ ";\nLink: " + map.categoryHighlighted.categoryLink;
                                        //stringToEdit = GUI.TextArea(new Rect(x, y, 250, 150), stringToEdit, 300);
                                        textMeshPro.text = stringToEdit;//新
                                    }
                                    else
                                    {
                                        stringToEdit = "";
                                    }
                                }
                                break;
                        }


                        // Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                        /*if (category.description != null || !string.Equals(category.description, "") || category.year!=-1) {
                            //mettere un if se c'è il link
                            stringToEdit = "Event: " + map.categoryHighlighted.name + ";\nYear: " + map.categoryHighlighted.year + ";\nDescription: " + map.categoryHighlighted.description; //+ ";\nLink: " + map.categoryHighlighted.categoryLink;
                            stringToEdit = GUI.TextArea(new Rect(x, y, 250, 150), stringToEdit, 300);
                        } else
                        {
                            stringToEdit = "";
                        }
                        */


                    }
                    else if (map.provinceHighlighted != null)
                    {
                        text = map.provinceHighlighted.name + ", " + map.countryHighlighted.name;
                        List<Province> neighbours = map.ProvinceNeighboursOfCurrentRegion();
                        if (neighbours.Count > 0)
                            text += "\n" + EntityListToString<Province>(neighbours);
                            //textMeshPro.text = text;
                    }
                    else if (map.countryHighlighted != null)
                    {
                        text = map.countryHighlighted.name + " (" + map.countryHighlighted.continent + ")";
                        List<Country> neighbours = map.CountryNeighboursOfCurrentRegion();
                        if (neighbours.Count > 0)
                            text += "\n" + EntityListToString<Country>(neighbours);
                            //textMeshPro.text = text;
                    }
                    else
                    {
                        text = "";
                    }

                    GUI.Label(new Rect(x - 1, y - 1, 0, 10), text, labelStyleShadow);
                    GUI.Label(new Rect(x + 1, y + 2, 0, 10), text, labelStyleShadow);
                    GUI.Label(new Rect(x + 2, y + 3, 0, 10), text, labelStyleShadow);
                    GUI.Label(new Rect(x + 3, y + 4, 0, 10), text, labelStyleShadow);
                    GUI.Label(new Rect(x, y, 0, 10), text, labelStyle);

                    /*GUI.Label(new Rect(x - 1, y - 1, 0, 10), stringToEdit, labelStyleShadow);
                    GUI.Label(new Rect(x + 1, y + 2, 0, 10), stringToEdit, labelStyleShadow);
                    GUI.Label(new Rect(x + 2, y + 3, 0, 10), stringToEdit, labelStyleShadow);
                    GUI.Label(new Rect(x + 3, y + 4, 0, 10), stringToEdit, labelStyleShadow);
                    GUI.Label(new Rect(x, y, 0, 10), stringToEdit, labelStyle);
                    */

                }
                text = map.calc.prettyCurrentLatLon;
                x = GUIResizer.authoredScreenWidth / 2.0f;
                y = GUIResizer.authoredScreenHeight - 20;
                GUI.Label(new Rect(x, y, 0, 10), text, labelStyle);
            }




            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.3f, 0.5f);



            //QUI FACCIO IL CHECK DELLA LINGUA?

            //Imposto un dropdown menu la lungua andrebbe dentro la classe map.?? in modo da cambiare tutto

            /*if (GUI.Button(new Rect(10, 170, 160, 30), "  switch Italian", buttonStyle)) {
                map.setLanguage = LANGUAGES_OPTIONS.ITALIAN;
            }*/

            // Add button to toggle Earth texture
            /*if (GUI.Button(new Rect(10, 170, 160, 30), "  Change Earth style", buttonStyle)) {
                map.earthStyle = (EARTH_STYLE)(((int)map.earthStyle + 1) % 10);
            }*/

            // Add buttons to show the color picker and change colors for the frontiers or fill
            /*if (GUI.Button(new Rect(10, 210, 160, 30), "  Change Frontiers Color", buttonStyle)) {
                colorPicker.showPicker = true;
                changingFrontiersColor = true;
            }*/

            /*if (GUI.Button(new Rect(10, 210, 160, 30), "  Change To English", buttonStyle)) {
                map.setLanguage = LANGUAGES_OPTIONS.ENGLISH;
            }
            
            
            if (GUI.Button(new Rect(10, 250, 160, 30), "  Change Fill Color", buttonStyle)) {
                colorPicker.showPicker = true;
                changingFrontiersColor = false;
            }
            if (colorPicker.showPicker) {
                if (changingFrontiersColor) {
                    map.frontiersColor = colorPicker.setColor;
                } else {
                    map.fillColor = colorPicker.setColor;
                }
            }

            // Add a button which demonstrates the navigateTo functionality -- pass the name of a country
            // For a list of countries and their names, check map.Countries collection.
            if (GUI.Button(new Rect(10, 290, 180, 30), "  Fly to Australia (Country)", buttonStyle)) {
                FlyToCountry("Australia");
            }
            if (GUI.Button(new Rect(10, 325, 180, 30), "  Fly to Mexico (Country)", buttonStyle)) {
                FlyToCountry("Mexico");
            }
            if (GUI.Button(new Rect(10, 360, 180, 30), "  Fly to New York (City)", buttonStyle)) {
                FlyToCity("New York");
            }
            if (GUI.Button(new Rect(10, 395, 180, 30), "  Fly to Madrid (City)", buttonStyle)) {
                FlyToCity("Madrid");
            }
*/
            /* Slider to show the new set zoom level API in V4.1
            GUI.Button(new Rect(10, 430, 85, 30), "  Zoom Level", buttonStyle);
            float prevZoomLevel = zoomLevel;
            GUI.backgroundColor = Color.white;
            zoomLevel = GUI.HorizontalSlider(new Rect(100, 445, 80, 85), zoomLevel, 0, 1, sliderStyle, sliderThumbStyle);
            GUI.backgroundColor = new Color(0.1f, 0.1f, 0.3f, 0.95f);
            if (zoomLevel != prevZoomLevel) {
                map.SetZoomLevel(zoomLevel);
            }
            */


            // Add a button to colorize countries
            /*if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth - 190, 20, 180, 30), "  Colorize Europe", buttonStyle)) {
                map.FlyToCity("Brussels");
                for (int colorizeIndex = 0; colorizeIndex < map.countries.Length; colorizeIndex++) {
                    if (map.countries[colorizeIndex].continent.Equals("Europe")) {
                        Color color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
                        map.ToggleCountrySurface(map.countries[colorizeIndex].name, true, color);
                    }
                }
            }*/
            /*
                        // Colorize random country and fly to it
                        if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth - 190, 60, 180, 30), "  Colorize Random", buttonStyle)) {
                            map.FlyToCity("Brussels");
                            int countryIndex = UnityEngine.Random.Range(0, map.countries.Length);
                            Color color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
                            map.ToggleCountrySurface(countryIndex, true, color);
                            map.FlyToCountry(countryIndex);
                        }

            */


            /*map.OnCategoryClick += (int categoryIndex) =>
            
            {

                Vector3 mousePos = Input.mousePosition;
                float x, y;
                string text;
                
                Category category = map.categoryHighlighted;
                if (category != null)
                {
                    if (category.description != null || !string.Equals(category.description, "") || category.year!=-1) { 
                        text = "Event: " + map.categoryHighlighted.name + ";\nYear: " + map.categoryHighlighted.year + ";\nDescription: " + map.categoryHighlighted.description ;

                    } else {
                        text = "Event: " + map.categoryHighlighted.name;
                    }
                    x = GUIResizer.authoredScreenWidth * (mousePos.x / Screen.width);
                    y = GUIResizer.authoredScreenHeight - (GUIResizer.authoredScreenHeight * (mousePos.y / Screen.height)) - 20 * (Input.touchSupported ? 3 : 1); // slightly up for touch devices
                    GUI.Label(new Rect(x - 1, y - 1, 0, 10), text, labelStyleShadow);
                    GUI.Label(new Rect(x + 1, y + 2, 0, 10), text, labelStyleShadow);
                    GUI.Label(new Rect(x + 2, y + 3, 0, 10), text, labelStyleShadow);
                    GUI.Label(new Rect(x + 3, y + 4, 0, 10), text, labelStyleShadow);
                    GUI.Label(new Rect(x, y, 0, 10), text, labelStyle);
                }
                text = map.calc.prettyCurrentLatLon;
                x = GUIResizer.authoredScreenWidth / 2.0f;
                y = GUIResizer.authoredScreenHeight - 20;
                GUI.Label(new Rect(x, y, 0, 10), text, labelStyle);
            };
            */

            // Button to clear colorized countries
            /*if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth - 190, 100, 180, 30), "  Reset countries", buttonStyle)) {
                map.HideCountrySurfaces();
            }
            */
            /*
                        // Tickers sample
                        if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth - 190, 140, 180, 30), "  Tickers Sample", buttonStyle)) {
                            TickerSample();
                        }

                        // Decorator sample
                        if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth - 190, 180, 180, 30), "  Texture Sample", buttonStyle)) {
                            TextureSample();
                        }

                        // Moving the Earth sample
                        if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth - 190, 220, 180, 30), "  Toggle Minimize", buttonStyle)) {
                            ToggleMinimize();
                        }

                        // Add marker sample (gameobject)
                        if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth - 190, 260, 180, 30), "  Add Marker (Object)", buttonStyle)) {
                            AddMarkerGameObjectOnRandomCity();
                        }

                        // Add marker sample (gameobject)
                        if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth - 190, 300, 180, 30), "  Add Marker (Circle)", buttonStyle)) {
                            AddMarkerCircleOnRandomPosition();
                        }

                        if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth - 190, 340, 180, 30), "  Add Trajectories", buttonStyle)) {
                            AddTrajectories(10);
                        }

                        if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth - 190, 380, 180, 30), "  Locate Mount Point", buttonStyle)) {
                            LocateMountPoint();
                        }

                        if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth - 190, 420, 180, 30), "  Fire Rocket!", buttonStyle)) {
                            FireRocket();
                        }
            */
            /* if (GUI.Button(new Rect(GUIResizer.authoredScreenWidth -190, 500, 180, 30), "  Show States Names", buttonStyle)) {
                 ShowStatesNames();
             }
             */

        }


        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //MIGLIORARE IL MENÚ PAUSA
                //currentPauseMenu.gameObject.SetActive(!currentPauseMenu.activeSelf);
                pauseMenu.SetActive(!pauseMenu.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                inGameMenu.SetActive(!inGameMenu.activeSelf);
                //currentInGameMenu.SetActive(!currentInGameMenu.activeSelf);
            }


            if (Input.GetKeyDown(KeyCode.G))
            {
                avoidGUI = !avoidGUI;
            }


            //Quando clicco su una categoria fa apparire il Panel con le informazioni
            //map.OnCategoryClick += (int categoryIndex) => AddPanel(categoryIndex);


            // Animates the camera field of view (just a cool effect at the begining)
            if (animatingField)
            {
                if (Camera.main.fieldOfView > 60)
                {
                    Camera.main.fieldOfView -= (181.0f - Camera.main.fieldOfView) / (220.0f - Camera.main.fieldOfView);
                }
                else
                {
                    Camera.main.fieldOfView = 60;
                    animatingField = false;
                }
            }

            /*
            if (Input.GetKeyDown(KeyCode.C)) {

                // Get a destination
                Vector3 destination = map.GetCountry("Spain").localPosition;

                // Modify zoom level to take into account zoomMin and zoomMaxDistance

                // compute zoom level based on altitude
                float altitudeInMeters = 1000f;
                const float EARTH_RADIUS_KM = 6371f;
                float radius = map.transform.localScale.x * 0.5f;

                float distanceToSurfaceWS = radius * (((altitudeInMeters + EARTH_RADIUS_KM) / EARTH_RADIUS_KM) - 1f);
                float maxDistance = map.GetZoomLevelDistance(1f);
                float zoomLevel = (distanceToSurfaceWS - map.zoomMinDistance) / (maxDistance - map.zoomMinDistance);
                zoomLevel = Mathf.Clamp01(zoomLevel);

                // Fly!
                map.FlyToLocation(destination, 2f, zoomLevel);

            }
            */
        }
        /*
        void AddPanel(int categoryIndex) {

                    // If previous panel exists, destroy it
                    if (currentPanel != null) {
                        Destroy(currentPanel);
                    }

                    // Instantiate panel
                    currentPanel = Instantiate<GameObject>(panelPrefab);

                    // Update panel texts
                    Text categoryName, categoryDescription, categoryYear, categoryLink;
            
                    categoryName = currentPanel.transform.Find("Panel/RowCategory/CategoryName").GetComponent<Text>();
                    categoryDescription = currentPanel.transform.Find("Panel/RowCategoryDescription/CategoryDescription").GetComponent<Text>();
                    categoryYear = currentPanel.transform.Find("Panel/RowCategoryYear/CategoryYear").GetComponent<Text>();
                    categoryLink = currentPanel.transform.Find("Panel/RowLink/LinkText").GetComponent<Text>();

                    // Gets the category clicked and populate data
                    Category category = map.GetCategory(categoryIndex);
                    categoryName.text = category.name;
                    categoryDescription.text = category.description;
                    categoryYear.text = category.year.ToString();

                    categoryLink.text = category.categoryLink;
            
                    // Position the canvas over the globe
                    float distaceToGlobeCenter = 1.0f;
                    Vector3 worldPos = map.transform.TransformPoint(category.localPosition * distaceToGlobeCenter);
                    currentPanel.transform.position = worldPos;

                    // Draw a circle around the city
                    //map.AddMarker(MARKER_TYPE.CIRCLE_PROJECTED, category.localPosition, 60, 0.8f, 1f, Color.green);

                    // Parent the panel to the globe so it rotates with it
                    //currentPanel.transform.SetParent(map.transform, true);
                    
                }
        */


        /*  void AddLabel(int categoryIndex)
          {
              string text;
              float x, y;
              Vector3 mousePos = Input.mousePosition;

              if (map.countryHighlighted != null && map.cityHighlighted != null || map.provinceHighlighted != null ||
                  map.categoryHighlighted != null)
              {
                  Category category = map.categoryHighlighted;

              if (category != null)
              {

                  if (category.description != null || !string.Equals(category.description, "") || category.year != -1)
                  {
                      //text = "Event: " + map.categoryHighlighted.name + "Year: " + map.categoryHighlighted.year + "Description: " + map.categoryHighlighted.description ;
                     text = "Event: " + map.categoryHighlighted.name +  ";\nYear: " + map.categoryHighlighted.year +  ";\nDescription: " + map.categoryHighlighted.description;
                  }
                  else
                  {
                      text = "";
                  }
                  x = GUIResizer.authoredScreenWidth * (mousePos.x / Screen.width);
                  y = GUIResizer.authoredScreenHeight - (GUIResizer.authoredScreenHeight * (mousePos.y / Screen.height)) - 20 * (Input.touchSupported ? 3 : 1); // slightly up for touch devices
                  GUI.Label(new Rect(x - 1, y - 1, 0, 10), text, labelStyleShadow);
                  GUI.Label(new Rect(x + 1, y + 2, 0, 10), text, labelStyleShadow);
                  GUI.Label(new Rect(x + 2, y + 3, 0, 10), text, labelStyleShadow);
                  GUI.Label(new Rect(x + 3, y + 4, 0, 10), text, labelStyleShadow);
                  GUI.Label(new Rect(x, y, 0, 10), text, labelStyle);
              }
              text = map.calc.prettyCurrentLatLon;
              x = GUIResizer.authoredScreenWidth / 2.0f;
              y = GUIResizer.authoredScreenHeight - 20;
              GUI.Label(new Rect(x, y, 0, 10), text, labelStyle);
              }
          }
          */


        string EntityListToString<T>(List<T> entities)
        {
            StringBuilder sb = new StringBuilder("Neighbours: ");
            for (int k = 0; k < entities.Count; k++)
            {
                if (k > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(((IAdminEntity)entities[k]).name);
            }
            return sb.ToString();
        }


        // Sample code to show how to:
        // 1.- Navigate and center a country in the map
        // 2.- Add a blink effect to one country (can be used on any number of countries)

        void FlyToCountry(string countryName)
        {
            int countryIndex = map.GetCountryIndex(countryName);
            float zoomLevel = map.GetCountryMainRegionZoomExtents(countryIndex);
            map.FlyToCountry(countryIndex, 2f, zoomLevel, 0.5f);
            map.BlinkCountry(countryIndex, Color.black, Color.green, 4, 2.5f, true);
        }

        // Sample code to show how to navigate to a city:
        void FlyToCity(string cityName)
        {
            int cityIndex = map.GetCityIndex(cityName);
            map.FlyToCity(cityIndex, 2f, 0.2f, 0.5f);
        }


        // Sample code to show how tickers work
        void TickerSample()
        {
            map.ticker.ResetTickerBands();

            // Configure 1st ticker band: a red band in the northern hemisphere
            TickerBand tickerBand = map.ticker.tickerBands[0];
            tickerBand.verticalOffset = 0.2f;
            tickerBand.backgroundColor = new Color(1, 0, 0, 0.9f);
            tickerBand.scrollSpeed = 0; // static band
            tickerBand.visible = true;
            tickerBand.autoHide = true;

            // Prepare a static, blinking, text for the red band
            TickerText tickerText = new TickerText(0, "WARNING!!");
            tickerText.textColor = Color.yellow;
            tickerText.blinkInterval = 0.2f;
            tickerText.horizontalOffset = 0.1f;
            tickerText.duration = 10.0f;

            // Draw it!
            map.ticker.AddTickerText(tickerText);

            // Configure second ticker band (below the red band)
            tickerBand = map.ticker.tickerBands[1];
            tickerBand.verticalOffset = 0.1f;
            tickerBand.verticalSize = 0.05f;
            tickerBand.backgroundColor = new Color(0, 0, 1, 0.9f);
            tickerBand.visible = true;
            tickerBand.autoHide = true;

            // Prepare a ticker text
            tickerText = new TickerText(1, "INCOMING MISSILE!!");
            tickerText.textColor = Color.white;

            // Draw it!
            map.ticker.AddTickerText(tickerText);
        }

        // Sample code to show how to use decorators to assign a texsture
        void TextureSample()
        {
            // 1st way (best): assign a flag texture to USA using direct API - this texture will get cleared when you call HideCountrySurfaces()
            Texture2D texture = Resources.Load<Texture2D>("flagUSA");
            int countryIndex = map.GetCountryIndex("United States of America");
            map.ToggleCountrySurface(countryIndex, true, Color.white, texture);

            // 2nd way: assign a flag texture to Brazil using decorator - the texture will stay when you call HideCountrySurfaces()
            string countryName = "Brazil";
            CountryDecorator decorator = new CountryDecorator();
            decorator.isColorized = true;
            decorator.texture = Resources.Load<Texture2D>("flagBrazil");
            decorator.textureOffset = Misc.Vector2down * 2.4f;
            map.decorator.SetCountryDecorator(0, countryName, decorator);

            Debug.Log("USA flag added with direct API.");
            Debug.Log("Brazil flag added with decorator (persistent texture).");

            map.FlyToCountry("Panama", 2f);
        }




        // The globe can be moved and scaled at wish
        void ToggleMinimize()
        {
            minimizeState = !minimizeState;

            Camera.main.transform.position = Vector3.back * 1.1f;
            Camera.main.transform.rotation = Misc.QuaternionZero; // Quaternion.Euler (Misc.Vector3zero);
            if (minimizeState)
            {
                map.gameObject.transform.localScale = Misc.Vector3one * 0.20f;
                map.gameObject.transform.localPosition = new Vector3(0.0f, -0.5f, 0);
                map.allowUserZoom = false;
                //map.earthStyle = EARTH_STYLE.Alternate2;
                map.earthColor = Color.black;
                map.longitudeStepping = 4;
                map.latitudeStepping = 40;
                map.showFrontiers = false;
                map.showCities = false;
                map.showCountryNames = false;
                map.gridLinesColor = new Color(0.06f, 0.23f, 0.398f);
            }
            else
            {
                map.gameObject.transform.localScale = Misc.Vector3one;
                map.gameObject.transform.localPosition = Misc.Vector3zero;
                map.allowUserZoom = true;
                map.earthStyle = EARTH_STYLE.NaturalHighRes16K;
                map.longitudeStepping = 15;
                map.latitudeStepping = 15;
                map.showFrontiers = true;
                map.showCities = true;
                map.showCountryNames = true;
                map.gridLinesColor = new Color(0.16f, 0.33f, 0.498f);
            }
        }


        /// <summary>
        /// Illustrates how to add custom markers over the globe using the AddMarker API.
        /// In this example a building prefab is added to a random city (see comments for other options).
        /// </summary>
        void AddMarkerGameObjectOnRandomCity()
        {

            // Every marker is put on a spherical-coordinate (assuming a radius = 0.5 and relative center at zero position)
            Vector3 sphereLocation;

            // Add a marker on a random city
            City city = map.cities[Random.Range(0, map.cities.Count)];
            sphereLocation = city.localPosition;

            // or... choose a city by its name:
            //		int cityIndex = map.GetCityIndex("Moscow");
            //		sphereLocation = map.cities[cityIndex].unitySphereLocation;

            // or... use the centroid of a country
            //		int countryIndex = map.GetCountryIndex("Greece");
            //		sphereLocation = map.countries[countryIndex].center;

            // or... use a custom location lat/lon. Example put the building over New York:
            //		map.calc.fromLatDec = 40.71f;	// 40.71 decimal degrees north
            //		map.calc.fromLonDec = -74.00f;	// 74.00 decimal degrees to the west
            //		map.calc.fromUnit = UNIT_TYPE.DecimalDegrees;
            //		map.calc.Convert();
            //		sphereLocation = map.calc.toSphereLocation;

            // Send the prefab to the AddMarker API setting a scale of 0.02f (this depends on your marker scales)
            GameObject building = Instantiate(Resources.Load<GameObject>("Building/Building"));

            map.AddMarker(building, sphereLocation, 0.02f);

            // Fly to the destination and see the building created
            map.FlyToLocation(sphereLocation);

            // Optionally add a blinking effect to the marker
            MarkerBlinker.AddTo(building, 4, 0.2f);
        }

        void AddMarkerCircleOnRandomPosition()
        {
            // Draw a beveled circle
            Vector3 sphereLocation = Random.onUnitSphere * 0.5f;
            float km = Random.value * 500 + 500; // Circle with a radius of (500...1000) km

            //			sphereLocation = map.cities[map.GetCityIndex("Paris")].unitySphereLocation;
            //			km = 1053;
            //			sphereLocation = map.cities[map.GetCityIndex("New York")].unitySphereLocation;
            //			km = 500;
            map.AddMarker(MARKER_TYPE.CIRCLE_PROJECTED, sphereLocation, km, 0.975f, 1.0f, new Color(0.85f, 0.45f, 0.85f, 0.9f));
            map.AddMarker(MARKER_TYPE.CIRCLE_PROJECTED, sphereLocation, km, 0, 0.975f, new Color(0.5f, 0, 0.5f, 0.9f));
            map.FlyToLocation(sphereLocation);
        }


        /// <summary>
        /// Example of how to add custom lines to the map
        /// Similar to the AddMarker functionality, you need two spherical coordinates and then call AddLine
        /// </summary>
        void AddTrajectories(int numberOfLines)
        {

            // In this example we will add random lines from a group of cities to another cities (see AddMaker example above for other options to get locations)
            for (int line = 0; line < numberOfLines; line++)
            {
                // Get two random cities
                int city1 = Random.Range(0, map.cities.Count);
                int city2 = Random.Range(0, map.cities.Count);

                // Get their sphere-coordinates
                Vector3 start = map.cities[city1].localPosition;
                Vector3 end = map.cities[city2].localPosition;

                // Add lines with random color, speeds and elevation
                Color color = new Color(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1));
                float elevation = Random.Range(0, 0.5f);    // elevation is % relative to the Earth radius
                float drawingDuration = 4.0f;
                float lineWidth = 0.005f;
                float fadeAfter = 2.0f; // line stays for 2 seconds, then fades out - set this to zero to avoid line removal
                map.AddLine(start, end, color, elevation, drawingDuration, lineWidth, fadeAfter);
            }
        }


        void ShowStatesNames()
        {
            // First we ensure only states for USA are shown
            int countryUSAIndex = map.GetCountryIndex("United States of America");
            for (int k = 0; k < map.countries.Length; k++)
            {
                if (k != countryUSAIndex)
                {
                    map.countries[k].allowShowProvinces = false;
                }
            }
            map.showProvinces = true;
            map.drawAllProvinces = true;

            // Now, hide all country names and show states for USA
            map.showCountryNames = false;
            Country usaCountry = map.countries[countryUSAIndex];
            for (int p = 0; p < usaCountry.provinces.Length; p++)
            {
                Province state = usaCountry.provinces[p];
                Color color = new Color(Random.value, Random.value, Random.value);
                map.AddText(state.name, state.localPosition, color);
            }

            map.FlyToCountry(usaCountry);
        }

        #region Bullet shooting!

        /// <summary>
        /// Creates a rocket on current map position and launch it over a random position on the globe following an arc
        /// </summary>
        ///
        /*
        void FireRocket() {

            GameObject rocket = Instantiate(rocketPrefab);
            rocket.GetComponentInChildren<Renderer>().material.color = Color.yellow;

            // Choose starting pos
            Vector3 startPos = map.GetCurrentMapLocation();

            // Get a random target city
            int randomCity = Random.Range(0, map.cities.Count);
            Vector3 endPos = map.cities[randomCity].localPosition;

            // Fire the bullet!
            StartCoroutine(AnimateMissile(rocket, 0.005f, startPos, endPos));
        }
        
        */

        IEnumerator AnimateMissile(GameObject missile, float scale, Vector3 startPos, Vector3 endPos, float duration = 3f, float arc = 0.25f)
        {

            // Optional: Draw the trajectory
            map.AddLine(startPos, endPos, Color.white, arc, duration, lineWidth: 0.002f, fadeOutAfter: 0.1f);

            // Optional: Follow the bullet
            map.FlyToLocation(endPos, duration);

            // Animate loop for moving bullet over time
            float bulletFireTime = Time.time;
            float elapsed = Time.time - bulletFireTime;
            Vector3 prevPos = map.transform.position;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                Vector3 pos = Vector3.Lerp(startPos, endPos, t).normalized * 0.5f;
                float altitude = Mathf.Sin(t * Mathf.PI) * arc / scale;
                map.AddMarker(missile, pos, scale, true, altitude);

                Vector3 wpos = map.transform.TransformPoint(pos);
                if (elapsed > 0)
                {
                    Vector3 axis = (wpos - prevPos).normalized;
                    missile.transform.forward = axis;
                }
                prevPos = wpos;

                yield return new WaitForFixedUpdate();
                elapsed = Time.time - bulletFireTime;
            }

            Destroy(missile);

        }


        #endregion

    }

}