using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendStateManager : MonoBehaviour
{
    public List<EnemyBehaviours> targets;
    public Transform player;

    public GameObject[] teleportPoints;
    public Transform[] coverPositions;
    public bool[] coverTaken;

    public float sightRadius = 30f;
    public float hiddenSightRadius = 20f;

    Ray ray;
    RaycastHit hit;

    private void Start()
    {
        int index = 0;
        foreach (EnemyBehaviours item in targets)
        {
            item.personalIndex = index;
            index++;
        }
    }

    public Transform SearchForTarget(Vector3 position)
    {
        Transform newTarget = null;
        float closest = Mathf.Infinity;
        foreach (EnemyBehaviours tar in targets)
        {
            float dist = Vector3.Distance(tar.transform.position, position);
            if (dist < closest && dist < sightRadius)
            {
                if (tar.State == EnemyState.Sneak ||
                    tar.State == EnemyState.Cover)
                {
                    if (dist < hiddenSightRadius)
                    {
                        closest = dist;
                        newTarget = tar.transform;
                    }
                }
                else
                {
                    closest = dist;
                    newTarget = tar.transform;
                }
            }
        }
        return newTarget;
    }

    public void UpdateCoverPoints(int index)
    {
        TeamCoverPosition[] covpos = teleportPoints[index].GetComponentsInChildren<TeamCoverPosition>();
        coverPositions = new Transform[4];
        for (int i = 0; i < 4; i++)
        {
            coverPositions[i] = covpos[i].transform;
        }
        coverTaken = new bool[coverPositions.Length];
    }

    public Transform SearchForCover(Vector3 position)
    {
        Transform newTarget = null;
        float closest = Mathf.Infinity;
        for (int i = 0; i < coverPositions.Length; i++)
        {
            float dist = Vector3.Distance(coverPositions[i].localPosition, position);
            if (dist < closest)
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