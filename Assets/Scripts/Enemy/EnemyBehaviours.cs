using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public int points = 500;
    public GameObject add;

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
    FriendStateManager fmanager;
    
    AgentMove controller;

    bool gunShown;
    MeshRenderer gunRenderer;
    Transform barrell;
    public GameObject bullet;
    public GameObject hitTrail;

    public Transform target;

    bool hasCoverPoint = false;

    public Transform[] patrolPositions;
    int patrolIndex;

    public float fireRate;
    public float rotationSpeed;
    public float shotSpeed;
    float shotTime;

    public SkinnedMeshRenderer robotRenderer;
    float deathtime = 2.0f;
    bool dead;

    float targetSearchTime = 2.0f;

    private void Start()
    {
        manager = FindObjectOfType<EnemyStatemanager>();
        fmanager = FindObjectOfType<FriendStateManager>();
        gunRenderer = GetComponentInChildren<MeshRenderer>();
        barrell = gunRenderer.transform.GetChild(0);
        robotRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        controller = GetComponent<AgentMove>();
        audioSource = GetComponent<AudioSource>();
        ChooseNextPatrolPosition();
    }

    private void Update()
    {
        if (Vector3.Distance(manager.player.transform.position, transform.position) <= 50f)
        {
            targetSearchTime -= Time.deltaTime;
            if (target == null || targetSearchTime < 0f)
            {
                targetSearchTime = 2.0f;
                target = manager.SearchForTarget(transform.position);
                if (target != null)
                {
                    state = EnemyState.TakingCover;
                }
            }
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
                        state = EnemyState.CoverShooting;
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
                    ShootTarget();
                    break;
                case EnemyState.Dead:
                    controller.Agent.isStopped = true;
                    controller.Animator.SetBool("Dead", true);
                    Die();
                    break;
            }
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
        else
        {
            state = EnemyState.CoverShooting;
        }
    }

    void ShootTarget()
    {
        if (target == null)
        target = manager.SearchForTarget(transform.position);

        if (target != null)
        {
            shotTime -= Time.deltaTime;
            if (!gunShown)
            {
                StartCoroutine(DissolveGunIn());
            }
            gunShown = true;

            Vector3 direction = (target.position - barrell.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            if (shotTime <= 0f)
            {
                Vector3 shotDir = (target.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-2f, 2f), Random.Range(-1f, 1f))) - barrell.position;

                GameObject bul = Instantiate(bullet, barrell.position, Quaternion.identity);
                bul.GetComponent<Rigidbody>().AddForce(shotDir * shotSpeed, ForceMode.Impulse);

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
            {
                fmanager.targets.Remove(this);
                StartCoroutine(DissolvePlayerOut());
                dead = true;
                WorldState.Score += points;
                Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                GameObject obj = Instantiate(add, pos, Quaternion.identity);
                obj.transform.LookAt(manager.player);
                obj.GetComponentInChildren<Text>().text = "+" + points.ToString();
                Destroy(obj, 2.0f);

            }
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