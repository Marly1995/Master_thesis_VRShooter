using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatemanager : MonoBehaviour
{
    public FriendlyBehaviour[] targets;
    public Transform player;

    public Transform[] coverPositions;
    public bool[] coverTaken;

    public float sightRadius;
    public float hiddenSightRadius;

    Ray ray;
    RaycastHit hit;

    private void Start()
    {
        coverTaken = new bool[coverPositions.Length];
    }

    public Transform SearchForTarget(Vector3 position)
    {
        Transform newTarget = null;
        float closest = Mathf.Infinity;
        foreach(FriendlyBehaviour tar in targets)
        {
            float dist = Vector3.Distance(tar.transform.position, position);
            if (dist < closest && dist < sightRadius)
            {
                if (tar.State == FriendState.Sneak ||
                    tar.State == FriendState.Cover)
                {
                    if (dist < hiddenSightRadius)
                    {
                        ray = new Ray(transform.position, tar.transform.position);
                        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Friend")))
                        {
                            if (hit.collider.tag == "Friendly")
                            {
                                closest = dist;
                                newTarget = tar.transform;
                            }
                        }
                    }
                }
                else
                {
                    ray = new Ray(transform.position, tar.transform.position);
                    if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Friend")))
                    {
                        if (hit.collider.tag == "Friendly")
                        {
                            closest = dist;
                            newTarget = tar.transform;
                        }
                    }
                }
            }
        }
        return newTarget;
    }

    public Transform SearchForCover(Vector3 position)
    {
        Transform newTarget = null;
        float closest = Mathf.Infinity;
        for (int i = 0; i < coverPositions.Length; i++)
        {
            float dist = Vector3.Distance(coverPositions[i].position, position);
            if (dist < closest && dist < sightRadius)
            {
                if (!coverTaken[i])
                {
                    closest = dist;
                    newTarget = coverPositions[i];
                    coverTaken[i] = true;
                }
            }
        }
        return newTarget;
    }
}