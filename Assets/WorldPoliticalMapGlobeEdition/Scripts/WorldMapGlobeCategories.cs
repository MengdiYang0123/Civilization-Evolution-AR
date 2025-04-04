// World Political Map - Globe Edition for Unity - Main Script
// Created by Ramiro Oliva (Kronnect)
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WPM
// ***************************************************************************
// This is the public API file - every property or public method belongs here
// ***************************************************************************

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

namespace WPM {

    public delegate void CategoryEvent(int categoryIndex);

    /* Public WPM Class */
    public partial class WorldMapGlobe : MonoBehaviour
    {

        public event CategoryEvent OnCategoryEnter; //QUESTI DOVREBBERO SERVIRE SOLO PER LE DEMO
        public event CategoryEvent OnCategoryExit;
        public event CategoryEvent OnCategoryPointerDown;
        public event CategoryEvent OnCategoryPointerUp;
        public event CategoryEvent OnCategoryClick;

        /// <summary>
        /// Complete list of categories in English with their names and country names.
        /// </summary>
        public List<Category> categories
        {
            get
            {
                if (_categories == null)
                    ReadCategoriesPackedString(SceneManager.GetActiveScene().name, _setLanguage);
                return _categories;
            }
            set
            {
                _categories = value;
                lastCategoryLookupCount = -1;
            }
        }

        string[] categoryClassOptions = new string[]
        {
            "All",
            "Battles",
            "Art",
            "Science",
            "Discovery",
            "Treaty",
            "HumanRights",
            "FunFact",
            "Tragedy",
            "Event"
        };

        int[] categoryClassValues = new int[]
         {
             (int)CATEGORY_CLASS.ALL,
             (int)CATEGORY_CLASS.BATTLE,
             (int)CATEGORY_CLASS.ART,
             (int)CATEGORY_CLASS.SCIENCE,
             (int)CATEGORY_CLASS.DISCOVERY,
             (int)CATEGORY_CLASS.TREATY,
             (int)CATEGORY_CLASS.FUNFACT,
             (int)CATEGORY_CLASS.HUMANRIGHTS,
             (int)CATEGORY_CLASS.TRAGEDY,
             (int)CATEGORY_CLASS.EVENT
         };
            Category _categoryHighlighted;

        /// <summary>
        /// Returns Category under mouse position or null if none.
        /// </summary>
        public Category categoryHighlighted { get { return _categoryHighlighted; } }

        int _categoryHighlightedIndex = -1;

        /// <summary>
        /// Returns Category index mouse position or null if none.
        /// </summary>
        public int categoryHighlightedIndex { get { return _categoryHighlightedIndex; } }

        int _categoryLastClicked = -1;

        /// <summary>
        /// Returns the last clicked city index.
        /// </summary>
        public int categoryLastClicked { get { return _categoryLastClicked; } }



        [SerializeField]
        bool
            _showCategories = true;
        
