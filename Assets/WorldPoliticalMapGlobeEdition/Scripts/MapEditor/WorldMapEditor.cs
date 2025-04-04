using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace WPM {

    public enum OPERATION_MODE {
        SELECTION = 0,
        RESHAPE = 1,
        CREATE = 2,
        UNDO = 3,
        CONFIRM = 4
    }

    public enum RESHAPE_REGION_TOOL {
        POINT = 0,
        CIRCLE = 1,
        SPLITV = 2,
        SPLITH = 3,
        MAGNET = 4,
        SMOOTH = 5,
        ERASER = 6,
        DELETE = 7
    }

    public enum RESHAPE_CITY_TOOL {
        MOVE = 0,
        DELETE = 1
    }
    
    public enum RESHAPE_CATEGORY_TOOL {
        MOVE = 0,
        DELETE = 1
    }

    public enum RESHAPE_MOUNT_POINT_TOOL {
        MOVE = 0,
        DELETE = 1
    }

    public enum CREATE_TOOL {
        CITY = 0,
        COUNTRY = 1,
        COUNTRY_REGION = 2,
        PROVINCE = 3,
        PROVINCE_REGION = 4,
        MOUNT_POINT = 5,
        CATEGORY = 6
    }


    public enum EDITING_MODE {
        COUNTRIES,
        PROVINCES
    }

    public enum EDITING_COUNTRY_FILE {
        COUNTRY_HIGHDEF = 0,
        COUNTRY_LOWDEF = 1
    }


    public static class ReshapeToolExtensons {
        public static bool hasCircle(this RESHAPE_REGION_TOOL r) {
            return r == RESHAPE_REGION_TOOL.CIRCLE || r == RESHAPE_REGION_TOOL.MAGNET || r == RESHAPE_REGION_TOOL.ERASER;
        }
    }

    [RequireComponent(typeof(WorldMapGlobe))]
    [ExecuteInEditMode]
    public partial class WorldMapEditor : MonoBehaviour {

        public const float HIT_PRECISION = 0.001f;

        public int entityIndex {
            get {
                if (editingMode == EDITING_MODE.PROVINCES)
                    return provinceIndex;
                else
                    return countryIndex;
            }
        }

        public int regionIndex {
            get {
                if (editingMode == EDITING_MODE.PROVINCES)
                    return provinceRegionIndex;
                else
                    return countryRegionIndex;
            }
        }

        public OPERATION_MODE operationMode;
        public RESHAPE_REGION_TOOL reshapeRegionMode;
        public RESHAPE_CITY_TOOL reshapeCityMode;
        public RESHAPE_CATEGORY_TOOL reshapeCategoryMode;
        public RESHAPE_MOUNT_POINT_TOOL reshapeMountPointMode;
        public CREATE_TOOL createMode;
        public Vector3 cursor;
        public bool circleMoveConstant, circleCurrentRegionOnly;
        public float reshapeCircleWidth = 0.01f;
        public bool shouldHideEditorMesh;
        public bool magnetAgressiveMode = false;

        public string infoMsg = "";
        public DateTime infoMsgStartTime;
        public EDITING_MODE editingMode;
        public EDITING_COUNTRY_FILE editingCountryFile;

        public float splitVerticallyAt, splitHorizontallyAt;
        public Rect regionRectLatLon;


        [NonSerialized]
        public List<Region> highlightedRegions;

        List<List<Region>> _undoRegionsList;

        List<List<Region>> undoRegionsList {
            get {
                if (_undoRegionsList == null)
                    _undoRegionsList = new List<List<Region>>();
                return _undoRegionsList;
            }
        }

        public int undoRegionsDummyFlag;

        List<List<City>> _undoCitiesList;

        List<List<City>> undoCitiesList {
            get {
                if (_undoCitiesList == null)
                    _undoCitiesList = new List<List<City>>();
                return _undoCitiesList;
            }
        }

        public int undoCitiesDummyFlag;

        List<List<MountPoint>> _undoMountPointsList;

        List<List<MountPoint>> undoMountPointsList {
            get {
                if (_undoMountPointsList == null)
                    _undoMountPointsList = new List<List<MountPoint>>();
                return _undoMountPointsList;
            }
        }
        

        public int undoMountPointsDummyFlag;
        
        List<List<Category>> _undoCategoriesList;

        List<List<Category>> undoCategoriesList {
            get {
                if (_undoCategoriesList == null)
                    _undoCategoriesList = new List<List<Category>>();
                return _undoCategoriesList;
            }
        }

        public int undoCategoriesDummyFlag;

        public IAdminEntity[] entities {
            get {
                if (editingMode == EDITING_MODE.PROVINCES)
                    return map.provinces;
                else
                    return map.countries;
            }
        }

        public List<Vector3> newShape;
        // for creating new regions

        #region Editor functionality

        WorldMapGlobe _map;

        /// <summary>
        /// Accesor to the World Map Globe core API
        /// </summary>
        public WorldMapGlobe map {
            get {
                if (_map == null)
                    _map = GetComponent<WorldMapGlobe>();
                return _map;
            }
        }
        
        [SerializeField]
        int lastMinPopulation;

        void OnEnable() {
            lastMinPopulation = map.minPopulation;
            map.minPopulation = 0;
        }

        void OnDisable() {
            if (_map != null) {
                if (_map.minPopulation == 0)
                    _map.minPopulation = lastMinPopulation;
            }
        }

        public void ClearSelection() {
            map.HideCountryRegionHighlights(true);
            highlightedRegions = null;
            countryIndex = -1;
            countryRegionIndex = -1;
            GUICountryName = "";
            GUICountryNewName = "";
            GUICountryIndex = -1;
            GUICountryTransferToCountryIndex = -1;
            ClearProvinceSelection();
            ClearCitySelection();
            ClearCategorySelection();
            ClearMountPointSelection();
           
        }

        /// <summary>
        /// Forces a hard redraw on the globe map and calls redraw frontiers again. This is for clearing any artifact.
        /// </summary>
        public void RedrawAll() {
            List<string> deletables = new List<string>(new string[] {
                "Cities",
                "Frontiers",
                "Mount Points",
                "Categories",
                "Cursor",
                "LatitudeLines",
                "LongitudeLines",
                "WPMOverlay",
                "SphereOverlayLayer"
            });
            Transform[] t = _map.GetComponentsInChildren<Transform>();
            for (int k = 0; k < t.Length; k++) {
                if (t[k] == null || t[k].gameObject == null || t[k] == _map.transform)
                    continue;
                if (deletables.Contains(t[k].gameObject.name))
                    DestroyImmediate(t[k].gameObject);
            }
            _map.Redraw();
            RedrawFrontiers();
        }

        /// <summary>
        /// Redraws all frontiers and highlights current selected regions.
        /// </summary>
        public void RedrawFrontiers(bool force = false) {
            RedrawFrontiers(highlightedRegions, true, force);
        }

        /// <summary>
        /// Redraws the frontiers and highlights specified regions filtered by provided list of regions. Also highlights current selected country/province.
        /// </summary>
        /// <param name="filterRegions">Regions.</param>
        /// <param name="highlightSelected">Pass false to just redraw borders.</param>
        public void RedrawFrontiers(List<Region> filterRegions, bool highlightSelected, bool force) {
            map.DestroySurfaces();
            bool needRedraw = force;
            if (!map.RefreshCountryDefinition(countryIndex, filterRegions)) {
                needRedraw = true;
            }
            if (editingMode == EDITING_MODE.PROVINCES) {
                if (!map.RefreshProvinceDefinition(provinceIndex)) {
                    needRedraw = true;
                }
            }
            if (needRedraw) {
                map.OptimizeFrontiers();
                map.Redraw();
            }
            if (highlightSelected) {
                CountryHighlightSelection(filterRegions);
            }
            if (editingMode == EDITING_MODE.PROVINCES) {
                if (highlightSelected) {
                    ProvinceHighlightSelection();
                }
            }
        }

        /// <summary>
        /// Called when a country or province region is selected
        /// </summary>
        void RegionSelected() {
            if (entityIndex < 0 || regionIndex < 0 || entities == null || entityIndex >= entities.Length || entities[entityIndex] == null || regionIndex >= entities[entityIndex].regions.Count)
                return;
            Region region = entities[entityIndex].regions[regionIndex];
            regionRectLatLon = region.latlonRect2D;
            splitVerticallyAt = (regionRectLatLon.yMin + regionRectLatLon.yMax) * 0.5f;
            splitHorizontallyAt = (regionRectLatLon.xMin + regionRectLatLon.xMax) * 0.5f;
        }


        public void DiscardChanges() {
            ClearSelection();
            map.ReloadData(LANGUAGES_OPTIONS.ENGLISH); //Qui sono nell'editor quindi lavoro sui file BASE -> in inglese
            RedrawFrontiers();
            cityChanges = false;
            countryChanges = false;
            provinceChanges = false;
            lastCityCount = -1;
            lastCountryCount = -1;
            lastProvinceCount = -1;
            shouldHideEditorMesh = true;
        }

        /// <summary>
        /// Moves any point inside circle.
        /// </summary>
        /// <returns>Returns a list with changed regions</returns>
        public List<Region> MoveCircle(Vector3 position, Vector3 dragAmount, float circleSize, bool circleMoveConstant) {
            if (entityIndex < 0 || entityIndex >= entities.Length)
                return null;

            float circleSizeSqr = circleSize * circleSize;
            List<Region> regions = new List<Region>(100);
            // Current region
            Region currentRegion = entities[entityIndex].regions[regionIndex];
            regions.Add(currentRegion);
            // Current region's neighbours
            if (!circleCurrentRegionOnly) {
                for (int r = 0; r < currentRegion.neighbours.Count; r++) {
                    Region region = currentRegion.neighbours[r];
                    if (!regions.Contains(region))
                        regions.Add(region);
                }
                // If we're editing provinces, check if country points can be moved as well
                if (editingMode == EDITING_MODE.PROVINCES) {
                    // Moves current country
                    for (int cr = 0; cr < map.countries[countryIndex].regions.Count; cr++) {
                        Region countryRegion = map.countries[countryIndex].regions[cr];
                        if (!regions.Contains(countryRegion))
                            regions.Add(countryRegion);
                        // Moves neighbours
                        for (int r = 0; r < countryRegion.neighbours.Count; r++) {
                            Region region = countryRegion.neighbours[r];
                            if (!regions.Contains(region))
                                regions.Add(region);
                        }
                    }
                }
            }
            // Execute move operation on each point
            List<Region> affectedRegions = new List<Region>(regions.Count);
            for (int r = 0; r < regions.Count; r++) {
                Region region = regions[r];
                bool regionAffected = false;
                for (int p = 0; p < region.spherePoints.Length; p++) {
                    Vector3 rp = region.spherePoints[p];
                    float dist = (rp - position).sqrMagnitude;
                    if (dist < circleSizeSqr) {
                        if (circleMoveConstant) {
                            region.spherePoints[p] += dragAmount;
                        } else {
                            region.spherePoints[p] += dragAmount - dragAmount * (dist / circleSizeSqr);
                        }
                        region.spherePoints[p] = region.spherePoints[p].normalized * 0.5f;
                        Vector2 latlon = Conversion.GetLatLonFromSpherePoint(region.spherePoints[p]);
                        latlon = region.AdjustLongitudeBeyond180(latlon);
                        region.latlon[p] = latlon;
                        regionAffected = true;
                    }
                }
                if (regionAffected && !affectedRegions.Contains(region)) {
                    affectedRegions.Add(region);
                    // add also neighbours
                    for (int k = 0; k < region.neighbours.Count; k++) {
                        if (!affectedRegions.Contains(region.neighbours[k])) {
                            affectedRegions.Add(region.neighbours[k]);
                        }
                    }
                }
            }
            return affectedRegions;
        }


        /// <summary>
        /// Moves a single point.
        /// </summary>
        /// <returns>Returns a list of affected regions</returns>
        public List<Region> MovePoint(Vector3 position, Vector3 dragAmount) {
            return MoveCircle(position, dragAmount, 0.00001f, true);
        }

        /// <summary>
        /// Moves points of other regions towards current frontier
        /// </summary>
        public bool Magnet(Vector3 position, float circleSize) {
            if (entityIndex < 0 || entityIndex >= entities.Length)
                return false;

            Region currentRegion = entities[entityIndex].regions[regionIndex];
            float circleSizeSqr = circleSize * circleSize;

            Dictionary<Vector3, bool> attractorsUse = new Dictionary<Vector3, bool>();
            // Attract points of other regions/countries
            List<Region> regions = new List<Region>();
            for (int c = 0; c < entities.Length; c++) {
                IAdminEntity entity = entities[c];
                if (entity.regions == null)
                    continue;
                for (int r = 0; r < entity.regions.Count; r++) {
                    if (c != entityIndex || r != regionIndex) {
                        regions.Add(entities[c].regions[r]);
                    }
                }
            }
            if (editingMode == EDITING_MODE.PROVINCES) {
                // Also add regions of current country and neighbours
                for (int r = 0; r < map.countries[countryIndex].regions.Count; r++) {
                    Region region = map.countries[countryIndex].regions[r];
                    regions.Add(region);
                    for (int n = 0; n < region.neighbours.Count; n++) {
                        Region nregion = region.neighbours[n];
                        if (!regions.Contains(nregion))
                            regions.Add(nregion);
                    }
                }
            }

            bool changes = false;
            Vector3 goodAttractor = Misc.Vector3zero;

            for (int r = 0; r < regions.Count; r++) {
                Region region = regions[r];
                bool changesInThisRegion = false;
                for (int p = 0; p < region.spherePoints.Length; p++) {
                    Vector3 rp = region.spherePoints[p];
                    float dist = (rp - position).sqrMagnitude;
                    if (dist < circleSizeSqr) {
                        float minDist = float.MaxValue;
                        int nearest = -1;
                        for (int a = 0; a < currentRegion.spherePoints.Length; a++) {
                            Vector3 attractor = currentRegion.spherePoints[a];
                            dist = (rp - attractor).sqrMagnitude;
                            if (dist < circleSizeSqr && dist < minDist) {
                                minDist = dist;
                                nearest = a;
                                goodAttractor = attractor;
                            }
                        }
                        if (nearest >= 0) {
                            changes = true;
                            // Check if this attractor is being used by other point
                            bool used = attractorsUse.ContainsKey(goodAttractor);
                            if (!used || magnetAgressiveMode) {
                                region.spherePoints[p] = goodAttractor;
                                if (!used)
                                    attractorsUse.Add(goodAttractor, true);
                                changesInThisRegion = true;
                            }
                        }
                    }
                }
                if (changesInThisRegion) {
                    // Remove duplicate points in this region
                    Dictionary<Vector3, bool> repeated = new Dictionary<Vector3, bool>();
                    for (int k = 0; k < region.spherePoints.Length; k++)
                        if (!repeated.ContainsKey(region.spherePoints[k]))
                            repeated.Add(region.spherePoints[k], true);
                    region.spherePoints = new List<Vector3>(repeated.Keys).ToArray();
                }
            }
            return changes;
        }


        /// <summary>
        /// Erase points inside circle.
        /// </summary>
        public bool Erase(Vector3 position, float circleSize) {
            if (entityIndex < 0 || entityIndex >= entities.Length)
                return false;

            float circleSizeSqr = circleSize * circleSize;
            List<Region> regions = new List<Region>(100);

            // Current region
            Region currentRegion = entities[entityIndex].regions[regionIndex];
            if (currentRegion.spherePoints.Length <= 3)
                return false;

            regions.Add(currentRegion);
            // Current region's neighbours
            if (!circleCurrentRegionOnly) {
                for (int r = 0; r < currentRegion.neighbours.Count; r++) {
                    Region region = currentRegion.neighbours[r];
                    if (!regions.Contains(region))
                        regions.Add(region);
                }
                // If we're editing provinces, check if country points can be deleted as well
                if (editingMode == EDITING_MODE.PROVINCES) {
                    // Current country
                    for (int cr = 0; cr < map.countries[countryIndex].regions.Count; cr++) {
                        Region countryRegion = map.countries[countryIndex].regions[cr];
                        if (!regions.Contains(countryRegion))
                            regions.Add(countryRegion);
                        // Neighbours
                        for (int r = 0; r < countryRegion.neighbours.Count; r++) {
                            Region region = countryRegion.neighbours[r];
                            if (!regions.Contains(region))
                                regions.Add(region);
                        }
                    }
                }
            }
            // Execute delete operation on each point
            List<Vector3> temp = new List<Vector3>(currentRegion.spherePoints.Length);
            bool someChange = false;
            for (int r = 0; r < regions.Count; r++) {
                Region region = regions[r];
                temp.Clear();
                bool changes = false;
                for (int p = 0; p < region.spherePoints.Length; p++) {
                    Vector3 rp = region.spherePoints[p];
                    float dist = (rp - position).sqrMagnitude;
                    if (dist > circleSizeSqr) {
                        temp.Add(rp);
                    } else {
                        changes = true;
                    }
                }
                if (changes) {
                    Vector3[] newPoints = temp.ToArray();
                    if (newPoints.Length >= 5) {
                        region.spherePoints = newPoints;
                        someChange = true;
                    } else {
                        SetInfoMsg("Can't delete more points. To delete it completely use the DELETE tool.");
                    }
                }
            }
            return someChange;
        }


        public void UndoRegionsPush(List<Region> regions) {
            UndoRegionsInsertAtCurrentPos(regions);
            undoRegionsDummyFlag++;
            if (editingMode == EDITING_MODE.COUNTRIES) {
                countryChanges = true;
            } else
                provinceChanges = true;
        }

        public void UndoRegionsInsertAtCurrentPos(List<Region> regions) {
            if (regions == null)
                return;
            List<Region> clonedRegions = new List<Region>();
            int rcount = regions.Count;
            for (int k = 0; k < rcount; k++) {
                clonedRegions.Add(regions[k].Clone());
            }
            if (undoRegionsDummyFlag > undoRegionsList.Count)
                undoRegionsDummyFlag = undoRegionsList.Count;
            undoRegionsList.Insert(undoRegionsDummyFlag, clonedRegions);
        }

        public void UndoCitiesPush() {
            UndoCitiesInsertAtCurrentPos();
            undoCitiesDummyFlag++;
        }

        public void UndoCitiesInsertAtCurrentPos() {
            int cityCount = map.cities.Count;
            List<City> cities = new List<City>(cityCount);
            for (int k = 0; k < cityCount; k++)
                cities.Add(map.cities[k].Clone());
            if (undoCitiesDummyFlag > undoCitiesList.Count)
                undoCitiesDummyFlag = undoCitiesList.Count;
            undoCitiesList.Insert(undoCitiesDummyFlag, cities);
        }
        
        //MODIFICA
        public void UndoCategoriesPush() {
            UndoCategoriesInsertAtCurrentPos();
            undoCategoriesDummyFlag++;
        }

        public void UndoCategoriesInsertAtCurrentPos() {
            int categoriesCount = map.categories.Count;
            List<Category> categories = new List<Category>(categoriesCount);
            for (int k = 0; k < categoriesCount; k++)
                categories.Add(map.categories[k].Clone());
            if (undoCategoriesDummyFlag > undoCategoriesList.Count)
                undoCategoriesDummyFlag = undoCategoriesList.Count;
            undoCategoriesList.Insert(undoCategoriesDummyFlag, categories);
        }
        //
        public void UndoMountPointsPush() {
            UndoMountPointsInsertAtCurrentPos();
            undoMountPointsDummyFlag++;
        }

        public void UndoMountPointsInsertAtCurrentPos() {
            if (map.mountPoints == null)
                map.mountPoints = new List<MountPoint>();
            List<MountPoint> mountPoints = new List<MountPoint>(map.mountPoints.Count);
            for (int k = 0; k < map.mountPoints.Count; k++)
                mountPoints.Add(map.mountPoints[k].Clone());
            if (undoMountPointsDummyFlag > undoMountPointsList.Count)
                undoMountPointsDummyFlag = undoMountPointsList.Count;
            undoMountPointsList.Insert(undoMountPointsDummyFlag, mountPoints);
        }


        public void UndoHandle() {
            if (undoRegionsList != null && undoRegionsList.Count >= 2) {
                if (undoRegionsDummyFlag >= undoRegionsList.Count) {
                    undoRegionsDummyFlag = undoRegionsList.Count - 2;
                }
                List<Region> savedRegions = undoRegionsList[undoRegionsDummyFlag];
                RestoreRegions(savedRegions);
            }
            if (undoCitiesList != null && undoCitiesList.Count >= 2) {
                if (undoCitiesDummyFlag >= undoCitiesList.Count) {
                    undoCitiesDummyFlag = undoCitiesList.Count - 2;
                }
                List<City> savedCities = undoCitiesList[undoCitiesDummyFlag];
                RestoreCities(savedCities);
            }
            if (undoMountPointsList != null && undoMountPointsList.Count >= 2) {
                if (undoMountPointsDummyFlag >= undoMountPointsList.Count) {
                    undoMountPointsDummyFlag = undoMountPointsList.Count - 2;
                }
                List<MountPoint> savedMountPoints = undoMountPointsList[undoMountPointsDummyFlag];
                RestoreMountPoints(savedMountPoints);
            }
        }


        void RestoreRegions(List<Region> savedRegions) {
            for (int k = 0; k < savedRegions.Count; k++) {
                IAdminEntity entity = savedRegions[k].entity;
                int regionIndex = savedRegions[k].regionIndex;
                entity.regions[regionIndex] = savedRegions[k];
            }
            RedrawFrontiers(true);
        }

        void RestoreCities(List<City> savedCities) {
            map.cities = savedCities;
            lastCityCount = -1;
            ReloadCityNames();
            map.DrawCities();
        }


        void RestoreMountPoints(List<MountPoint> savedMountPoints) {
            map.mountPoints = savedMountPoints;
            lastMountPointCount = -1;
            ReloadMountPointNames();
            map.DrawMountPoints();
        }

        int EntityAdd(IAdminEntity newEntity) {
            if (newEntity is Country) {
                Country c = (Country)newEntity;
                if (map.CountryAdd(c) >= 0)
                    return map.GetCountryIndex(c);
            } else {
                Province p = (Province)newEntity;
                if (map.ProvinceAdd(p))
                    return map.GetProvinceIndex(p);
            }
            return -1;
        }

        void SplitCities(int sourceCountryIndex, Region regionOtherCountry) {
            int cityCount = map.cities.Count;
            int targetCountryIndex = map.GetCountryIndex((Country)regionOtherCountry.entity);
            for (int k = 0; k < cityCount; k++) {
                City city = map.cities[k];
                if (city.countryIndex == sourceCountryIndex && regionOtherCountry.Contains(city.latlon)) {
                    city.countryIndex = targetCountryIndex;
                    cityChanges = true;
                }
            }
        }

        public void SplitHorizontally() {
            if (entityIndex < 0 || entityIndex >= entities.Length)
                return;

            IAdminEntity currentEntity = entities[entityIndex];
            Region currentRegion = currentEntity.regions[regionIndex];

            List<Vector2> half1 = new List<Vector2>();
            List<Vector2> half2 = new List<Vector2>();
            int prevSide = 0;
            Vector2 q = currentRegion.latlon[0];
            for (int k = 0; k < currentRegion.latlon.Length; k++) {
                Vector2 p = currentRegion.latlon[k];
                if (p.x > splitHorizontallyAt) {
                    if (prevSide == -1) {
                        float t = (splitHorizontallyAt - q.x) / (p.x - q.x);
                        Vector2 r = Vector2.Lerp(q, p, t);
                        half1.Add(r);
                        half2.Add(r);
                    }
                    half1.Add(p);
                    q = p;
                    prevSide = 1;
                } else {
                    if (prevSide == 1) {
                        float t = (splitHorizontallyAt - q.x) / (p.x - q.x);
                        Vector2 r = Vector2.Lerp(q, p, t);
                        half1.Add(r);
                        half2.Add(r);
                    }
                    half2.Add(p);
                    q = p;
                    prevSide = -1;
                }
            }
            // Setup new entity
            IAdminEntity newEntity;
            if (currentEntity is Country) {
                string name = map.GetCountryUniqueName("New " + currentEntity.name); 
                string domain = name;
                newEntity = new Country(name, ((Country)currentEntity).continent , ((Country)currentEntity).domain);
            } else {
                string name = map.GetProvinceUniqueName("New " + currentEntity.name);
                newEntity = new Province(name, ((Province)currentEntity).countryIndex);
                newEntity.regions = new List<Region>();
            }

            // Update polygons
            Region newRegion = new Region(newEntity, 0);
            if (entities[countryIndex].latlonCenter.x > splitHorizontallyAt) {
                currentRegion.latlon = half1.ToArray();
                newRegion.latlon = half2.ToArray();
            } else {
                currentRegion.latlon = half2.ToArray();
                newRegion.latlon = half1.ToArray();
            }
            map.RegionSanitize(currentRegion);
            map.RegionSmooth(currentRegion, 1.5f);
            map.RegionSanitize(newRegion);
            map.RegionSmooth(newRegion, 1.5f);

            newEntity.regions.Add(newRegion);
            int newEntityIndex = EntityAdd(newEntity);

            // Refresh old entity and selects the new
            if (currentEntity is Country) {
                map.RefreshCountryDefinition(newEntityIndex, null);
                map.RefreshCountryDefinition(countryIndex, null);
                SplitCities(countryIndex, newRegion);
                ClearSelection();
                RedrawFrontiers();
                countryIndex = newEntityIndex;
                countryRegionIndex = 0;
                CountryRegionSelect();
                countryChanges = true;
                cityChanges = true;
            } else {
                map.RefreshProvinceDefinition(newEntityIndex);
                map.RefreshProvinceDefinition(provinceIndex);
                highlightedRegions = null;
                ClearSelection();
                RedrawFrontiers();
                provinceIndex = newEntityIndex;
                provinceRegionIndex = 0;
                countryIndex = map.provinces[provinceIndex].countryIndex;
                countryRegionIndex = map.countries[countryIndex].mainRegionIndex;
                CountryRegionSelect();
                ProvinceRegionSelect();
                provinceChanges = true;
            }
            map.RedrawMapLabels();
        }

        public void SplitVertically() {
            if (entityIndex < 0 || entityIndex >= entities.Length)
                return;

            IAdminEntity currentEntity = entities[entityIndex];
            Region currentRegion = currentEntity.regions[regionIndex];

            List<Vector2> half1 = new List<Vector2>();
            List<Vector2> half2 = new List<Vector2>();
            int prevSide = 0;
            Vector2 q = currentRegion.latlon[0];
            for (int k = 0; k < currentRegion.latlon.Length; k++) {
                Vector2 p = currentRegion.latlon[k];
                if (p.y > splitVerticallyAt) { // compare longitudes
                    if (prevSide == -1) {
                        float t = (splitVerticallyAt - q.y) / (p.y - q.y);
                        Vector2 r = Vector2.Lerp(q, p, t);
                        half1.Add(r);
                        half2.Add(r);
                    }
                    half1.Add(p);
                    q = p;
                    prevSide = 1;
                } else {
                    if (prevSide == 1) {
                        float t = (splitVerticallyAt - q.y) / (p.y - q.y);
                        Vector2 r = Vector2.Lerp(q, p, t);
                        half1.Add(r);
                        half2.Add(r);
                    }
                    half2.Add(p);
                    q = p;
                    prevSide = -1;
                }
            }
            // Setup new entity
            IAdminEntity newEntity;
            if (currentEntity is Country) {
                string name = map.GetCountryUniqueName("New " + currentEntity.name);
                //string sovereign = name;
                newEntity = new Country(name, ((Country)currentEntity).continent , ((Country)currentEntity).domain);
            } else {
                string name = map.GetProvinceUniqueName("New " + currentEntity.name);
                newEntity = new Province(name, ((Province)currentEntity).countryIndex);
                newEntity.regions = new List<Region>();
            }

            // Update polygons
            Region newRegion = new Region(newEntity, 0);
            if (entities[countryIndex].latlonCenter.y > splitVerticallyAt) {
                currentRegion.latlon = half1.ToArray();
                newRegion.latlon = half2.ToArray();
            } else {
                currentRegion.latlon = half2.ToArray();
                newRegion.latlon = half1.ToArray();
            }
            map.RegionSanitize(currentRegion);
            map.RegionSmooth(currentRegion, 1.5f);
            map.RegionSanitize(newRegion);
            map.RegionSmooth(newRegion, 1.5f);

            newEntity.regions.Add(newRegion);
            int newEntityIndex = EntityAdd(newEntity);

            // Refresh old entity and selects the new
            if (currentEntity is Country) {
                map.RefreshCountryDefinition(newEntityIndex, null);
                map.RefreshCountryDefinition(countryIndex, null);
                SplitCities(countryIndex, newRegion);
                ClearSelection();
                RedrawFrontiers();
                countryIndex = newEntityIndex;
                countryRegionIndex = 0;
                CountryRegionSelect();
                countryChanges = true;
                cityChanges = true;
            } else {
                map.RefreshProvinceDefinition(newEntityIndex);
                map.RefreshProvinceDefinition(provinceIndex);
                highlightedRegions = null;
                ClearSelection();
                RedrawFrontiers();
                provinceIndex = newEntityIndex;
                provinceRegionIndex = 0;
                countryIndex = map.provinces[provinceIndex].countryIndex;
                countryRegionIndex = map.countries[countryIndex].mainRegionIndex;
                CountryRegionSelect();
                ProvinceRegionSelect();
                provinceChanges = true;
            }
            map.RedrawMapLabels();
        }

        public void AddPointToShape(Vector3 newPoint) {
            int pointCount = newShape.Count;
            for (int k=0;k<pointCount;k++) {
                Vector3 pos = newShape[k];
                if (Vector3.Distance(newPoint, pos) < HIT_PRECISION) {
                    Debug.Log("Point too near to other point in shape.");
                    return;
                }
            }
            newShape.Add(newPoint);
        }

        /// <summary>
        /// Adds the new point to currently selected region.
        /// </summary>
        public void AddPoint(Vector3 newPoint) {
            if (entities == null || entityIndex < 0 || entityIndex >= entities.Length || regionIndex < 0 || entities[entityIndex].regions == null || regionIndex >= entities[entityIndex].regions.Count)
                return;
            //			List<Region> affectedRegions = new List<Region>();
            Region region = entities[entityIndex].regions[regionIndex];
            float minDist = float.MaxValue;
            int nearest = -1, previous = -1;
            int max = region.latlon.Length;
            Vector2 latlonNew = Conversion.GetLatLonFromSpherePoint(newPoint);
            latlonNew = region.AdjustLongitudeBeyond180(latlonNew);
            for (int p = 0; p < max; p++) {
                int q = p == 0 ? max - 1 : p - 1;
                Vector2 rp = (region.latlon[p] + region.latlon[q]) * 0.5f;
                float dist = (rp - latlonNew).sqrMagnitude; // (rp.x - newPoint.x) * (rp.x - newPoint.x)*4  + (rp.y - newPoint.y) * (rp.y - newPoint.y);
                if (dist < minDist) {
                    // Get nearest point
                    minDist = dist;
                    nearest = p;
                    previous = q;
                }
            }

            if (nearest >= 0) {
                Vector2 latlonToInsert = (region.latlon[nearest] + region.latlon[previous]) * 0.5f;

                // Check if nearest and previous exists in any neighbour
                int nearest2 = -1, previous2 = -1;
                for (int n = 0; n < region.neighbours.Count; n++) {
                    Region nregion = region.neighbours[n];
                    for (int p = 0; p < nregion.latlon.Length; p++) {
                        if (nregion.latlon[p] == region.latlon[nearest]) {
                            nearest2 = p;
                        }
                        if (nregion.latlon[p] == region.latlon[previous]) {
                            previous2 = p;
                        }
                    }
                    if (nearest2 >= 0 && previous2 >= 0) {
                        nregion.latlon = InsertLatLon(nregion.latlon, previous2, latlonToInsert);
                        //						affectedRegions.Add (nregion);
                        break;
                    }
                }

                // Insert the point in the current region (must be done after inserting in the neighbour so nearest/previous don't unsync)
                region.latlon = InsertLatLon(region.latlon, nearest, latlonToInsert);
                //				affectedRegions.Add (region);
            }
        }

        Vector2[] InsertLatLon(Vector2[] pointArray, int index, Vector2 latlonToInsert) {
            List<Vector2> temp = new List<Vector2>(pointArray.Length + 1);
            for (int k = 0; k < pointArray.Length; k++) {
                if (k == index)
                    temp.Add(latlonToInsert);
                temp.Add(pointArray[k]);
            }
            return temp.ToArray();
        }

        public void SetInfoMsg(string msg) {
            this.infoMsg = msg;
            infoMsgStartTime = DateTime.Now;
        }

        public bool GetVertexNearSpherePos(Vector3 spherePos, out Vector3 nearPoint, bool mustBeDifferent) {
            // Iterate country regions

            float requiredMinDist = mustBeDifferent ? 0.00001f : 0;

            int numCountries = _map.countries.Length;
            Vector3 np = spherePos;
            float minDist = float.MaxValue;
            // Countries
            for (int c = 0; c < numCountries; c++) {
                Country country = _map.countries[c];
                int regCount = country.regions.Count;
                for (int cr = 0; cr < regCount; cr++) {
                    Region region = country.regions[cr];
                    int pointCount = region.spherePoints.Length;
                    for (int p = 0; p < pointCount; p++) {
                        float dist = (spherePos - region.spherePoints[p]).sqrMagnitude;
                        if (dist < minDist && dist >= requiredMinDist) {
                            // Check that it's not already in the new shape
                            if (!newShape.Contains(region.spherePoints[p])) {
                                minDist = dist;
                                np = region.spherePoints[p];
                            }
                        }
                    }
                }
            }
            // Provinces
            if (_map.editor.editingMode == EDITING_MODE.PROVINCES) {
                int numProvinces = _map.provinces.Length;
                for (int p = 0; p < numProvinces; p++) {
                    Province province = _map.provinces[p];
                    if (province.regions == null)
                        continue;
                    int regCount = province.regions.Count;
                    for (int pr = 0; pr < regCount; pr++) {
                        Region region = province.regions[pr];
                        int pointCount = region.spherePoints.Length;
                        for (int po = 0; po < pointCount; po++) {
                            float dist = (spherePos - region.spherePoints[po]).sqrMagnitude;
                            if (dist < minDist && dist >= requiredMinDist) {
                                // Check that it's not already in the new shape
                                if (!newShape.Contains(region.spherePoints[p])) {
                                    minDist = dist;
                                    np = region.spherePoints[p];
                                }
                            }
                        }
                    }
                }
            }

            nearPoint = np;
            return nearPoint != spherePos;
        }

        #endregion

        #region Tools

        /// <summary>
        /// Generates two text files at the root of the Unity project containing the list of countries and provinces
        /// </summary>
        public void ExportEntitiesToFile() {
            FRONTIERS_DETAIL prev = map.frontiersDetail;
            map.frontiersDetail = FRONTIERS_DETAIL.Low;
            ExportEntitiesToFile(Application.dataPath + "/Entities Low Definition.txt");
            map.frontiersDetail = FRONTIERS_DETAIL.High;
            ExportEntitiesToFile(Application.dataPath + "/Entities High Definition.txt");
            map.frontiersDetail = prev;
            ExportCitiesToFile(Application.dataPath + "/Cities.txt");
        }

        void ExportEntitiesToFile(string filename) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Country\tCountry Index\tProvince\tProvince Index");
            for (int k = 0; k < map.countries.Length; k++) {
                Country c = map.countries[k];
                sb.Append(c.name);
                sb.Append("\t");
                sb.AppendLine(k.ToString());
                if (c.provinces == null)
                    continue;
                for (int p = 0; p < c.provinces.Length; p++) {
                    sb.Append("\t");
                    sb.Append("\t");
                    sb.Append(c.provinces[p].name);
                    sb.Append("\t");
                    sb.AppendLine(map.GetProvinceIndex(c.provinces[p]).ToString());
                }
            }
            System.IO.File.WriteAllText(filename, sb.ToString(), Encoding.UTF8);
        }

        void ExportCitiesToFile(string filename) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Country\tProvince\tCity\tCity Index");
            int cityCount = map.cities.Count;
            for (int k = 0; k < cityCount; k++) {
                City city = map.cities[k];
                sb.Append(map.countries[city.countryIndex].name);
                sb.Append("\t");
                sb.Append(city.province);
                sb.Append("\t");
                sb.Append(city.name);
                sb.Append("\t");
                sb.AppendLine(k.ToString());
            }
            System.IO.File.WriteAllText(filename, sb.ToString(), Encoding.UTF8);
        }


        #endregion

    }
}
