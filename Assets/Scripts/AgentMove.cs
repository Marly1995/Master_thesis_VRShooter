using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMove : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    public Transform goal;
	void Start ()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
	}

    private void Update()
    {
        agent.SetDestination(goal.position);
        animator.SetFloat("Forward", agent.velocity.magnitude);
    }
}
