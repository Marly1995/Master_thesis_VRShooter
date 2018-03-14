using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviours : MonoBehaviour
{
    AgentMove controller;

    public Transform[] patrolPositions;

    int patrolIndex;

    public bool takeCover;

    private void Start()
    {
        controller = GetComponent<AgentMove>();
        ChooseNextPatrolPosition();
    }

    private void Update()
    {
        if (takeCover)
        {
            if (Vector3.Distance(controller.transform.position, controller.goal.position) <= 1f)
            {
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", true);
            }
        }
        else
        {
            controller.Agent.isStopped = false;
            if (Vector3.Distance(controller.transform.position, controller.goal.position) <= 1f)
            {
                ChooseNextPatrolPosition();
            }
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
