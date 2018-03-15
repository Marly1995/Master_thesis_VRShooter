using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Sneak,
    Cover,
    TakingCover
}

public class EnemyBehaviours : MonoBehaviour
{
    [SerializeField]
    EnemyState state;
    public EnemyState State
    {
        get { return state; }
        set { state = value; }
    }
    
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
        switch (state)
        {
            case EnemyState.Idle:
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", false);
                break;
            case EnemyState.Cover:
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", true);
                break;
            case EnemyState.TakingCover:
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", false);
                if (Vector3.Distance(controller.transform.position, controller.goal.position) <= 1f)
                {
                    controller.Agent.isStopped = true;
                    controller.Animator.SetBool("Crouch", true);
                    state = EnemyState.Cover;
                }
                break;
            case EnemyState.Patrol:
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", false);
                if (Vector3.Distance(controller.transform.position, controller.goal.position) <= 1f)
                {
                    ChooseNextPatrolPosition();
                }
                break;
            case EnemyState.Sneak:
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", true);
                if (Vector3.Distance(controller.transform.position, controller.goal.position) <= 1f)
                {
                    ChooseNextPatrolPosition();
                }
                break;
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
