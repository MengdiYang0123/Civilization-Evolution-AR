using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class RightIndexFingerPosition : MonoBehaviour, IMixedRealityHandJointHandler
{
    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
    }

    public void OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    {
        foreach (var joint in eventData.InputData)
        {
            if (joint.Key == TrackedHandJoint.IndexTip && eventData.Handedness == Handedness.Right)
            {
                // Get the position of the index fingertip
                Vector3 fingertipPosition = joint.Value.Position;

                // Now you can use fingertipPosition as the position of the right index fingertip in world space
                Debug.Log("Right Index Fingertip Position: " + fingertipPosition);
            }
        }
    }
}













