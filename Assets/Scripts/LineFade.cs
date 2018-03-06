using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFade : MonoBehaviour
{
    VolumetricLines.VolumetricLineBehavior vol;
    public float decayRate;
    void Start () {
        vol = GetComponent<VolumetricLines.VolumetricLineBehavior>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        vol.LineWidth -= decayRate;
        if (vol.LineWidth <= 0)
        {
            Destroy(gameObject);
        }
	}
}
