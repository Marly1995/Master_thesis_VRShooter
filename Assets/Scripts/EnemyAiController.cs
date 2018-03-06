using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates
{
    Moving,
    Crouching,
    Idle,
}

public class EnemyAiController : MonoBehaviour
{
    public float speed;
    public GameObject trackingPoint;

    public EnemyStates currentState;

    Animator animator;

    Vector3 destination;
    public Vector3 Destination
    {
        get { return destination; }
        set { destination = value; }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update ()
    {
		switch (currentState)
        {
            case EnemyStates.Moving:
                DestinationCheck();
                Move();
                animator.SetFloat("Forward", 1.0f);
                break;
            case EnemyStates.Crouching:
                animator.SetBool("Crouch", true);
                animator.SetFloat("Forward", .0f);
                break;
            case EnemyStates.Idle:
                animator.SetBool("Crouch", false);
                animator.SetFloat("Forward", .0f);
                break;
        }
	}

    void Move()
    {
        Vector3 direction = destination - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
        transform.position = Vector3.MoveTowards(transform.position, destination, speed);
        
        if (transform.position == destination)
        {
            currentState = EnemyStates.Idle;
        }
    }

    void DestinationCheck()
    {
        destination = trackingPoint.transform.position;
    }
}
