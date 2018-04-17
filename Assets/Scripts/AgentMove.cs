using UnityEngine;
using UnityEngine.AI;

public class AgentMove : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip footstep;

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
        audioSource = GetComponent<AudioSource>();
	}


    float audioTimer = 0.4f;
    float audioTime = 0.4f;
    private void Update()
    {
        audioTimer -= Time.deltaTime;
        if (audioTimer <= 0f && agent.velocity.magnitude >= 1f)
        {
            audioSource.clip = footstep;
            audioSource.volume = Random.Range(0.6f, 0.8f);
            audioSource.Play();
            audioTimer = audioTime;
        }

        animator.SetFloat("Forward", agent.velocity.magnitude);
        if (goal != null)
        {
            agent.SetDestination(goal.position);
        }
    }
}