        [SerializeField]
        bool
            _showBattles = true;
        
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
                    }
                }
            }
        }
        
        
        [SerializeField]
        bool
            _showArts = true;
        
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
                    }
                }
            }
        }
        
        
        [SerializeField]
        bool
            _showDiscoveries = true;
        
        public bool showDiscoveries {
            get {
                return _showDiscoveries;
            }
            set {
                if (_showDiscoveries != value) {
                    _showDiscoveries = value;
                    isDirty = true;
                    if (discoveryLayer != null) {
                        discoveryLayer.SetActive(_showDiscoveries);
                    }
                }
            }
        }
        
        [SerializeField]
        bool
            _showSciences = true;
        
        public bool showSciences {
            get {
                return _showSciences;
            }
            set {
                if ( _showSciences!= value) {
                    _showSciences = value;
                    isDirty = true;
                    if (scienceLayer != null) {
                        scienceLayer.SetActive(_showSciences);
                    }
                }
            }
        }
        
        
        [SerializeField]
        bool
            _showTreaties = true;
        
        public bool showTreaties {
            get {
                return _showTreaties;
            }
            set {
                if ( _showTreaties!= value) {
                    _showTreaties = value;
                    isDirty = true;
                    if (treatyLayer != null) {
                        treatyLayer.SetActive(_showTreaties);
                    }
                }
            }
        }
        
        [SerializeField]
        bool
            _showHumanRights = true;
        
        public bool showHumanRights {
            get {
                return _showHumanRights;
            }
            set {
                if ( _showHumanRights!= value) {
                    _showHumanRights = value;
                    isDirty = true;
                    if (humanRightsLayer != null) {
                        humanRightsLayer.SetActive(_showHumanRights);
                    }
                }
            }
        }
        
        [SerializeField]
        bool
            _showFunFact = true;
        
        public bool showFunFact {
            get {
                return _showFunFact;
            }
            set {
                if ( _showFunFact!= value) {
                    _showFunFact = value;
                    isDirty = true;
                    if (funFactLayer != null) {
                        funFactLayer.SetActive(_showFunFact);
                    }
                }
            }
        }

        [SerializeField]
        bool
            _showTragedy = true;
        
        public bool showTragedy {
            get {
                return _showTragedy;
            }
            set {
                if ( _showTragedy!= value) {
                    _showTragedy = value;
                    isDirty = true;
                    if (tragedyLayer != null) {
                        tragedyLayer.SetActive(_showTragedy);
                    }
                }
            }
        }
        
        [SerializeField]
        bool
            _showEvent = true;
        public bool showEvent {
            get {
                return _showEvent;
            }
            set {
                if ( _showEvent!= value) {
                    _showEvent = value;
                    isDirty = true;
                    if (eventLayer != null) {
                        eventLayer.SetActive(_showEvent);
                    }
                }
            }
        }
        
        

        [SerializeField]
       int _filterCategoriesClass = -1 ; 
       
        [SerializeField]
       int _filterCategoriesMinYear = -9999 ;

       [SerializeField]
       int _filterCategoriesMaxYear = 3000;
           

        /// <summary>
        /// Toggle categories visibility.
        /// </summary>
        
        /*public bool showCategories {
        
            get {
                return _showCategories;
            }
            set {//QUESTO SE VARIA IL TOGGLE DI SHOW CATEGORIES
                if (_showCategories != value) {
                    _showCategories = value;
                    isDirty = true;
                    if (categoriesLayer != null) {
                        categoriesLayer.SetActive(_showCategories);
                    } else if (_showCategories) {
                        DrawCategories(_filterCategoriesClass); //QUI DENTRO DECIDO QUALI DISEGNARE MA DEVO PASSARGLI IL FILTRO?
                    }
                }
                if (_showCategories != value) {
                    _showCategories = value;
                    isDirty = true;
                    if (battleLayer != null) {
                        battleLayer.SetActive(_showCategories);
                    } else if (_showCategories) {
                        battleLayer.SetActive(_showCategories); //QUI DENTRO DECIDO QUALI DISEGNARE MA DEVO PASSARGLI IL FILTRO?
                    }
                }

            }
        }
        */
        
        public bool showCategories {
            get {
                return _showCategories;
            }
            set {
                if (_showCategories != value) {
                    _showCategories = value;
                    isDirty = true;
                    if (battleLayer != null) {
                        battleLayer.SetActive(_showCategories);
                    } else if (_showCategories) {
                        battleLayer.SetActive(_showCategories); //QUI DENTRO DECIDO QUALI DISEGNARE MA DEVO PASSARGLI IL FILTRO?
                    }
                }

            }
        }
        
        public int FilterCategoriesClass {
            get {
                return _filterCategoriesClass;
            }
            set {
                if (_filterCategoriesClass != value) {
                    _filterCategoriesClass = value;
                    
                    //Il parametro dell'anno non serve perché poi faccio i controlli con <3000 e >-9999 
                       DrawCategories(_filterCategoriesClass);
                }
            }
        }

        
        public int FilterCategoriesMaxYear {
            get {
                return _filterCategoriesMaxYear;
            }
            set {
                if (_filterCategoriesMaxYear != value) {
                    _filterCategoriesMaxYear = value;
                    
                    //DrawCategories(_filterCategoriesMaxYear);
                }
            }
        }
        
        public int FilterCategoriesMinYear {
            get {
                return _filterCategoriesMinYear;
            }
            set {
                if (_filterCategoriesMinYear != value) {
                    _filterCategoriesMinYear = value;
                    
                    //DrawCategories(_filterCategoriesMinYear);
                }
            }
        }

        [NonSerialized]
        int
            _numCategoriesDrawn = 0;

        /// <summary>
        /// Gets the number categories drawn.
        /// </summary>
        public int numCategoriesDrawn { get { return _numCategoriesDrawn; } }


        
        [SerializeField]
        Color
            _categoriesBattleColor = Color.white;

        /// <summary>
        /// Global color for Battle Categories.
        /// </summary>
        public Color categoriesBattleColor {
            get {
                if (categoriesBattleMat != null) {
                    return categoriesBattleMat.color;
                } else {
                    return _categoriesBattleColor;
                }
            }
            set {
                if (value != _categoriesBattleColor) {
                    _categoriesBattleColor = value;
                    isDirty = true;

                    if (categoriesBattleMat != null && _categoriesBattleColor != categoriesBattleMat.color) {
                        categoriesBattleMat.color = _categoriesBattleColor;
                    }
                }
            }
        }


        [SerializeField] 
        Color
            _categoriesArtColor = Color.magenta;
        /// <summary>
        /// Global color for Art Categories.
        /// </summary>
        public Color categoriesArtColor {
            get {
                if (categoriesArtMat != null) {
                    return categoriesArtMat.color;
                } else {
                    return _categoriesArtColor;
                }
            }
            set {
                if (value != _categoriesArtColor) {
                    _categoriesArtColor = value;
                    isDirty = true;

                    if (categoriesArtMat != null && _categoriesArtColor != categoriesArtMat.color) {
                        categoriesArtMat.color = _categoriesArtColor;
                    }
                }
            }
        }

[SerializeField]
Color
    _categoriesScienceColor = Color.blue;
/// <summary>
/// Global color for Science Categories.
/// </summary>
public Color categoriesScienceColor {
    get {
        if (categoriesScienceMat != null) {
            return categoriesScienceMat.color;
        } else {
            return _categoriesScienceColor;
        }
    }
    set {
        if (value != _categoriesScienceColor) {
            _categoriesScienceColor = value;
            isDirty = true;

            if (categoriesScienceMat != null && _categoriesScienceColor != categoriesScienceMat.color) {
                categoriesScienceMat.color = _categoriesScienceColor;
            }
        }
    }
}

