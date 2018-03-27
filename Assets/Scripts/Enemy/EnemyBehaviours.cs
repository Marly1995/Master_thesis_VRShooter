using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Sneak,
    Cover,
    TakingCover,
    CoverShooting,
    Shooting,
    Dead
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

    bool gunShown;
    public MeshRenderer gunRenderer;
    public Transform barrell;
    public GameObject hitParticles;
    public GameObject hitTrail;

    public Transform target;

    public Transform[] coverPositions;
    bool hasCoverPoint = false;

    public Transform[] patrolPositions;
    int patrolIndex;

    public float fireRate;
    float shotTime;

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
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", false);
                break;
            case EnemyState.Cover:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", true);
                controller.Animator.SetBool("Shoot", false);
                break;
            case EnemyState.TakingCover:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", false);
                if (!hasCoverPoint)
                { TakeCover(); }
                if (Vector3.Distance(controller.transform.position, controller.goal.position) <= 1f)
                {
                    controller.Agent.isStopped = true;
                    controller.Animator.SetBool("Crouch", true);
                    hasCoverPoint = false;
                    state = EnemyState.Cover;
                }
                break;
            case EnemyState.Patrol:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", false);
                if (Vector3.Distance(controller.transform.position, controller.goal.position) <= 1f)
                {
                    ChooseNextPatrolPosition();
                }
                break;
            case EnemyState.Sneak:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", true);
                controller.Animator.SetBool("Shoot", false);
                if (Vector3.Distance(controller.transform.position, controller.goal.position) <= 1f)
                {
                    ChooseNextPatrolPosition();
                }
                break;
            case EnemyState.Shooting:
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", true);
                ShootTarget();
                break;
            case EnemyState.CoverShooting:
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", true);
                controller.Animator.SetBool("Shoot", true);
                break;
            case EnemyState.Dead:
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Dead", true);
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

    void TakeCover()
    {
        float shortest = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < coverPositions.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, coverPositions[i].position);
            Debug.Log(dist);
            if (dist < shortest)
            {
                index = i;
                shortest = dist;
            }
        }
        controller.goal = coverPositions[index];
        hasCoverPoint = true;
    }

    void ShootTarget()
    {
        shotTime -= Time.deltaTime;
        if (!gunShown)
        {
            StartCoroutine(DissolveIn());
        }
        gunShown = true;
        transform.rotation = Quaternion.Euler(transform.rotation.x, Quaternion.LookRotation(-target.position, Vector3.up).eulerAngles.y, transform.rotation.z);

        if (shotTime <= 0f)
        {
            Vector3 shotDir = (target.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f))) - barrell.position;
            Ray ray = new Ray(barrell.position, shotDir);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject particles = Instantiate(hitParticles, hit.point, Quaternion.identity);
                Destroy(particles, 0.4f);
                if (hit.collider.tag == "Enemy")
                {
                    //hit.collider.gameObject.GetComponent<EnemyDie>().Die();
                }
            }
            GameObject trail = Instantiate(hitTrail);
            VolumetricLines.VolumetricLineBehavior vol = trail.GetComponent<VolumetricLines.VolumetricLineBehavior>();
            vol.StartPos = barrell.position;
            vol.EndPos = hit.point;
            shotTime = fireRate;
        }
    }
    
    IEnumerator DissolveOut()
    {
        for (float i = 0f; i <= 1f; i += 0.02f)
        {
            gunRenderer.material.SetFloat("_SliceAmount", i);
            yield return new WaitForSeconds(0.002f);
        }
    }

    IEnumerator DissolveIn()
    {
        for (float i = 1f; i >= -0.1f; i -= 0.02f)
        {
            gunRenderer.material.SetFloat("_SliceAmount", i);
            yield return new WaitForSeconds(0.002f);
        }
    }
}