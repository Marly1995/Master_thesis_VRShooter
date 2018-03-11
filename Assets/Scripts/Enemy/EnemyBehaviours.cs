using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviours : MonoBehaviour
{
    AgentMove controller;

    public Transform[] patrolPositions;

    int patrolIndex;

    private void Start()
    {
        controller = GetComponent<AgentMove>();
        ChooseNextPatrolPosition();
    }

    private void Update()
    {
        if (Vector3.Distance(controller.transform.position, controller.goal.position) <= 1f)
        {
            ChooseNextPatrolPosition();
        }
    }

    void ChooseNextPatrolPosition()
    {
        if (patrolIndex < patrolPositions.Length)
        {
            controller.goal = patrolPositions[patrolIndex++];
        }
        else
        {
            patrolIndex = 0;
            controller.goal = patrolPositions[patrolIndex];
        }
    }
}
