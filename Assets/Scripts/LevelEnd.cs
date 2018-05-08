using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        WorldState.Score -= Mathf.FloorToInt(Time.time * 50);
    }
}
