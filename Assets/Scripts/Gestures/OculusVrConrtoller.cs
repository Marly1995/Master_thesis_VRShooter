using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusVrConrtoller : MonoBehaviour
{
    MousePositionRecorder rec;

    private void Start()
    {
        rec = GetComponent<MousePositionRecorder>();
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            rec.BeginRecording();
        }
        else if (OVRInput.GetUp(OVRInput.Button.One))
        {
            rec.EndRecording();
            rec.CheckRecognized();
        }
    }
}
