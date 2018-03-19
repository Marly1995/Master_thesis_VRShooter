using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightModel : MonoBehaviour
{
    [Range(0, 1)]
    public float highlightIntensity;

    SkinnedMeshRenderer mr;

    private void Start()
    {
        mr = GetComponent<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        mr.sharedMaterial.SetFloat("_GlobalXRayVisibility", highlightIntensity);
    }
}
