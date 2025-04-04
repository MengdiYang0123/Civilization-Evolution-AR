using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;

namespace WPM {

	public enum CATEGORY_CLASS {
		ALL = -1,
		BATTLE = 0,
		ART = 3,
		DISCOVERY = 5,
		SCIENCE = 7,
		TREATY = 9,
		FUNFACT = 11,
		HUMANRIGHTS = 13,
		TRAGEDY = 15,
		EVENT = 17
	}
	
	/// <summary>
	/// Category records. Categories are stored in the mountPoints file, in packed string editable format inside Resources/Geodata folder.
	/// </summary>
	public partial class Category : IExtendableAttribute {
		
		/// <summary>
		/// Name of this categort.
		/// </summary>
		public string name;


		/// <summary>
		/// The index of the country.
		/// </summary>
		public int countryIndex;
		
		/// <summary>
		/// The index of the region or -1 if the  is not linked to any region.?????
		/// </summary>
		public int regionIndex = -1;
		
		//public int regionIndex = -1;
		
		[System.Obsolete("Use localPosition instead")]
		public Vector3 unitySphereLocation {
			get { return localPosition; }
		}

		/// <summary>
		/// The location of the category on the sphere.
		/// </summary>
		public Vector3 localPosition;
		public string description;
		public int year;
		//New tag
		//public string tag;
		public CATEGORY_CLASS categoryClass;
		
		public string categoryLink;

		/// <summary>
		/// Reference to the category icon drawn over the globe.
		/// </summary>
		public GameObject gameObject;
		
		/// <summary>
		/// Returns if category is visible on the map based on cityClass filter.???? //DA MODIFICARE: Vedere se funziona sto filtro
		/// </summary>
		public bool isShown;
		
		public float latitude {
			get {
				return latlon.x;
			}
		}

		public float longitude {
			get {
				return latlon.y;
			}
		}

		Vector2 _latlon;

		public Vector2 latlon {
			get {
				if (latlonPending) {
					latlonPending = false;
					_latlon = Conversion.GetLatLonFromUnitSpherePoint(localPosition);
				}
				return _latlon;
			}
			set {
				if (value != _latlon) {
					_latlon = value;
					localPosition = Conversion.GetSpherePointFromLatLon (_latlon);
				}
			}
		}

        JSONObject _attrib;

        /// <summary>
        /// Use this property to add/retrieve custom attributes for this country/province
        /// </summary>
        public JSONObject attrib { get { if (_attrib == null) { _attrib = new JSONObject(); } return _attrib; } set { _attrib = value; } }

        public bool hasAttributes { get { return _attrib != null; } }


        bool latlonPending;

        //Add tag
		public Category (string name, int countryIndex, string description, Vector3 location, CATEGORY_CLASS categoryClass, int year, string categoryLink) {
			this.name = name;
			this.countryIndex = countryIndex;
			this.description = description;
			this.categoryClass = categoryClass;
			this.year = year;
			this.categoryLink = categoryLink;
			this.localPosition = location;
			this.latlonPending = true;
			//this.tag = tag;
		}

		public Category Clone () {
			//Add tag
			Category c = new Category (name, countryIndex, description, localPosition, categoryClass, year, categoryLink);
            if (_attrib != null) {
                c.attrib = new JSONObject();
                c.attrib.Absorb(attrib);
            }
            return c;
		}

	}
}