using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightModel : MonoBehaviour
{
    [Range(0, 1)]
    public float highlightIntensity;

    public Material mat;

    private void Update()
    {
        mat.SetFloat("GlobalEnemyXRay", highlightIntensity);
    }
}
