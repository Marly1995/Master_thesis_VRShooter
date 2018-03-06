using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDie : MonoBehaviour
{
    public GameObject obj;
    SkinnedMeshRenderer render;

    private void Start()
    {
        render = GetComponent<SkinnedMeshRenderer>();
    }

    public void Die()
    {
        StartCoroutine(DissolveOut());
    }

    IEnumerator DissolveOut()
    {
        for (float i = 0f; i <= 1f; i += 0.02f)
        {
            render.material.SetFloat("_SliceAmount", i);
            yield return new WaitForSeconds(0.001f);
        }
        Destroy(obj);
    }
}
