using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WPM {
	/// <summary>
	/// City scaler. Checks the category icons' size is always appropiate
	/// </summary>
	public class CategoryScaler : MonoBehaviour {

		const int CATEGORY_SIZE_ON_SCREEN = 15;
		Vector3 lastCamPos, lastPos;
		float lastIconSize;
		float lastCustomSize;

		[NonSerialized]
		public WorldMapGlobe map;

		void Start () {
			if (map == null) {
				Destroy (this);
			} else {
				ScaleCategories();
			}
		}
	
		// Update is called once per frame
		void Update () {
			if (map == null || lastPos == transform.position && lastCamPos == map.mainCamera.transform.position && lastIconSize == map.cityIconSize)
				return;
			ScaleCategories();
		}

		public static float GetScale (WorldMapGlobe map) {
			Camera cam = map.mainCamera;
			if (cam == null || cam.pixelWidth == 0)
				return 0;
			float oldFV = cam.fieldOfView;
			if (!UnityEngine.XR.XRSettings.enabled && !map.earthInvertedMode) { 
				cam.fieldOfView = 60.0f;
			}
			Vector3 refPos = map.transform.position;
            if (map.earthInvertedMode) {
                refPos += cam.transform.forward * (map.transform.lossyScale.y * 0.5f); 
            }
			Vector3 a = cam.WorldToScreenPoint (refPos);
			Vector3 b = new Vector3 (a.x, a.y + CATEGORY_SIZE_ON_SCREEN * map.categoryIconSize, a.z);
			Vector3 aa = cam.ScreenToWorldPoint (a);
			Vector3 bb = cam.ScreenToWorldPoint (b);
			if (!UnityEngine.XR.XRSettings.enabled) {
				cam.fieldOfView = oldFV;
			}
			float scale = (aa - bb).magnitude / map.transform.localScale.y;
			//return Mathf.Clamp (scale, 0.00001f, 0.005f);
            return Mathf.Clamp(scale, 0.008f,0.01f);
        }

		public void ScaleCategories () {
			if (map == null)
				return;
			Camera cam = map.mainCamera;
			if (cam == null || cam.pixelWidth == 0)
				return;
			lastPos = transform.position;
			lastCamPos = cam.transform.position;
			lastIconSize = map.cityIconSize;
			float oldFV = cam.fieldOfView;
			if (!UnityEngine.XR.XRSettings.enabled && !map.earthInvertedMode) {
				cam.fieldOfView = 60.0f;
			}
			Vector3 refPos = transform.position;
			if (map.earthInvertedMode)
				refPos += cam.transform.forward * (map.transform.lossyScale.y * 0.5f); ; // otherwise, transform.position = 0 in inverted mode
            Vector3 a = cam.WorldToScreenPoint (refPos);
			Vector3 b = new Vector3 (a.x, a.y + CATEGORY_SIZE_ON_SCREEN * map.categoryIconSize, a.z);
			Vector3 aa = cam.ScreenToWorldPoint (a);
			Vector3 bb = cam.ScreenToWorldPoint (b);
			if (!UnityEngine.XR.XRSettings.enabled) {
				cam.fieldOfView = oldFV;
			}
			float scale = (aa - bb).magnitude / map.transform.localScale.y; // * map.categoryIconSize;
			scale = Mathf.Clamp (scale, 0.00001f, 0.005f);
			Vector3 newScale = new Vector3 (scale, scale, 1.0f);
			ScaleCategories(newScale);
		}

		public void ScaleCategories (float customSize) {
			customSize = Mathf.Clamp (customSize, 0, 0.005f);
			if (customSize == lastCustomSize)
				return;
			lastCustomSize = customSize;
			Vector3 newScale = new Vector3 (customSize, customSize, 1);
			ScaleCategories (newScale);
		}

		void ScaleCategories (Vector3 newScale) {
			Transform battleCategories = transform.Find ("BattleLayer");
			if (battleCategories != null) {
				foreach (Transform t in battleCategories)
					t.localScale = newScale;
			}
			Transform artCategories = transform.Find ("ArtLayer");
			if (artCategories != null) {
				foreach (Transform t in artCategories)
					t.localScale = newScale * 1.75f;
			}
		}
	}

}