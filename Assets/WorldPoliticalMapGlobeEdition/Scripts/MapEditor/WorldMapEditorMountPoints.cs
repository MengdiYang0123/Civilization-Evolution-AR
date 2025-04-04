﻿using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace WPM {
    public partial class WorldMapEditor : MonoBehaviour {

        public int GUIMountPointIndex;
        public string GUIMountPointName = "";
        public string GUIMountPointNewName = "";
        public string GUIMountPointNewType = "";
        public string GUIMountPointNewTagKey = "";
        public string GUIMountPointNewTagValue = "";
        public int mountPointIndex = -1;
        public bool mountPointChanges;  // if there's any pending change to be saved

        // private fields
        int lastMountPointCount = -1;
        string[] _mountPointNames;


        public string[] mountPointNames {
            get {
                if (map.mountPoints != null && lastMountPointCount != map.mountPoints.Count) {
                    mountPointIndex = -1;
                    ReloadMountPointNames();
                }
                return _mountPointNames;
            }
        }


        #region Editor functionality


        public void ClearMountPointSelection() {
            map.HideMountPointHighlights();
            mountPointIndex = -1;
            GUIMountPointIndex = -1;
            GUIMountPointName = "";
            GUIMountPointNewName = "";
            GUIMountPointNewTagKey = "";
            GUIMountPointNewTagValue = "";
            GUIMountPointNewType = "";
        }


        /// <summary>
        /// Adds a new mount point to current country.
        /// </summary>
        public void MountPointCreate(Vector3 newPoint) {
            int countryIndex = _map.GetCountryIndex(newPoint);
            if (countryIndex < 0) return;
            GUIMountPointName = "New Mount Point " + (map.mountPoints.Count + 1);
            newPoint = newPoint.normalized * 0.5f;
            int provinceIndex = _map.GetProvinceIndex(newPoint);
            MountPoint newMountPoint = new MountPoint(GUIMountPointName, countryIndex, provinceIndex, newPoint);
            if (map.mountPoints == null) map.mountPoints = new List<MountPoint>();
            map.mountPoints.Add(newMountPoint);
            map.DrawMountPoints();
            lastMountPointCount = -1;
            ReloadMountPointNames();
            mountPointChanges = true;
        }

        public bool MountPointUpdateType() {
            if (mountPointIndex < 0) return false;
            int type = map.mountPoints[mountPointIndex].type;
            int.TryParse(GUIMountPointNewType, out type);
            map.mountPoints[mountPointIndex].type = type;
            mountPointChanges = true;
            return true;
        }


        public bool MountPointRename() {
            if (mountPointIndex < 0) return false;
            string prevName = map.mountPoints[mountPointIndex].name;
            GUIMountPointNewName = GUIMountPointNewName.Trim();
            if (prevName.Equals(GUIMountPointNewName)) return false;
            map.mountPoints[mountPointIndex].name = GUIMountPointNewName;
            GUIMountPointName = GUIMountPointNewName;
            lastMountPointCount = -1;
            ReloadMountPointNames();
            map.DrawMountPoints();
            mountPointChanges = true;
            return true;
        }


        public void MountPointMove(Vector3 destination) {
            if (mountPointIndex < 0) return;
            map.mountPoints[mountPointIndex].localPosition = destination.normalized * 0.5f;
            Transform t = map.transform.Find(WorldMapGlobe.MOUNT_POINTS_LAYER + "/" + mountPointIndex.ToString());
            if (t != null) t.localPosition = destination * 1.001f;
            mountPointChanges = true;
        }

        public void MountPointSelectByCombo(int selection) {
            GUIMountPointIndex = selection;
            GUIMountPointName = "";
            GetMountPointIndexByGUISelection();
            MountPointSelect();
        }

        bool GetMountPointIndexByGUISelection() {
            if (GUIMountPointIndex < 0 || GUIMountPointIndex >= mountPointNames.Length) return false;
            string[] s = mountPointNames[GUIMountPointIndex].Split(new char[] {
                '(',
                ')'
            }, System.StringSplitOptions.RemoveEmptyEntries);
            if (s.Length >= 2) {
                GUIMountPointName = s[0].Trim();
                if (int.TryParse(s[1], out mountPointIndex)) {
                    return true;
                }
            }
            return false;
        }

        public void MountPointSelect() {
            if (mountPointIndex < 0 || mountPointIndex > map.mountPoints.Count)
                return;

            // If no country is selected (the mount point could be at sea) select it
            MountPoint mp = map.mountPoints[mountPointIndex];
            int mpCountryIndex = mp.countryIndex;
            if (mpCountryIndex < 0) {
                SetInfoMsg("Country not found in this country file.");
            }

            if (countryIndex != mpCountryIndex && mpCountryIndex >= 0) {
                ClearSelection();
                countryIndex = mpCountryIndex;
                countryRegionIndex = map.countries[countryIndex].mainRegionIndex;
                CountryRegionSelect();
            }

            // Just in case makes GUICountryIndex selects appropiate value in the combobox
            GUIMountPointName = mp.name;
            SyncGUIMountPointSelection();
            if (mountPointIndex >= 0) {
                GUIMountPointNewName = mp.name;
                GUIMountPointNewType = mp.type.ToString();
                MountPointHighlightSelection();
            }
        }

        public bool MountPointSelectByScreenClick(Ray ray, int countryIndex = -1) {
            int targetMountPointIndex;
            if (map.GetMountPointIndex(ray, out targetMountPointIndex, countryIndex)) {
                mountPointIndex = targetMountPointIndex;
                MountPointSelect();
                return true;
            }
            return false;
        }

        void MountPointHighlightSelection() {

            if (mountPointIndex < 0 || mountPointIndex >= map.mountPoints.Count) return;

            // Colorize mount point
            map.HideMountPointHighlights();
            map.ToggleMountPointHighlight(mountPointIndex, Color.green, true);
        }


        public void ReloadMountPointNames() {
            if (map == null || map.mountPoints == null) {
                lastMountPointCount = -1;
                return;
            }
            lastMountPointCount = map.mountPoints.Count; // check this size, and not result from GetCityNames because it could return additional rows (separators and so)
            _mountPointNames = map.GetMountPointNames(countryIndex);
            SyncGUIMountPointSelection();
            MountPointSelect(); // refresh selection
        }

        void SyncGUIMountPointSelection() {
            // recover GUI mount point index selection
            if (GUIMountPointName.Length > 0) {
                for (int k = 0; k < mountPointNames.Length; k++) {
                    if (_mountPointNames[k].TrimStart().StartsWith(GUIMountPointName)) {
                        GUIMountPointIndex = k;
                        mountPointIndex = map.GetMountPointIndex(countryIndex, GUIMountPointName);
                        return;
                    }
                }
                SetInfoMsg("Mount point " + GUIMountPointName + " not found in database.");
            }
            GUIMountPointIndex = -1;
            GUIMountPointName = "";
        }

        /// <summary>
        /// Deletes current mount point
        /// </summary>
        public void DeleteMountPoint() {
            if (map.mountPoints == null || mountPointIndex < 0 || mountPointIndex >= map.mountPoints.Count) return;

            map.HideMountPointHighlights();
            map.mountPoints.RemoveAt(mountPointIndex);
            mountPointIndex = -1;
            GUIMountPointName = "";
            SyncGUIMountPointSelection();
            map.DrawMountPoints();
            mountPointChanges = true;
        }

        /// <summary>
        /// Deletes all mount points of current selected country
        /// </summary>
        public void DeleteCountryMountPoints() {
            if (countryIndex < 0) return;

            map.HideMountPointHighlights();
            if (map.mountPoints != null) {
                int k = -1;
                while (++k < map.mountPoints.Count) {
                    if (map.mountPoints[k].countryIndex == countryIndex) {
                        map.mountPoints.RemoveAt(k);
                        k--;
                    }
                }
            }
            mountPointIndex = -1;
            GUIMountPointName = "";
            SyncGUIMountPointSelection();
            map.DrawMountPoints();
            mountPointChanges = true;
        }


        /// <summary>
        /// Deletes all mount points of current selected country's continent
        /// </summary>
        public void DeleteMountPointsSameContinent() {
            if (countryIndex < 0 || map.mountPoints == null) return;

            map.HideMountPointHighlights();
            int k = -1;
            string continent = map.countries[countryIndex].continent;
            while (++k < map.mountPoints.Count) {
                int cindex = map.mountPoints[k].countryIndex;
                if (cindex >= 0) {
                    string mpContinent = map.countries[cindex].continent;
                    if (mpContinent.Equals(continent)) {
                        map.mountPoints.RemoveAt(k);
                        k--;
                    }
                }
            }
            mountPointIndex = -1;
            GUIMountPointName = "";
            SyncGUIMountPointSelection();
            map.DrawMountPoints();
            mountPointChanges = true;
        }


        #endregion

        #region IO stuff

        /// <summary>
        /// Returns the file name corresponding to the current mount point data file
        /// </summary>
        public string GetMountPointGeoDataFileNameLegacy() {
            return "mountPoints.txt";
        }



        /// <summary>
        /// Exports the geographic data in packed string format.
        /// </summary>
        public string GetMountPointGeoData() {
            if (map.mountPoints == null) return null;
            int count = map.mountPoints.Count;
            if (count == 0) return "";
            JSONObject json = new JSONObject();
            for (int k = 0; k < count; k++) {
                MountPoint mp = map.mountPoints[k];
                JSONObject mpJSON = new JSONObject();
                mpJSON.AddField("Name", mp.name);
                mpJSON.AddField("Country", _map.countries[mp.countryIndex].name);
                mpJSON.AddField("Province", mp.provinceIndex);
                mpJSON.AddField("Type", mp.type);
                mpJSON.AddField("X", (int)(mp.localPosition.x * WorldMapGlobe.MAP_PRECISION));
                mpJSON.AddField("Y", (int)(mp.localPosition.y * WorldMapGlobe.MAP_PRECISION));
                mpJSON.AddField("Z", (int)(mp.localPosition.z * WorldMapGlobe.MAP_PRECISION));
                mpJSON.AddField("Attrib", mp.attrib);
                json.Add(mpJSON);
            }
            return json.Print(true);
        }


        #endregion

    }
}
