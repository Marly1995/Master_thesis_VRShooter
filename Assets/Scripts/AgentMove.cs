using UnityEngine;
using UnityEngine.AI;

public class AgentMove : MonoBehaviour
{
    NavMeshAgent agent;
    public NavMeshAgent Agent
    {
        get { return agent; }
    }
    Animator animator;
    public Animator Animator
    {
        get { return animator; }
    }

    public Transform goal;
	void Start ()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
	}

    private void Update()
    {
       // agent.SetDestination(goal.position);
        animator.SetFloat("Forward", agent.velocity.magnitude);
    }
}
