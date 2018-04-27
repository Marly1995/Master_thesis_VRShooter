using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    bool end;
    private void OnTriggerEnter(Collider other)
    {
        end = true;
    }

    private void Update()
    {
        if (!end)
        {
            WorldState.Score -= Mathf.FloorToInt(Time.deltaTime * 10);
        }
    }
}