[SerializeField]
Color
    _categoriesTreatyColor = Color.grey;
/// <summary>
/// Global color for Science Categories.
/// </summary>
public Color categoriesTreatyColor {
    get {
        if (categoriesTreatyMat != null) {
            return categoriesTreatyMat.color;
        } else {
            return _categoriesTreatyColor;
        }
    }
    set {
        if (value != _categoriesTreatyColor) {
            _categoriesTreatyColor = value;
            isDirty = true;

            if (categoriesTreatyMat != null && _categoriesTreatyColor != categoriesTreatyMat.color) {
                categoriesTreatyMat.color = _categoriesTreatyColor;
            }
        }
    }
}

[SerializeField]
Color
    _categoriesDiscoveryColor = Color.grey;
/// <summary>
/// Global color for Science Categories.
/// </summary>
public Color categoriesDiscoveryColor {
    get {
        if (categoriesDiscoveryMat != null) {
            return categoriesTreatyMat.color;
        } else {
            return _categoriesDiscoveryColor;
        }
    }
    set {
        if (value != _categoriesDiscoveryColor) {
            _categoriesDiscoveryColor = value;
            isDirty = true;

            if (categoriesDiscoveryMat != null && _categoriesDiscoveryColor != categoriesDiscoveryMat.color) {
                categoriesDiscoveryMat.color = _categoriesDiscoveryColor;
            }
        }
    }
}

[SerializeField]
Color
    _categoriesTragedyColor = Color.grey;
/// <summary>
/// Global color for Science Categories.
/// </summary>
public Color categoriesTragedyColor {
    get {
        if (categoriesTragedyMat != null) {
            return categoriesTragedyMat.color;
        } else {
            return _categoriesTragedyColor;
        }
    }
    set {
        if (value != _categoriesTragedyColor) {
            _categoriesTragedyColor = value;
            isDirty = true;

            if (categoriesTragedyMat != null && _categoriesTragedyColor != categoriesTragedyMat.color) {
                categoriesTragedyMat.color = _categoriesTragedyColor;
            }
        }
    }
}

[SerializeField]
Color
    _categoriesEventColor = Color.grey;
/// <summary>
/// Global color for Event Categories.
/// </summary>
public Color categoriesEventColor {
    get {
        if (categoriesEventMat != null) {
            return categoriesTreatyMat.color;
        } else {
            return _categoriesEventColor;
        }
    }
    set {
        if (value != _categoriesEventColor) {
            _categoriesEventColor = value;
            isDirty = true;

            if (categoriesEventMat != null && _categoriesEventColor != categoriesEventMat.color) {
                categoriesEventMat.color = _categoriesEventColor;
            }
        }
    }
}


