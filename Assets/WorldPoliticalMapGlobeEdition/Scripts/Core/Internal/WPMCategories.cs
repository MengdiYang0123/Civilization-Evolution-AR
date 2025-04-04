// World Political Map - Globe Edition for Unity - Main Script
// Created by Ramiro Oliva (Kronnect)
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WPM


using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
//using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace WPM {

    public partial class WorldMapGlobe : MonoBehaviour {

        const float CATEGORIES_HIT_PRECISION = 0.0015f;
        //public const string CATEGORIES_LAYER = "Categories";
        
        const string CATEGORY_ATTRIB_DEFAULT_FILENAME = "categoriesAttrib";
        
        public LANGUAGES_OPTIONS _setLanguage = LANGUAGES_OPTIONS.ENGLISH;
        
        public bool toggleBool = true;

        #region Internal variables
        
        
        List<Category> _categories;
        
        // resources
        
        Material categoriesBattleMat, categoriesArtMat, categoriesScienceMat, categoriesTreatyMat, categoriesDiscoveryMat, categoriesFunFactMat, categoriesHumanRightsMat, categoriesTragedyMat, categoriesEventMat;  //DA MODIFICARE : Creare materiale diversificato per le altre categorie
        GameObject  categoriesLayer, artLayer, battleLayer, discoveryLayer, humanRightsLayer, scienceLayer, treatyLayer, funFactLayer, tragedyLayer, eventLayer, categorySpotBattle, categorySpotArt, categorySpotScience, categorySpotTreaty, categorySpotFunFact, categorySpotDiscovery,categorySpotHumanRights, categorySpotTragedy, categorySpotEvent;
        
        #endregion
        
        // internal cache
        Category[] visibleCategories;

        /// <summary>
        /// Category look up dictionary. Used internally for fast searching of category objects.
        /// </summary>
        Dictionary<Category, int> _categoryLookUp;
        int lastCategoryLookupCount = -1;

        Dictionary<Category, int> categoryLookUp {
            get {
                if (_categoryLookUp != null && categories.Count == lastCategoryLookupCount)
                    return _categoryLookUp;
                if (_categoryLookUp == null) {
                    _categoryLookUp = new Dictionary<Category, int>();
                } else {
                    _categoryLookUp.Clear();
                }
                if (categories != null) {
                    int categoryCount = categories.Count;
                    for (int k = 0; k < categoryCount; k++)
                        _categoryLookUp[categories[k]] = k;
                }
                lastCategoryLookupCount = _categoryLookUp.Count;
                return _categoryLookUp;
            }
        }

        #region System initialization

        //Gli passo la lingua e in base al caso passo i vari file
        public void ReadCategoriesPackedString() {
            //Qua chiamo il file in funzione della lingua settata e della scena
            //ReadCategoriesPackedString(SceneManager.GetActiveScene().name, _setLanguage);
            //Qui metto uno switch in base alla scena
            Debug.Log("LEGGO I FILE delle categorie ");
        }
        
        public void ReadCategoriesPackedString(string periodo, LANGUAGES_OPTIONS lingua) {
            //QUI FACCIO IL COSTRUTTORE
            //string categoryCatalogFileName = _geodataResourcesPath + "/" + filename;
            /*string periodo = "1801-1850";
            string lingua = "Eng";*/
            string filename = "categories" + "_" + periodo + "_" + lingua ;
            string categoryCatalogFileName = _geodataResourcesPath + "/" + periodo + "/" + "Categories" + "/" + filename;
            
            TextAsset ta = Resources.Load<TextAsset>(categoryCatalogFileName);
            if (ta != null) {
                SetCategoryGeoData(ta.text);
                Resources.UnloadAsset(ta);
                ReloadCategoriesAttributes();
            }
        }
        void ReloadCategoriesAttributes()
        {
            TextAsset ta = Resources.Load<TextAsset>(_geodataResourcesPath + "/" + SceneManager.GetActiveScene().name + "/Categories" + _categoryAttributeFile);
            if (ta == null)
                return;
            SetCategoriesAttributes(ta.text);
            Resources.UnloadAsset(ta);
        }


        void ReloadCategoriesttributes() {
            TextAsset ta = Resources.Load<TextAsset>(_geodataResourcesPath + "/" + _categoryAttributeFile);
            if (ta == null)
                return;
            SetCategoriesAttributes(ta.text);
            Resources.UnloadAsset(ta);
        }


        /// <summary>
        /// Reads the categories data from a packed string. Use GetCategoryGeoData method to get the current category geodata information.
        /// </summary>
        public void SetCategoryGeoData(string s) {

            if (_countries == null) {
                Init();
                if (_categories != null)
                    return;
            }
            lastCategoryLookupCount = -1;

            int categoryCount = s.Count('|', 0, s.Length);
            categories = new List<Category>(categoryCount);
            int categoriesCount = 0;
            foreach (StringSpan categorySpan in s.Split('|', 0, s.Length)) {
                IEnumerator<StringSpan> categoryReader = s.Split('$', categorySpan.start, categorySpan.length).GetEnumerator();
                string name = s.ReadString(categoryReader);
                string country = s.ReadString(categoryReader);
                int year = s.ReadInt(categoryReader);
                string description = s.ReadString(categoryReader);
                int countryIndex = GetCountryIndex(country);
                if (countryIndex >= 0) {
                    float x = s.ReadFloat(categoryReader) / MAP_PRECISION;
                    float y = s.ReadFloat(categoryReader) / MAP_PRECISION;
                    float z = s.ReadFloat(categoryReader) / MAP_PRECISION;

                    
                    CATEGORY_CLASS categoryClass = (CATEGORY_CLASS)s.ReadInt(categoryReader);
                    string categoryLink = s.ReadString(categoryReader);
                    
                    //New tag
                    //string tag = s.ReadString(categoryReader);
                    //add tag
                    Category category = new Category(name, countryIndex, description, new Vector3(x, y, z), categoryClass, year, categoryLink);
                    _categories.Add(category);

                    categoriesCount++;
                }
            }
        }

        #endregion

        #region IO stuff

        /// <summary>
        /// Returns the file name corresponding to the current category data file
        /// </summary>
        public int counterLang = 0;
        public string GetCategoryGeoDataFileName() {
            
            //Ritorno il file in INGLESE della scena attuale che sto modificando

            
            string filename = "categories" + "_" + SceneManager.GetActiveScene().name + "_" + LANGUAGES_OPTIONS.ENGLISH + ".txt" ;
            
            return filename;

            //return "categories10.txt";
        }

        /// <summary>
        /// Exports the geographic data in packed string format.
        /// </summary>
        public string GetCategoryGeoData() {
            if (categories == null) return null;
            StringBuilder sb = new StringBuilder();
            int categoriesCount = categories.Count;
            for (int k = 0; k < categoriesCount; k++) {
                Category category = categories[k];
                if (k > 0)
                    sb.Append("|");
                
                sb.Append(category.name);
                sb.Append("$");
                sb.Append(countries[category.countryIndex].name);
                sb.Append("$");
                sb.Append(category.year);
                sb.Append("$");
                sb.Append(category.description);
                sb.Append("$");
                sb.Append((int)(category.localPosition.x * WorldMapGlobe.MAP_PRECISION));
                sb.Append("$");
                sb.Append((int)(category.localPosition.y * WorldMapGlobe.MAP_PRECISION));
                sb.Append("$");
                sb.Append((int)(category.localPosition.z * WorldMapGlobe.MAP_PRECISION));
                sb.Append("$");
                sb.Append((int)category.categoryClass);
                sb.Append("$");
                sb.Append(category.categoryLink);
                
            }
            return sb.ToString();
        }


        #endregion

        #region Drawing stuff

        /// <summary>
        /// Redraws the categories. This is automatically called by Redraw(). Used internally by the Map Editor. You should not need to call this method directly.
        /// </summary>
        ///
        
        
        
         public virtual void DrawCategories(int filterClass) {

            if (!_showCategories || !gameObject.activeInHierarchy || categories == null)
                return;

            // Create categories ALL layer
           Transform t = transform.Find("Categories");
            if (t != null)
                DestroyImmediate(t.gameObject);
            categoriesLayer = new GameObject("Categories");
            categoriesLayer.transform.SetParent(transform, false);
            categoriesLayer.layer = gameObject.layer;
            if (_earthInvertedMode) {
                categoriesLayer.transform.localScale *= 0.99f;
            }

            // Create categoryclass parents
            //ART
            t = transform.Find("ArtLayer");
            if (t != null)
                DestroyImmediate(t.gameObject);
            artLayer = new GameObject("ArtLayer");
            artLayer.hideFlags = HideFlags.DontSave;
            artLayer.transform.SetParent(transform, false);
            artLayer.layer = gameObject.layer;
            
            //BATTLE
            t = transform.Find("BattleLayer");
            if (t != null)
                DestroyImmediate(t.gameObject);
            battleLayer = new GameObject("BattleLayer");
            battleLayer.hideFlags = HideFlags.DontSave;
            battleLayer.transform.SetParent(transform, false);
            battleLayer.layer = gameObject.layer;
            
            //DISCOVERY
            t = transform.Find("DiscoveryLayer");
            if (t != null)
                DestroyImmediate(t.gameObject);
            discoveryLayer = new GameObject("DiscoveryLayer");
            discoveryLayer.hideFlags = HideFlags.DontSave;
            discoveryLayer.transform.SetParent(categoriesLayer.transform, false);
            discoveryLayer.layer = gameObject.layer;
            
            //HUMAN RIGHTS
            t = transform.Find("HumanRightsLayer");
            if (t != null)
                DestroyImmediate(t.gameObject);
            humanRightsLayer = new GameObject("HumanRightsLayer");
            humanRightsLayer.hideFlags = HideFlags.DontSave;
            humanRightsLayer.transform.SetParent(categoriesLayer.transform, false);
            humanRightsLayer.layer = gameObject.layer;
            
            //SCIENCE
            t = transform.Find("ScienceLayer");
            if (t != null)
                DestroyImmediate(t.gameObject);
            scienceLayer = new GameObject("ScienceLayer");
            scienceLayer.hideFlags = HideFlags.DontSave;
            scienceLayer.transform.SetParent(transform, false);
            scienceLayer.layer = gameObject.layer;
            
            //TREATY
            t = transform.Find("TreatyLayer");
            if (t != null)
                DestroyImmediate(t.gameObject);
            treatyLayer = new GameObject("TreatyLayer");
            treatyLayer.hideFlags = HideFlags.DontSave;
            treatyLayer.transform.SetParent(categoriesLayer.transform, false);
            treatyLayer.layer = gameObject.layer;

            //TRAGEDY
            t = transform.Find("TragedyLayer");
            if (t != null)
                DestroyImmediate(t.gameObject);
            tragedyLayer = new GameObject("TragedyLayer");
            tragedyLayer.hideFlags = HideFlags.DontSave;
            tragedyLayer.transform.SetParent(categoriesLayer.transform, false);
            tragedyLayer.layer = gameObject.layer;
            
            //FUN FACT
            t = transform.Find("FunFactLayer");
            if (t != null)
                DestroyImmediate(t.gameObject);
            funFactLayer = new GameObject("FunFactLayer");
            funFactLayer.hideFlags = HideFlags.DontSave;
            funFactLayer.transform.SetParent(categoriesLayer.transform, false);
            funFactLayer.layer = gameObject.layer;
            
            //EVENT
            t = transform.Find("EventLayer");
            if (t != null)
                DestroyImmediate(t.gameObject);
            eventLayer = new GameObject("EventLayer");
            eventLayer.hideFlags = HideFlags.DontSave;
            eventLayer.transform.SetParent(categoriesLayer.transform, false);
            eventLayer.layer = gameObject.layer;
            
            
            bool combineMeshesActive = _combineCategoryMeshes && Application.isPlaying;
            float scale = CategoryScaler.GetScale(this);
            Vector3 categoryScale = new Vector3(scale, scale, 1f);
            
            // Draw category marks
            _numCategoriesDrawn = 0;
            //string minDescription = _minDescription;
            //Aggiungere anno?
            //int filterCat = _filterCategoriesClass;

            //int minYear = _filterCategoriesMinYear;
            //int maxYear = _filterCategoriesMaxYear;
            
            int visibleCount = 0;

            // flip localscale.x to prevent transform issues
            if (_earthInvertedMode) {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }

            int categoryCount = categories.Count;
            int layer = gameObject.layer;
            for (int k = 0; k < categoryCount; k++) {
                Category category = categories[k];
                Country country = countries[category.countryIndex];
                category.isShown = !country.hidden && ((((int)category.categoryClass & _categoryClassAlwaysShow) != 0) || (string.Equals(minDescription,_minDescription) || category.description.Length >= minDescription.Length));
                if (category.isShown) { //Qua aggiungo qualcosa tipo if toggle is off / is on
                    GameObject categoryObj = null, categoryParent = null;
                    
                    switch (category.categoryClass)
                        {
                            case CATEGORY_CLASS.BATTLE:
                                categoryObj = Instantiate(categorySpotBattle);
                                if (!combineMeshesActive)
                                {
                                    categoryObj.GetComponent<Renderer>().sharedMaterial = categoriesBattleMat;
                                }

                                categoryParent = battleLayer;
                                break;
                            case CATEGORY_CLASS.ART:
                                categoryObj = Instantiate(categorySpotArt);
                                if (!combineMeshesActive)
                                {
                                    categoryObj.GetComponent<Renderer>().sharedMaterial = categoriesArtMat;
                                }

                                categoryParent = artLayer;
                                break;
                            case CATEGORY_CLASS.SCIENCE:
                                categoryObj = Instantiate(categorySpotScience);
                                if (!combineMeshesActive)
                                {
                                    categoryObj.GetComponent<Renderer>().sharedMaterial = categoriesScienceMat;
                                }

                                categoryParent = scienceLayer;
                                break;
                            case CATEGORY_CLASS.TREATY:
                                categoryObj = Instantiate(categorySpotTreaty);
                                if (!combineMeshesActive)
                                {
                                    categoryObj.GetComponent<Renderer>().sharedMaterial = categoriesTreatyMat;
                                }

                                categoryParent = treatyLayer;
                                break;
                            case CATEGORY_CLASS.DISCOVERY:
                                categoryObj = Instantiate(categorySpotDiscovery);
                                if (!combineMeshesActive)
                                {
                                    categoryObj.GetComponent<Renderer>().sharedMaterial = categoriesDiscoveryMat;
                                }
                                categoryParent = discoveryLayer;
                                
                                break;

                            case CATEGORY_CLASS.FUNFACT:
                                categoryObj = Instantiate(categorySpotFunFact);
                                if (!combineMeshesActive)
                                {
                                    categoryObj.GetComponent<Renderer>().sharedMaterial = categoriesFunFactMat;
                                }

                                categoryParent = funFactLayer;
                                break;
                            
                            case CATEGORY_CLASS.HUMANRIGHTS:
                                categoryObj = Instantiate(categorySpotHumanRights);
                                if (!combineMeshesActive)
                                {
                                    categoryObj.GetComponent<Renderer>().sharedMaterial = categoriesHumanRightsMat;
                                }
                                categoryParent = humanRightsLayer;
                                
                                break;
                            
                            case CATEGORY_CLASS.TRAGEDY:
                                categoryObj = Instantiate(categorySpotTragedy);
                                if (!combineMeshesActive)
                                {
                                    categoryObj.GetComponent<Renderer>().sharedMaterial = categoriesTragedyMat;
                                }
                                categoryParent = tragedyLayer;
                                break;
                            
                            case CATEGORY_CLASS.EVENT:
                                categoryObj = Instantiate(categorySpotEvent);
                                if (!combineMeshesActive)
                                {
                                    categoryObj.GetComponent<Renderer>().sharedMaterial = categoriesEventMat;
                                }
                                categoryParent = eventLayer;
                                break;

                            
                        }

                        categoryObj.layer = layer;
                        categoryObj.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
                        categoryObj.transform.SetParent(categoryParent.transform, false);
                        categoryObj.transform.localPosition = category.localPosition;
                        categoryObj.transform.localScale = categoryScale;
                        if (_earthInvertedMode)
                        {
                            categoryObj.transform.LookAt(transform.TransformPoint(category.localPosition * 2f));
                        }
                        else
                        {
                            categoryObj.transform.LookAt(transform.position);
                        }

                        category.gameObject = categoryObj;
                        _numCategoriesDrawn++;
                        visibleCount++;
                        
                } else {
                    category.gameObject = null;
                }
            }

            if (_earthInvertedMode) {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }

            // Cache visible categories (this is faster than iterate through the entire collection)
            if (visibleCategories == null || visibleCategories.Length != visibleCount)
                visibleCategories = new Category[visibleCount];

            for (int k = 0; k < categoryCount; k++) {
                Category category = categories[k];
               if (category.isShown) //ANNO
                    visibleCategories[--visibleCount] = category;
            }

            // Toggle categories layer visibility according to settings
            categoriesLayer.SetActive(_showCategories);
            /*
             if(battleLayer!=null)
                battleLayer.SetActive(_showCategories);
            if(artLayer!=null) 
                artLayer.SetActive(_showCategories);
            if(discoveryLayer!=null) 
                discoveryLayer.SetActive(_showCategories);
            if(scienceLayer!=null) 
                scienceLayer.SetActive(_showCategories);
            if(treatyLayer)
                treatyLayer.SetActive(_showCategories);
             if(tragedyLayer)
                tragedyLayer.SetActive(_showCategories);
                */
            
            

            CategoryScaler categoryScaler = categoriesLayer.GetComponent<CategoryScaler>();
            if (categoryScaler == null) {
                categoryScaler = categoriesLayer.AddComponent<CategoryScaler>();
            }
            categoryScaler.map = this;
            categoryScaler.ScaleCategories();

            if (combineMeshesActive) {
                DestroyImmediate(categoryScaler);
                CombineCatMeshes(battleLayer, categoriesBattleMat);
                CombineCatMeshes(artLayer, categoriesArtMat);
                CombineCatMeshes(scienceLayer, categoriesScienceMat);
                CombineCatMeshes(treatyLayer, categoriesTreatyMat);
                CombineCatMeshes(discoveryLayer, categoriesDiscoveryMat);
                CombineCatMeshes(funFactLayer, categoriesFunFactMat);
                CombineCatMeshes(humanRightsLayer, categoriesHumanRightsMat);
                CombineCatMeshes(tragedyLayer, categoriesTragedyMat);
                CombineCatMeshes(eventLayer, categoriesEventMat);
            }
        }

        void CombineCatMeshes(GameObject obj, Material mat) {
            obj.transform.SetParent(null);
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Misc.QuaternionZero;
            obj.transform.position = Vector3.zero;
            MeshFilter[] meshFilters = obj.transform.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length) {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            MeshFilter mf = obj.AddComponent<MeshFilter>();
            mf.mesh = new Mesh();
            mf.sharedMesh.CombineMeshes(combine);
            Renderer renderer = obj.AddComponent<MeshRenderer>();
            renderer.receiveShadows = false;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.sharedMaterial = mat;
            obj.transform.SetParent(categoriesLayer.transform, false);
        }

        void HighlightCategory(int categoryIndex) {
            if (categoryIndex == _categoryHighlightedIndex)
                return;
            _categoryHighlightedIndex = categoryIndex;
            _categoryHighlighted = categories[categoryIndex];

            // Raise event
            if (OnCategoryEnter != null)
                OnCategoryEnter(_categoryHighlightedIndex);
        }

        void HideCategoryHighlight() {
            if (_categoryHighlightedIndex < 0)
                return;

            // Raise event
            if (OnCategoryExit != null)
                OnCategoryExit(_categoryHighlightedIndex);
            _categoryHighlighted = null;
            _categoryHighlightedIndex = -1;
        }

        #endregion

        #region Internal Categories API


        /// <summary>
        /// Returns any city near the point specified in sphere coordinates. If accurate is false, the city must be closer than CITY_HIT_PRECISION constant, usually when pointer clicks over a city icon.
        /// Optimized method for mouse hitting.
        /// </summary>
        /// <returns>The city near point.</returns>
        /// <param name="localPoint">Local point in sphere coordinates.</param>
        
         ////DA MODIFICARE????
        
        public int GetCategoryNearPointFast(Vector3 localPoint) {
            if (visibleCategories == null)
                return -1;
            float hitPrecision = CATEGORIES_HIT_PRECISION * _categoryIconSize * 5.0f;
            float hitPrecisionSqr = hitPrecision * hitPrecision;
            Vector2 latlon = Conversion.GetLatLonFromUnitSpherePoint(localPoint);
            int categoryCount = visibleCategories.Length;
            for (int c = 0; c < countries.Length; c++) {
                Country country = countries[c];
                if (country.regionsRect2D.Contains(latlon)) {
                    for (int t = 0; t < categoryCount; t++) {
                        Category category = visibleCategories[t];
                        if (category.countryIndex != c)
                            continue;
                        float dist = FastVector.SqrDistance(ref category.localPosition, ref localPoint);
                        if (dist < hitPrecisionSqr) {
                            return GetCategoryIndex(category, false);
                        }
                    }
                }
            }
            return -1;
        }
        

        bool GetCategoryUnderMouse(int countryIndex, Vector3 localPoint, out int categoryIndex) {
            categoryIndex = -1;
            if (visibleCategories == null)
                return false;
            float hitPrecision = CATEGORIES_HIT_PRECISION * _categoryIconSize * 24.0f;
            float hitPrecisionSqr = hitPrecision * hitPrecision;
            int categoryCount = visibleCategories.Length;
            for (int c = 0; c < categoryCount; c++) {
                Category category = visibleCategories[c];
                if (category.countryIndex == countryIndex && category.isShown) {
                    if (FastVector.SqrDistance(ref category.localPosition, ref localPoint) < hitPrecisionSqr) {
                        categoryIndex = GetCategoryIndex(category, false);
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Returns the nearest city to the point specified in sphere coordinates.
        /// </summary>
        /// <param name="localPoint">Local point in sphere coordinates.</param>
        public int GetCategoryNearPoint(Vector3 localPoint, int countryIndex = -1) {
            if (visibleCategories == null)
                return -1;
            float minDist = float.MaxValue;
            int categoryCount = visibleCategories.Length;
            int candidate = -1;
            for (int t = 0; t < categoryCount; t++) {
                Category category = visibleCategories[t];
                if (countryIndex >= 0 && category.countryIndex != countryIndex) continue;
                float dist = FastVector.SqrDistance(ref category.localPosition, ref localPoint);
                if (dist < minDist) {
                    minDist = dist;
                    candidate = GetCategoryIndex(category, false);
                }
            }
            return candidate;
        }
        

        /// <summary>
        /// Returns categories belonging to a provided country.
        /// </summary>
        public List<Category> GetCategories(int countryIndex) {
            List<Category> results = new List<Category>(20);
            int categoryCount = categories.Count;
            for (int c = 0; c < categoryCount; c++) {
                if (categories[c].countryIndex == countryIndex)
                    results.Add(categories[c]);
            }
            return results;
        }

        /// <summary>
        /// Returns categories enclosed by a region.
        /// </summary>
        public List<Category> GetCategories(Region region) {
            List<Category> results = new List<Category>(20);
            int categoryCount = categories.Count;
            for (int c = 0; c < categoryCount; c++) {
                if (region.Contains(_categories[c].latlon)) {
                    results.Add(categories[c]);
                }
            }
            return results;
        }

        public List<Category> GetCategoriesWithClass(string Class) {
            List<Category> results = new List<Category>(20);
            int categoryCount = categories.Count;
            for (int c = 0; c < categoryCount; c++) {
                if (categories[c].categoryClass.ToString() == Class) {
                    results.Add(categories[c]);
                }
            }
            return results;
        }

        /// <summary>
        /// Updates the category scale.
        /// </summary>
        public void ScaleCategories() {
            if (categoriesLayer != null) {
                CategoryScaler scaler = categoriesLayer.GetComponent<CategoryScaler>();
                if (scaler != null) {
                    scaler.ScaleCategories();
                } else {
                    DrawCategories(FilterCategoriesClass);
                }
            }
        }


        int GetCategoryCountryRegionIndex(Category category) {
            if (category.regionIndex < 0) {
                int countryIndex = category.countryIndex;
                if (countryIndex < 0 || countryIndex > countries.Length)
                    return -1;

                Country country = countries[countryIndex];
                if (country.regions == null)
                    return -1;
                int regionCount = country.regions.Count;

                float minDist = float.MaxValue;
                for (int cr = 0; cr < regionCount; cr++) {
                    Region region = country.regions[cr];
                    if (region == null || region.spherePoints == null)
                        continue;
                    for (int p = 0; p < region.spherePoints.Length; p++) {
                        float dist = (region.spherePoints[p] - category.localPosition).sqrMagnitude;
                        if (dist < minDist) {
                            minDist = dist;
                            category.regionIndex = cr;
                        }
                    }
                }
            }
            return category.regionIndex;
        }



        #endregion
    }

}