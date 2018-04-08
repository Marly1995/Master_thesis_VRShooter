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
    AudioSource audioSource;
    public AudioClip laserSound;

    [SerializeField]
    EnemyState state;
    public EnemyState State
    {
        get { return state; }
        set { state = value; }
    }

    [HideInInspector]
    public EnemyStatemanager manager;
    
    AgentMove controller;

    bool gunShown;
    MeshRenderer gunRenderer;
    Transform barrell;
    public GameObject hitParticles;
    public GameObject hitTrail;

    Transform target;

    bool hasCoverPoint = false;

    public Transform[] patrolPositions;
    int patrolIndex;

    public float fireRate;
    float shotTime;

    public SkinnedMeshRenderer robotRenderer;
    float deathtime = 2.0f;
    bool dead;

    private void Start()
    {
        manager = FindObjectOfType<EnemyStatemanager>();
        gunRenderer = GetComponentInChildren<MeshRenderer>();
        barrell = gunRenderer.transform.GetChild(0);
        robotRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        controller = GetComponent<AgentMove>();
        ChooseNextPatrolPosition();
    }

    private void Update()
    {
        switch (state)
        {
            case EnemyState.Idle:
                if (gunShown)
                    StartCoroutine(DissolveGunOut());
                gunShown = false;
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", false);
                break;
            case EnemyState.Cover:
                if (gunShown)
                    StartCoroutine(DissolveGunOut());
                gunShown = false;
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", true);
                controller.Animator.SetBool("Shoot", false);
                break;
            case EnemyState.TakingCover:
                if (gunShown)
                    StartCoroutine(DissolveGunOut());
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
                    hasCoverPoint = true;
                    state = EnemyState.Cover;
                }
                break;
            case EnemyState.Patrol:
                if (gunShown)
                    StartCoroutine(DissolveGunOut());
                gunShown = false;
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", false);
                if (controller.goal == null) { state = EnemyState.Idle; }
                if (Vector3.Distance(controller.transform.position, controller.goal.position) <= 1f)
                {
                    ChooseNextPatrolPosition();
                }
                break;
            case EnemyState.Sneak:
                if (gunShown)
                    StartCoroutine(DissolveGunOut());
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
                    Die();
                break;
        }
    }

    void ChooseNextPatrolPosition()
    {
        if (patrolPositions.Length <= 0) { state = EnemyState.Idle; return; }
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
        Transform coverpoint = manager.SearchForCover(transform.position);
        if (coverpoint != null)
        {
            controller.goal = coverpoint;
            hasCoverPoint = true;
        }
    }

    void ShootTarget()
    {
        shotTime -= Time.deltaTime;
        if (!gunShown)
        {
            StartCoroutine(DissolveGunIn());
        }
        gunShown = true;

        target = manager.SearchForTarget(transform.position);

        if (target != null)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, Quaternion.LookRotation(-target.position, Vector3.up).eulerAngles.y, transform.rotation.z);
            if (shotTime <= 0f)
            {
                Vector3 shotDir = (target.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))) - barrell.position;
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
                audioSource.clip = laserSound;
                audioSource.Play();
            }
        }
    }

    void Die()
    {
        deathtime -= Time.deltaTime;
        if (!dead)
        {
            if (deathtime <= 0f)
            { StartCoroutine(DissolvePlayerOut()); dead = true; }
        }
    }
    
    IEnumerator DissolveGunOut()
    {
        for (float i = 0f; i <= 1f; i += 0.02f)
        {
            gunRenderer.material.SetFloat("_SliceAmount", i);
            yield return new WaitForSeconds(0.002f);
        }
    }

    IEnumerator DissolveGunIn()
    {
        for (float i = 1f; i >= -0.1f; i -= 0.02f)
        {
            gunRenderer.material.SetFloat("_SliceAmount", i);
            yield return new WaitForSeconds(0.002f);
        }
    }

    IEnumerator DissolvePlayerOut()
    {
        for (float i = 0f; i <= 1f; i += 0.02f)
        {
            robotRenderer.material.SetFloat("_SliceAmount", i);
            yield return new WaitForSeconds(0.002f);
        }
        Destroy(gameObject);
    }
}