[SerializeField]
        float _categoryIconSize = 0.4f;

        /// <summary>
        /// The size of the categories icon (dot).
        /// </summary>
        public float categoryIconSize {
            get {
                return _categoryIconSize ;
            }
            set {
                if (value != _categoryIconSize) {
                    _categoryIconSize = value;
                    isDirty = true;
                    ScaleCities();
                    ScaleCategories();
                    ScaleMountPoints();
                }
            }
        }
        //DESCRIPTION
        [SerializeField]
        string
            _minDescription = "ND";

        public string minDescription {
            get {
                return _minDescription;
            }
            set {
                if (value != _minDescription) {
                    _minDescription = value;
                    isDirty = true;
                    DrawCategories(_filterCategoriesClass);
                }
            }
        }


        [SerializeField]
        int _categoryClassAlwaysShow;

        /// <summary>
        /// Flags for specifying the class of categories to always show irrespective of other filter. Can assign a combination of bit flags defined by CITY_CLASS_FILTER* constants.
        /// </summary>
        public int categoryClassAlwaysShow {
            get { return _categoryClassAlwaysShow; }
            set {
                if (_categoryClassAlwaysShow != value) {
                    _categoryClassAlwaysShow = value;
                    isDirty = true;
                    DrawCategories(_filterCategoriesClass);
                }
            }
        }
        
        
        [SerializeField]
        bool
            _combineCategoryMeshes = true;

        /// <summary>
        /// Optimize categories meshes.
        /// </summary>
        public bool combineCategoryMeshes {
            get {
                return _combineCategoryMeshes;
            }
            set {
                if (_combineCategoryMeshes != value) {
                    _combineCategoryMeshes = value;
                    isDirty = true;
                    DrawCategories(_filterCategoriesClass);
                }
            }
        }



        string _categoryAttributeFile = CATEGORY_ATTRIB_DEFAULT_FILENAME;

        public string categoryAttributeFile {
            get { return _categoryAttributeFile; }
            set {
                if (value != _categoryAttributeFile) {
                    _categoryAttributeFile = value;
                    if (_categoryAttributeFile == null)
                        _categoryAttributeFile = CATEGORY_ATTRIB_DEFAULT_FILENAME;
                    isDirty = true;
                    ReloadCategoriesAttributes();
                }
            }
        }


        #region Public API area

        /// <summary>
        /// Starts navigation to target category. Returns false if not found.
        /// </summary>
        public CallbackHandler FlyToCategory(string categoryName) {
            int categoryIndex = GetCategoryIndex(categoryName);
            return FlyToCategory(categoryIndex);
        }

        /// <summary>
        /// Starts navigation to target category. Returns false if not found.
        /// </summary>
        public CallbackHandler FlyToCategory(string countryName, string categoryName) {
            int categoryIndex = GetCategoryIndexInCountry(countryName, categoryName);
            return FlyToCategory(categoryIndex);
        }

        /// <summary>
        /// Starts navigation to target category by index in the categories collection. Returns false if not found.
        /// </summary>
        public CallbackHandler FlyToCategory(int categoryIndex) {
            if (categoryIndex < 0 || categoryIndex >= categories.Count)
                return CallbackHandler.Null;
            return FlyToCategory(categories[categoryIndex]);
        }


        /// <summary>
        /// Starts navigation to target category by index in the cities collection and duration. Returns false if not found.
        /// </summary>
        public CallbackHandler FlyToCategory(int categoryIndex, float duration) {
            if (categoryIndex < 0 || categoryIndex >= categories.Count)
                return CallbackHandler.Null;
            return FlyToLocation(categories[categoryIndex].localPosition, duration, 0, _navigationBounceIntensity);
        }


        /// <summary>
        /// Starts navigation to target category. Returns false if not found.
        /// </summary>
        public CallbackHandler FlyToCategory(Category category) {
            return FlyToCategory(category, _navigationTime);
        }

        /// <summary>
        /// Starts navigation to target category with duration (seconds). Returns false if not found.
        /// </summary>
        public CallbackHandler FlyToCategory(Category category, float duration) {
            return FlyToLocation(category.localPosition, duration, 0, _navigationBounceIntensity);
        }


        /// <summary>
        /// Starts navigating to target category by index in the categories collection with specified duration, ignoring NavigationTime property.
        /// Set duration to zero to go instantly.
        /// Set zoomLevel to a value from 0 to 1 for the destination zoom level. A value of 0 will keep current zoom level.
        /// </summary>
        public CallbackHandler FlyToCategory(int categoryIndex, float duration, float zoomLevel) {
            if (categoryIndex < 0 || categoryIndex >= categories.Count)
                return CallbackHandler.Null;
            return FlyToLocation(_categories[categoryIndex].localPosition, duration, zoomLevel, _navigationBounceIntensity);
        }

        /// <summary>
        /// Starts navigating to target category by index in the cities collection with specified duration, ignoring NavigationTime property.
        /// </summary>
        /// <param name="cityIndex">City index.</param>
        /// <param name="duration">Set duration to zero to go instantly.</param>
        /// <param name="zoomLevel">Set zoomLevel to a value from 0 to 1 for the destination zoom level.</param>
        /// <param name="bounceIntensity">Set bounceIntensity to a value from 0 to 1 for a bouncing effect between current position and destination.</param>
        public CallbackHandler FlyToCategory(int categoryIndex, float duration, float zoomLevel, float bounceIntensity) {
            if (categoryIndex < 0 || categoryIndex >= categories.Count)
                return CallbackHandler.Null;
            return FlyToLocation(_categories[categoryIndex].localPosition, duration, zoomLevel, bounceIntensity);
        }


        /// <summary>
        /// Returns an array with the category names.
        /// </summary>
        public string[] GetCategoryNames() {
            return GetCategoryNames(true);
        }

        /// <summary>
        /// Returns an array with the category names.
        /// </summary>
        public string[] GetCategoryNames(bool includeCategoryIndex) {
            List<string> c = new List<string>(categories.Count);
            for (int k = 0; k < categories.Count; k++) {
                if (includeCategoryIndex) {
                    c.Add(categories[k].name + " (" + k + ")");
                } else {
                    c.Add(categories[k].name);
                }
            }
            c.Sort();
            return c.ToArray();
        }

        /// <summary>
        /// Returns an array with the category names.
        /// </summary>
        public string[] GetCategoryNames(int countryIndex, bool includeCategoryIndex) {
            List<string> c = new List<string>(categories.Count);
            for (int k = 0; k < categories.Count; k++) {
                if (categories[k].countryIndex == countryIndex) {
                    if (includeCategoryIndex) {
                        c.Add(categories[k].name + " (" + k + ")");
                    } else {
                        c.Add(categories[k].name);
                    }
                }
            }
            c.Sort();
            return c.ToArray();
        }
        //New tag
        /*public string[] GetCategoryTag(int countryIndex, bool includeCategoryIndex) {
            List<string> c = new List<string>(categories.Count);
            for (int k = 0; k < categories.Count; k++) {
                if (categories[k].countryIndex == countryIndex) {
                    if (includeCategoryIndex) {
                        c.Add(categories[k].tag + " (" + k + ")");
                    } else {
                        c.Add(categories[k].tag);
                    }
                }
            }
            c.Sort();
            return c.ToArray();
        }*/
        
        /// <summary>
        /// Returns an array with the category descriptions.
        /// </summary>
        public string[] GetCategoryDescriptions(int countryIndex, bool includeCategoryIndex) {
            List<string> c = new List<string>(categories.Count);
            for (int k = 0; k < categories.Count; k++) {
                if (categories[k].countryIndex == countryIndex) {
                    if (includeCategoryIndex) {
                        c.Add(categories[k].description + " (" + k + ")");
                    } else {
                        c.Add(categories[k].description);
                    }
                }
            }
            c.Sort();
            return c.ToArray();
        }

        /// <summary>
        /// Given a country name and a category name, returns the Category object
        /// </summary>
        /// <returns>The city.</returns>
        /// <param name="countryName">Country name.</param>
        /// <param name="cityName">City name.</param>
        public Category GetCategory(string countryName, string categoryName) {
            int countryIndex = GetCountryIndex(countryName);
            return GetCategory(countryIndex, categoryName);
        }

        /*
        /// <summary>
        /// Given a country name, province name and a city name, returns the City object
        /// </summary>
        /// <returns>The city.</returns>
        /// <param name="countryName">Country name.</param>
        /// <param name="provinceName">Province name.</param>
        /// <param name="cityName">City name.</param>
        public Category GetCategory(string countryName, string provinceName, string cityName) {
            int cityIndex = GetCityIndex(countryName, provinceName, cityName);
            if (cityIndex < 0)
                return null;
            return cities[cityIndex];
        }
        */
        
        /// <summary>
        /// Gets a city object by its index
        /// </summary>
        public Category GetCategory(int categoryIndex) {
            if (categoryIndex < 0 || categoryIndex >= categories.Count) {
                return null;
            }
            return _categories[categoryIndex];
        }


        /// <summary>
        /// Given a country index and a category name returns the Category object
        /// </summary>
        /// <returns>The Categeory.</returns>
        /// <param name="countryIndex">Country index.</param>
        /// <param name="categoryName">Category name.</param>
        public Category GetCategory(int countryIndex, string categoryName) {
            int categoryIndex = GetCategoryIndexInCountry(countryIndex, categoryName);
            if (categoryIndex < 0 || categoryIndex >= categories.Count) {
                return null;
            }
            return _categories[categoryIndex];
        }

        /// <summary>
        /// Returns the index of a random visible Category.
        /// </summary>
        public int GetCategoryIndex() {
            if (categories == null)
                return -1;
            int z = UnityEngine.Random.Range(0, categories.Count);
            int categoryCount = categories.Count;
            for (int k = z; k < categoryCount; k++) {
                if (categories[k].isShown)
                    return k;
            }
            for (int k = 0; k < z; k++) {
                if (categories[k].isShown)
                    return k;
            }
            return -1;
        }


        /// <summary>
        /// Returns the index of the category by its name in the cities collection.
        /// </summary>
        public int GetCategoryIndex(string categoryName) {
            int categoryCount = categories.Count;
            for (int k = 0; k < categoryCount; k++) {
                if (categoryName.Equals(categories[k].name)) {
                    return k;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the city by its name in the cities collection of a given country and province.
        /// </summary>
        /*
        public int GetCityIndex(string countryName, string provinceName, string cityName) {
            int countryIndex = GetCountryIndex(countryName);
            return GetCityIndex(countryIndex, provinceName, cityName);
        }
        */

        /// <summary>
        /// Returns the index of the city by its name in the cities collection of a given country and province.
        /// </summary>
        /*
        public int GetCityIndex(int countryIndex, string provinceName, string cityName) {
            if (countryIndex < 0 || countryIndex >= countries.Length)
                return -1;
            int provinceIndex = GetProvinceIndex(countryIndex, provinceName);
            if (provinceIndex < 0)
                return -1;
            return GetCityIndexInProvince(provinceIndex, cityName);
        }
        */
        
        /// <summary>
        /// Returns the index of the city by its name in the cities collection of a given province.
        /// </summary>
        /*
        public int GetCityIndexInProvince(int provinceIndex, string cityName) {
            if (provinceIndex < 0 || provinceIndex >= provinces.Length)
                return -1;
            string provinceName = _provinces[provinceIndex].name;
            int countryIndex = _provinces[provinceIndex].countryIndex;
            int cityCount = cities.Count;
            for (int k = 0; k < cityCount; k++) {
                City city = _cities[k];
                if (city.countryIndex == countryIndex && city.province.Equals(provinceName) && cityName.Equals(cities[k].name)) {
                    return k;
                }
            }
            return -1;
        }
        */
        /// <summary>
        /// Returns the index of the city by its name in the cities collection of a given province.
        /// </summary>
        /*
        public int GetCityIndexInProvince(string provinceName, string cityName) {
            int cityCount = cities.Count;
            for (int k = 0; k < cityCount; k++) {
                City city = _cities[k];
                if (city.province.Equals(provinceName) && cityName.Equals(cities[k].name)) {
                    return k;
                }
            }
            return -1;
        }
        */


        /// <summary>
        /// Returns the index of the city by its name in the cities collection of a given country.
        /// </summary>
        public int GetCategoryIndexInCountry(int countryIndex, string categoryName) {
            if (countryIndex < 0 || countryIndex >= countries.Length)
                return -1;
            int categoryCount = categories.Count;
            for (int k = 0; k < categoryCount; k++) {
                Category category = _categories[k];
                if (category.countryIndex == countryIndex && categoryName.Equals(categories[k].name)) {
                    return k;
                }
            }
            return -1;
        }


        /// <summary>
        /// Returns the index of a city in the cities collection by its reference.
        /// </summary>
        public int GetCategoryIndex(Category category) {
            return GetCategoryIndex(category, true);
        }

        /// <summary>
        /// Returns the index of a city in the cities collection by its reference.
        /// </summary>
        public int GetCategoryIndex(Category category, bool includeNotVisible) {
            if (includeNotVisible)
                return categories.IndexOf(category);
            int categoryIndex;
           if (categoryLookUp.TryGetValue(category, out categoryIndex))
                return categoryIndex;
            else
                return -1;
        }

        /// <summary>
        /// Returns the index of a city in the global countries collection. Note that country name needs to be supplied due to repeated city names.
        /// </summary>
        public int GetCategoryIndexInCountry(string countryName, string categoryName) {
            int countryIndex = GetCountryIndex(countryName);
            if (countryIndex < 0 || countryIndex >= countries.Length)
                return -1;
            int categoryCount = categories.Count;
            for (int k = 0; k < categoryCount; k++) {
                Category category = _categories[k];
                if (category.countryIndex == countryIndex && categoryName.Equals(categories[k].name)) {
                    return k;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the category index by screen position.
        /// </summary> //PROBLEMI CAUSA OVERFLOW far vedere sia la categoria che la città + vicina
        public bool GetCategoryIndex(Ray ray, out int categoryIndex, int countryIndex = -1) {
            Vector3 hitPos;
            if (GetGlobeIntersection(ray, out hitPos)) {
                Vector3 localHit = transform.InverseTransformPoint(hitPos);
                int c = GetCategoryNearPoint(localHit, countryIndex);
                if (c >= 0) {
                    categoryIndex = c;
                    return true;
                }
            }
            categoryIndex = -1;
            return false;
        }


        /// <summary>
        /// Returns the index of the nearest category to a location (lat/lon).
        /// </summary>
        public int GetCategoryIndex(float lat, float lon) {
            Vector3 spherePosition = Conversion.GetSpherePointFromLatLon(lat, lon);
            return GetCategoryIndex(spherePosition);
        }

        /// <summary>
        /// Returns the nearest city to a point specified in sphere coordinates.
        /// </summary>
        /// <returns>The city near point.</returns>
        /// <param name="localPoint">Local point in sphere coordinates.</param>
        /// <param name="cityIndexToExclude">Optional city index which will be excluded. Useful for getting the nearest city to a given one.</param>
        public int GetCategoryIndex(Vector2 latlon, int categoryIndexToExclude = -1) {
            if (visibleCategories == null)
                return -1;

            int nearest = -1;
            float minDist = float.MaxValue;
            int categoryCount = categories.Count;
            for (int c = 0; c < categoryCount; c++) {
                if (c == categoryIndexToExclude)
                    continue;
                Category category = categories[c];
                if (!category.isShown)
                    continue;
                Vector2 categoryLoc = category.latlon;
                float dist = FastVector.SqrDistance(ref categoryLoc, ref latlon);
                if (dist < minDist) {
                    minDist = dist;
                    nearest = c;
                }
            }
            return nearest;
        }


        /// <summary>
        /// Returns the nearest city to a point specified in sphere coordinates.
        /// </summary>
        /// <returns>The city near point.</returns>
        /// <param name="localPoint">Local point in sphere coordinates.</param>
        /// <param name="excludeCitiesList">Optional list with city index which will be excluded. Useful for getting the nearest city to a given one.</param>
        public int GetCategoryIndex(Vector2 latlon, List<int> excludeCategoriesList) {
            if (visibleCategories == null)
                return -1;

            int nearest = -1;
            float minDist = float.MaxValue;
            int categoryCount = categories.Count;
            for (int c = 0; c < categoryCount; c++) {
                Category category = categories[c];
                if (!category.isShown)
                    continue;
                float dist = FastVector.SqrDistanceByValue(category.latlon, latlon);
                if (dist < minDist) {
                    if (!excludeCategoriesList.Contains(c)) {
                        minDist = dist;
                        nearest = c;
                    }
                }
            }
            return nearest;
        }


        /// <summary>
        /// Gets the name of the city country.
        /// </summary>
        public string GetCategoryCountryName(int categoryIndex) {
            if (categoryIndex < 0 || categoryIndex >= categories.Count)
                return "";
            int countryIndex = _categories[categoryIndex].countryIndex;
            Country country = GetCountry(countryIndex);
            if (country != null)
                return country.name;
            else
                return "";
        }

        /// <summary>
        /// Convenient method that returns the name of the city plus the province and country names
        /// </summary>
        /// <returns>The city full name.</returns>
        /// <param name="cityIndex">City index.</param>
        /*
        public string GetCategpryFullName(Category category) {
            if (category == null)
                return null;
            sb.Length = 0;
            sb.Append(category.name);
            sb.Append(" (");
            if (!string.IsNullOrEmpty(category.province) && !city.province.Equals(city.name)) {
                sb.Append(city.province);
                sb.Append(", ");
            }
            sb.Append(countries[city.countryIndex].name);
            sb.Append(")");
            return sb.ToString();
        }



        /// <summary>
        /// Convenient method that returns the name of the city plus the province and country names
        /// </summary>
        /// <returns>The city full name.</returns>
        /// <param name="cityIndex">City index.</param>
        public string GetCityFullName(int cityIndex) {
            if (cityIndex < 0 || cityIndex >= cities.Count)
                return null;
            return GetCityFullName(cities[cityIndex]);
        }

        /// <summary>
        /// Gets the name of the city province.
        /// </summary>
        public string GetCityProvinceName(int cityIndex) {
            if (cityIndex < 0 || cityIndex >= cities.Count)
                return "";
            return _cities[cityIndex].province;
        }
        */

        /// <summary>
        /// Gets a random city from a given country
        /// </summary>
        /// <returns>The city.</returns>
        /// <param name="country">Country object.</param>
        public Category GetCategoryRandom(Country country, bool onlyVisible = false) {
            int categoryCount = categories.Count;
            int countryIndex = GetCountryIndex(country);
            List<Category> cc = new List<Category>(100);
            for (int k = 0; k < categoryCount; k++) {
                if ((!onlyVisible || _categories[k].isShown) && _categories[k].countryIndex == countryIndex) {
                    cc.Add(_categories[k]);
                }
            }
            int count = cc.Count;
            if (count == 0)
                return null;
            return cc[UnityEngine.Random.Range(0, count)];
        }
        
        /// <summary>
        /// Gets a random visible category
        /// </summary>
        /// <returns>The city.</returns>
        public Category GetCategoryRandom(bool onlyVisible = true) {
            if (onlyVisible) {
                int count = visibleCategories.Length;
                if (count == 0)
                    return null;
                return visibleCategories[UnityEngine.Random.Range(0, count)];
            } else {
                int count = categories.Count;
                if (count == 0)
                    return null;
                return _categories[UnityEngine.Random.Range(0, count)];
            }
        }

        /// <summary>
        /// Gets a random category index from a given country
        /// </summary>
        /// <returns>The city random.</returns>
        /// <param name="country">Country object.</param>
        public int GetCategoryIndexRandom(Country country, bool onlyVisible = true) {
            Category category = GetCategoryRandom(country, onlyVisible);
            if (category == null)
                return -1;
            return GetCategoryIndex(category);
        }
        

        /// <summary>
        /// Gets the name of the country of the city.
        /// </summary>
        public string GetCategoryCountryName(Category category) {
            Country country = GetCountry(category.countryIndex);
            if (country != null)
                return country.name;
            else
                return "";
        }

        /// <summary>
        /// Returns a list of categories whose attributes matches predicate
        /// </summary>
        public void GetCategories(AttribPredicate predicate, List<Category> results) {
            if (results == null) return;
            int categoryCount = categories.Count;
            for (int k = 0; k < categoryCount; k++) {
                Category category = _categories[k];
                if (category.hasAttributes && predicate(category.attrib))
                    results.Add(category);
            }
        }


        /// <summary>
        /// Gets XML attributes of all categories in jSON format.
        /// </summary>
        public string GetCategoriesAttributes(bool prettyPrint = true) {
            if (categories == null) return null;
            return GetCategoriesAttributes(new List<Category>(categories), prettyPrint);
        }

        /// <summary>
        /// Gets XML attributes of provided categories in jSON format.
        /// </summary>
        public string GetCategoriesAttributes(List<Category> categories, bool prettyPrint = true) {
            JSONObject composed = new JSONObject();
            int categoryCount = categories.Count;
            for (int k = 0; k < categoryCount; k++) {
                Category category = _categories[k];
                if (category.hasAttributes && category.attrib.keys != null) {
                    composed.AddField(k.ToString(), category.attrib);
                }
            }
            return composed.Print(prettyPrint);
        }

        /// <summary>
        /// Sets categories attributes from a jSON formatted string.
        /// </summary>
        public void SetCategoriesAttributes(string jSON) {
            JSONObject composed = new JSONObject(jSON);
            if (composed.keys == null)
                return;
            int keyCount = composed.keys.Count;
            for (int k = 0; k < keyCount; k++) {
                int categoryIndex = int.Parse(composed.keys[k]);
                if (categoryIndex >= 0) {
                    categories[categoryIndex].attrib = composed[k];
                }
            }
        }



        /// <summary>
        /// Clears any category highlighted (color changed) and resets them to default category color
        /// </summary>
        public void HideCategoryHighlights() {
            if (categoriesLayer == null)
                return;
            Renderer[] rr = categoriesLayer.GetComponentsInChildren<Renderer>(true);
            for (int k = 0; k < rr.Length; k++) {
                string matName = rr[k].sharedMaterial.name;
                if (matName.Equals("CategoriesBattle")) {
                    rr[k].sharedMaterial = categoriesBattleMat;
                } 
                else if (matName.Equals("CategoriesArt"))
                    rr[k].sharedMaterial = categoriesArtMat;
                else if (matName.Equals("CategoriesDiscovery"))
                    rr[k].sharedMaterial = categoriesDiscoveryMat;
                else if (matName.Equals("CategoriesScience"))
                    rr[k].sharedMaterial = categoriesScienceMat;
                else if (matName.Equals("CategoriesTreaty"))
                    rr[k].sharedMaterial = categoriesTreatyMat;
                else if (matName.Equals("CategoriesFunFact"))
                    rr[k].sharedMaterial = categoriesFunFactMat;
                else if (matName.Equals("CategoriesHumanRights"))
                    rr[k].sharedMaterial = categoriesHumanRightsMat;
                else if (matName.Equals("CategoriesTragedy"))
                    rr[k].sharedMaterial = categoriesTragedyMat;
            }
        }

        /// <summary>
        /// Toggles the category highlight.
        /// </summary>
        /// <param name="cityIndex">City index.</param>
        /// <param name="color">Color.</param>
        /// <param name="highlighted">If set to <c>true</c> the color of the category will be changed. If set to <c>false</c> the color of the category will be reseted to default color</param>
        public void ToggleCategoryHighlight(int categoryIndex, Color color, bool highlighted) {
            if (categoriesLayer == null)
                return;
            GameObject categoryObj = categories[categoryIndex].gameObject;
            if (categoryObj == null)
                return;
            Renderer rr = categoryObj.GetComponent<Renderer>();
            if (rr == null)
                return;
            Material mat= null; //DA MODIFICARE
            if (highlighted) {
                mat = Instantiate(rr.sharedMaterial);
                mat.name = rr.sharedMaterial.name;
                mat.color = color;
                rr.sharedMaterial = mat;
            } else {
                switch (categories[categoryIndex].categoryClass) {
                    case CATEGORY_CLASS.BATTLE:
                        mat = categoriesBattleMat;
                        break;
                    case CATEGORY_CLASS.ART:
                        mat = categoriesArtMat;
                        break;
                    case CATEGORY_CLASS.DISCOVERY:
                        mat = categoriesDiscoveryMat;
                        break;
                    case CATEGORY_CLASS.SCIENCE:
                        mat = categoriesScienceMat;
                        break;
                    case CATEGORY_CLASS.TREATY:
                        mat = categoriesTreatyMat;
                        break;
                    case CATEGORY_CLASS.FUNFACT:
                        mat = categoriesFunFactMat;
                        break;
                    case CATEGORY_CLASS.HUMANRIGHTS:
                        mat = categoriesHumanRightsMat;
                        break;
                    case CATEGORY_CLASS.TRAGEDY:
                        mat = categoriesTragedyMat;
                        break;
                    case CATEGORY_CLASS.EVENT:
                        mat = categoriesEventMat;
                        break;
                }
                rr.sharedMaterial = mat;
            }
        }
        
        

        /// <summary>
        /// Flashes specified city by index in the global city collection.
        /// </summary>
        public void BlinkCategory(int categoryIndex, Color color1, Color color2, float duration, float blinkingSpeed) {
            if (categoriesLayer == null)
                return;
            //			string cobj = GetCityHierarchyName(cityIndex);
            //			Transform t = transform.Find (cobj);
            GameObject categoryObj = categories[categoryIndex].gameObject;
            if (categoryObj == null)
                return;
            CategoryBlinker sb = categoryObj.AddComponent<CategoryBlinker>();
            sb.blinkMaterial = categoryObj.GetComponent<Renderer>().sharedMaterial;
            sb.color1 = color1;
            sb.color2 = color2;
            sb.duration = duration;
            sb.speed = blinkingSpeed;
        }

        /// <summary>
        /// Deletes all categories of current selected country's continent
        /// </summary>
        public void CategoriessDeleteFromContinent(string continentName) {
            HideCategoryHighlights();
            int k = -1;
            while (++k < categories.Count) {
                int cindex = categories[k].countryIndex;
                if (cindex >= 0) {
                    string categoryContinent = countries[cindex].continent;
                    if (categories.Equals(continentName)) {
                        categories.RemoveAt(k);
                        k--;
                    }
                }
            }
        }


        /// <summary>
        /// Returns a list of provinces that are visible (front facing camera)
        /// </summary>
        public List<Category> GetVisibleCategories() {
            List<Category> vc = new List<Category>(30);
            if (categories == null)
                return null;
            Camera cam = mainCamera;
            float viewportMinX = cam.rect.xMin;
            float viewportMaxX = cam.rect.xMax;
            float viewportMinY = cam.rect.yMin;
            float viewportMaxY = cam.rect.yMax;

            for (int k = 0; k < visibleCategories.Length; k++) {
                Category category = visibleCategories[k];

                // Check if category is facing camera
                Vector3 center = transform.TransformPoint(category.localPosition);
                Vector3 dir = center - transform.position;
                float d = Vector3.Dot(cam.transform.forward, dir);
                if (d < -0.2f) {
                    // Check if category is inside viewport
                    Vector3 vpos = cam.WorldToViewportPoint(center);
                    if (vpos.x >= viewportMinX && vpos.x <= viewportMaxX && vpos.y >= viewportMinY && vpos.y <= viewportMaxY) {
                        vc.Add(category);
                    }
                }
            }
            return vc;
        }


        /// <summary>
        /// Returns a list of cities that are visible and located inside the rectangle defined by two given sphere points
        /// </summary>
        public List<Category> GetVisibleCategories(Vector3 rectTopLeft, Vector3 rectBottomRight) {
            Vector2 latlon0, latlon1;
            latlon0 = Conversion.GetBillboardPosFromSpherePoint(rectTopLeft);
            latlon1 = Conversion.GetBillboardPosFromSpherePoint(rectBottomRight);
            Rect rect = new Rect(latlon0.x, latlon1.y, latlon1.x - latlon0.x, latlon0.y - latlon1.y);
            List<Category> selectedCategories = new List<Category>();

            int categoryCount = visibleCategories.Length;
            for (int k = 0; k < categoryCount; k++) {
                Category category = visibleCategories[k];
                Vector2 bpos = Conversion.GetBillboardPosFromSpherePoint(category.localPosition);
                if (rect.Contains(bpos)) {
                    selectedCategories.Add(category);
                }
            }
            return selectedCategories;
        }

        #endregion


    }

}