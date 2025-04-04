using UnityEngine;
using UnityEngine.UI;

namespace WPM {

    public class CategoryPanel : MonoBehaviour {

        public GameObject prefab;
        WorldMapGlobe map;
        GUIStyle labelStyle;
        GameObject currentPanel;

        void Start() {
            // Get a reference to the World Map API:
            map = WorldMapGlobe.instance;

            // UI Setup - non-important, only for this demo
            labelStyle = new GUIStyle();
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.white;

            // setup GUI resizer - only for the demo
            GUIResizer.Init(800, 500);

        }


        void Update() {
            
            map.OnCategoryClick += (int categoryIndex) => AddPanel(categoryIndex);
        }

         void AddPanel(int categoryIndex) {

                    // If previous panel exists, destroy it
                    if (currentPanel != null) {
                        Destroy(currentPanel);
                    }

                    // Instantiate panel
                    currentPanel = Instantiate<GameObject>(prefab);

                    // Update panel texts
                    Text countryName, categoryName, categoryDescription, categoryYear, categoryLink;
                    countryName = currentPanel.transform.Find("Panel/RowCountry/CountryName").GetComponent<Text>();
                    categoryName = currentPanel.transform.Find("Panel/RowCategory/CategoryName").GetComponent<Text>();
                    categoryDescription = currentPanel.transform.Find("Panel/RowCategoryDescription/CategoryDescription").GetComponent<Text>();
                    categoryYear = currentPanel.transform.Find("Panel/RowCategoryYear/CategoryYear").GetComponent<Text>();
                    
                    categoryLink = currentPanel.transform.Find("Panel/RowLink/LinkText").GetComponent<Text>();

                    // Gets the category clicked and populate data
                    Category category = map.GetCategory(categoryIndex);
                    categoryName.text = category.name;
                    categoryDescription.text = category.description;
                    categoryYear.text = category.year.ToString();
                    countryName.text = map.GetCategoryCountryName(categoryIndex);

                    categoryLink.text = category.categoryLink;
            
                    // Position the canvas over the globe
                    float distaceToGlobeCenter = 1.1f;
                    Vector3 worldPos = map.transform.TransformPoint(category.localPosition * distaceToGlobeCenter);
                    currentPanel.transform.position = worldPos;

                    // Draw a circle around the city
                    //map.AddMarker(MARKER_TYPE.CIRCLE_PROJECTED, category.localPosition, 60, 0.8f, 1f, Color.green);

                    // Parent the panel to the globe so it rotates with it
                    //currentPanel.transform.SetParent(map.transform, true);
                    
                }


    }

}

