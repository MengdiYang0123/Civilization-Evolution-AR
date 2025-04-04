using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace WPM {
    public partial class WorldMapEditor : MonoBehaviour {

        public int GUICategoryIndex;
        public string GUICategoryName = "";
        public string GUICategoryNewName = "";
        public string GUICategoryYear;
        public string GUICategoryDescription = "";
        public string GUICategoryNewDescription = "";
        public string GUICategoryLink = "";
        public string GUICategoryNewLink = "";
        //New tag
        //public string GUICategoryTag = "";
        //public string GUICategoryNewTag = "";

        public Vector2 GUICategoryLatLon;
        public CATEGORY_CLASS GUICategoryClass = CATEGORY_CLASS.BATTLE;
        public int GUIFilterCategoryClass = -1;
        public int GUIFilterCategoryMinYear = -9999;
        public int GUIFilterCategoryMaxYear = 3000;
        public int categoryIndex = -1;
        public int descriptionIndex = -1;
        public bool categoryChanges;
        // if there's any pending change to be saved
        public bool categoryAttribChanges;
        
        // private fields
        int lastCategoryCount = -1;
        string[] _categoryNames;

        //New tag
        //private string[] _categoryTags;
        
        string[] _categoryDescriptions;

        public string[] categoryNames {
            get {
                if (map.categories != null && lastCategoryCount != map.categories.Count) {
                    categoryIndex = -1;
                    ReloadCategoryNames();
                }
                return _categoryNames;
            }
        }


        #region Editor functionality


        public void ClearCategorySelection() {
            map.HideCategoryHighlights();
            categoryIndex = -1;
            GUICategoryName = "";
            GUICategoryIndex = -1;
            GUICategoryNewName = "";
            GUICategoryDescription = "ND";
            GUICategoryNewDescription = "";
            //New tag
            //GUICategoryTag = "";
            //GUICategoryNewTag = "";
        }


        /// <summary>
        /// Adds a new category to current country.
        /// </summary>
        public void CategoryCreate(Vector3 newPoint) {
            if (countryIndex < 0)
                return;
            GUICategoryName = "New Category " + (map.categories.Count + 1);
            newPoint = newPoint.normalized * 0.5f;
            //Add , GUICategoryTag
            Category newCategory = new Category(GUICategoryName, countryIndex, GUICategoryDescription, newPoint, GUICategoryClass, -1, GUICategoryLink);
            map.categories.Add(newCategory);
            map.DrawCategories(GUIFilterCategoryClass);
            lastCategoryCount = -1;
            ReloadCategoryNames();
            categoryChanges = true;
        }

        public bool CategoryUpdate() {
            if (categoryIndex < 0)
                return false;

            bool changes = false;
            Category category = map.categories[categoryIndex];
            
            if (category.categoryClass != GUICategoryClass) {
                category.categoryClass = GUICategoryClass;
                changes = true;
            }
           
            string prevDescription = category.description;
            GUICategoryNewDescription = GUICategoryNewDescription.Trim();
            if (!prevDescription.Equals(GUICategoryNewDescription)) {
                category.description = GUICategoryNewDescription;
                GUICategoryDescription = GUICategoryNewDescription;
                changes = true;
            }
            
            string prevLink = category.categoryLink;
            GUICategoryNewLink = GUICategoryNewLink.Trim();
            if (!prevLink.Equals(GUICategoryNewLink)) {
                category.categoryLink = GUICategoryNewLink;
                GUICategoryLink = GUICategoryNewLink;
                changes = true;
            }
            
            if (category.latlon != GUICategoryLatLon) {
                category.latlon = GUICategoryLatLon;
                changes = true;
            }

            int newYear;
            if (int.TryParse(GUICategoryYear, out newYear) && category.year != newYear) {
                category.year = newYear;
                changes = true;
            }
            

            string prevName = category.name;
            GUICategoryNewName = GUICategoryNewName.Trim();
            if (!prevName.Equals(GUICategoryNewName)) {
                category.name = GUICategoryNewName;
                GUICategoryName = GUICategoryNewName;
                lastCategoryCount = -1;
                ReloadCategoryNames();
                changes = true;
            }

            //New tag
            /*string prevTag = category.tag;
            GUICategoryNewTag = GUICategoryNewTag.Trim();
            if (!prevName.Equals(GUICategoryNewTag)) {
                category.tag = GUICategoryNewTag;
                GUICategoryTag = GUICategoryNewTag;
                lastCategoryCount = -1;
                ReloadCategoryTags();
                changes = true;
            }*/

            if (changes) {
                map.DrawCategories(GUIFilterCategoryClass);
                categoryChanges = true;
            }
            return true;
        }


        public void CategoryMove(Vector3 destination) {
            if (categoryIndex < 0)
                return;
            map.categories[categoryIndex].localPosition = destination.normalized * 0.5f;
            UpdateCategoryVisualPosition();
            categoryChanges = true;
        }

        void UpdateCategoryVisualPosition() {
            GameObject categoryObj = map.categories[categoryIndex].gameObject;
            if (categoryObj != null) {
                categoryObj.transform.localPosition = map.categories[categoryIndex].localPosition * 1.001f;
            }
        }

        public void CategorySelectByCombo(int selection) {
            GUICategoryName = "";
            GUICategoryIndex = selection;
            if (GetCategoryIndexByGUISelection()) {
                if (Application.isPlaying) {
                    map.BlinkCategory(categoryIndex, Color.grey, Color.yellow, 1.2f, 0.2f);
                }
            }
            CategorySelect();
        }

        bool GetCategoryIndexByGUISelection() {
            if (GUICategoryIndex < 0 || GUICategoryIndex >= categoryNames.Length)
                return false;
            string[] s = categoryNames[GUICategoryIndex].Split(new char[] {
                '(',
                ')'
            }, System.StringSplitOptions.RemoveEmptyEntries);
            if (s.Length >= 2) {
                GUICategoryName = s[0].Trim();
                if (int.TryParse(s[1], out categoryIndex)) {
                    return true;
                }
            }
            return false;
        }

        public void CategorySelect() {
            if (categoryIndex < 0 || categoryIndex > map.categories.Count)
                return;

            // If no country is selected (the category could be at sea) select it
            Category category = map.categories[categoryIndex];
            int categoryCountryIndex = category.countryIndex;
            if (categoryCountryIndex < 0) {
                SetInfoMsg("Country not found in this country file.");
            }

            if (countryIndex != categoryCountryIndex && categoryCountryIndex >= 0) {
                ClearSelection();
                countryIndex = categoryCountryIndex;
                countryRegionIndex = map.countries[countryIndex].mainRegionIndex;
                CountryRegionSelect();
            }

            // Just in case makes GUICountryIndex selects appropiate value in the combobox
            GUICategoryName = category.name;
            GUICategoryDescription = category.description;
            GUICategoryClass = category.categoryClass;
            GUICategoryYear = category.year.ToString();
            GUICategoryLink = category.categoryLink;
            GUICategoryLatLon = category.latlon;
            //New tag
            //GUICategoryTag = category.tag;
            SyncGUICategorySelection();
            if (categoryIndex >= 0) {
                GUICategoryNewName = category.name;
                GUICategoryNewDescription = category.description;
                GUICategoryNewLink = category.categoryLink;
                //New tag
                //GUICategoryNewTag = category.tag;
                CategoryHighlightSelection();
            }
        }

        public bool CategorySelectByScreenClick(Ray ray, int countryIndex = -1) {
            int targetCategoryIndex;
            if (map.GetCategoryIndex(ray, out targetCategoryIndex, countryIndex)) {
                categoryIndex = targetCategoryIndex;
                CategorySelect();
                return true;
            }
            return false;
        }

        void CategoryHighlightSelection() {

            if (categoryIndex < 0 || categoryIndex >= map.categories.Count)
                return;

            // Colorize category
            map.HideCategoryHighlights();
            map.ToggleCategoryHighlight(categoryIndex, Color.grey, true);
        }

        public void ReloadCategoryNames() {
            if (map == null || map.categories == null) {
                lastCategoryCount = -1;
                return;
            }
            lastCategoryCount = map.categories.Count; // check this size, and not result from GetCityNames because it could return additional rows (separators and so)
            _categoryNames = map.GetCategoryNames(countryIndex, true);
            SyncGUICategorySelection();
            CategorySelect(); // refresh selection
        }
        //New tag
        /*public void ReloadCategoryTags() {
            if (map == null || map.categories == null) {
                lastCategoryCount = -1;
                return;
            }
            lastCategoryCount = map.categories.Count; // check this size, and not result from GetCityNames because it could return additional rows (separators and so)
            _categoryTags = map.GetCategoryTag(countryIndex, true);
            SyncGUICategorySelection();
            CategorySelect(); // refresh selection
        }*/
        
         public void ReloadCategoryDescriptions() {
            if (map == null || map.categories == null) {
                lastCategoryCount = -1;
                return;
            }
            lastCategoryCount = map.categories.Count; // check this size, and not result from GetCityNames because it could return additional rows (separators and so)
            _categoryDescriptions = map.GetCategoryDescriptions(countryIndex, true);
            SyncGUICategorySelection();
            CategorySelect(); // refresh selection
        }

        void SyncGUICategorySelection() {
            // recover GUI category index selection
            if (GUICategoryName.Length > 0) {
                for (int k = 0; k < categoryNames.Length; k++) {
                    if (_categoryNames[k].TrimStart().StartsWith(GUICategoryName)) {
                        GUICategoryIndex = k;
                        if (categoryIndex < 0) {
                            categoryIndex = map.GetCategoryIndexInCountry(countryIndex, GUICategoryName);
                        }
                        return;
                    }
                }
                if (map.GetCategoryIndex(GUICategoryName) < 0) {
                    SetInfoMsg("Event " + GUICategoryName + " not found in database.");
                }
            }
            GUICategoryIndex = -1;
            GUICategoryName = "";
        }

        /// <summary>
        /// Deletes current category
        /// </summary>
        public void DeleteCategory() {
            if (categoryIndex < 0 || categoryIndex >= map.categories.Count)
                return;

            map.HideCategoryHighlights();
            map.categories.RemoveAt(categoryIndex);
            categoryIndex = -1;
            GUICategoryName = "";
            GUICategoryDescription = "";
            SyncGUICategorySelection();
            map.DrawCategories(GUIFilterCategoryClass);
            categoryChanges = true;
        }

        /// <summary>
        /// Deletes all categories of current selected country
        /// </summary>
        public void DeleteCountryCategories() {
            if (countryIndex < 0)
                return;

            map.HideCategoryHighlights();
            int k = -1;
            while (++k < map.categories.Count) {
                if (map.categories[k].countryIndex == countryIndex) {
                    map.categories.RemoveAt(k);
                    k--;
                }
            }
            categoryIndex = -1;
            GUICategoryName = "";
            GUICategoryDescription = "ND";
            SyncGUICategorySelection();
            map.DrawCategories(GUIFilterCategoryClass);
            categoryChanges = true;
        }


        /// <summary>
        /// Deletes all cities of current selected country's continent
        /// </summary>
        public void DeleteCategoriesSameContinent() {
            if (countryIndex < 0)
                return;

            map.HideCategoryHighlights();
            int k = -1;
            string continent = map.countries[countryIndex].continent;
            while (++k < map.categories.Count) {
                int catindex = map.categories[k].countryIndex;
                if (catindex >= 0) {
                    string categoryContinent = map.countries[catindex].continent;
                    if (categoryContinent.Equals(continent)) {
                        map.categories.RemoveAt(k);
                        k--;
                    }
                }
            }
            categoryIndex = -1;
            GUICategoryName = "";
            SyncGUICategorySelection();
            map.DrawCategories(GUIFilterCategoryClass);
            categoryChanges = true;
        }
        
        #endregion

   

    }
}
