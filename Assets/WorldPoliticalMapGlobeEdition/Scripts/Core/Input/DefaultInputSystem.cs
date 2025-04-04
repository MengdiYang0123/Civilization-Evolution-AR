﻿using UnityEngine;

namespace WPM {
    /// <summary>
    /// This class provides an input layer that can be replaced or overriden to provide other kind of input systems
    /// </summary>
    public class DefaultInputSystem : IInputProxy {

        public virtual Vector3 mousePosition { get { return Input.mousePosition; } }
        //public Vector3 mousePosition = new Vector3(1, 1, 1);

        public virtual bool touchSupported { get { return Input.touchSupported; } }

        public virtual int touchCount { get { return Input.touchCount; } }

        public virtual Touch[] touches { get { return Input.touches; } }

        public virtual LocationService location { get { return Input.location; } }

        Vector3 IInputProxy.mousePosition => throw new System.NotImplementedException();

        public virtual float GetAxis(string axisName) {
            return Input.GetAxis(axisName);
        }

        public virtual bool GetButtonDown(string buttonName) {
            return Input.GetButtonDown(buttonName);
        }

        public virtual bool GetButtonUp(string buttonName) {
            return Input.GetButtonUp(buttonName);
        }

        public virtual bool GetKey(string name) {
            return Input.GetKey(name);
        }

        public virtual bool GetKey(KeyCode keyCode) {
            return Input.GetKey(keyCode);
        }

        public virtual bool GetKeyDown(KeyCode keyCode) {
            return Input.GetKeyDown(keyCode);
        }

        public virtual bool GetMouseButton(int buttonIndex) {
            return Input.GetMouseButton(buttonIndex);
        }

        public virtual bool GetMouseButtonDown(int buttonIndex) {
            return Input.GetMouseButtonDown(buttonIndex);
        }

        public virtual bool GetMouseButtonUp(int buttonIndex) {
            return Input.GetMouseButtonUp(buttonIndex);
        }

        public virtual Touch GetTouch(int index) {
            return Input.GetTouch(index);
        }


    }

}