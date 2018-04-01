using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FriendState
{
    Idle,
    Sneak,
    Cover,
    TakingCover,
    CoverShooting,
    Shooting,
    Downed,
    Reviving
}

public class FriendlyBehaviour : MonoBehaviour
{
    [SerializeField]
    FriendState state;
    public FriendState State
    {
        get { return state; }
        set { state = value; }
    }

    AgentMove controller;
    FriendStateManager manager;

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
    
    public bool CeaseFire { get; set; }

    bool startedReviving = false;

    private void Start()
    {
        controller = GetComponent<AgentMove>();
    }

    private void Update()
    {
        switch (state)
        {
            case FriendState.Idle:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", false);
                break;
            case FriendState.Cover:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", true);
                controller.Animator.SetBool("Shoot", false);
                break;
            case FriendState.TakingCover:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", false);
                if (!hasCoverPoint)
                { TakeCover(); }
                if (Vector3.Distance(transform.position, controller.goal.position) <= 1f)
                {
                    controller.Agent.isStopped = true;
                    controller.Animator.SetBool("Crouch", true);
                    hasCoverPoint = true;
                    if (CeaseFire)
                    {
                        state = FriendState.Cover;
                    }
                    else
                    {
                        state = FriendState.CoverShooting;
                    }
                }
                break;
            case FriendState.Sneak:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", true);
                controller.Animator.SetBool("Shoot", false);
                if (Vector3.Distance(transform.position, controller.goal.position) <= 1f)
                {
                    controller.Agent.isStopped = true;
                    controller.Animator.SetBool("Crouch", true);
                    hasCoverPoint = true;
                    state = FriendState.Cover;
                }
                break;
            case FriendState.Shooting:
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", true);
                ShootTarget();
                break;
            case FriendState.CoverShooting:
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", true);
                controller.Animator.SetBool("Shoot", true);
                break;
            case FriendState.Downed:
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Dead", true);
                break;
            case FriendState.Reviving:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", false);
                if (Vector3.Distance(transform.position, controller.goal.position) <= 2f &&
                    !startedReviving)
                {
                    controller.Agent.isStopped = true;
                    controller.Animator.SetBool("Crouch", true);
                    Invoke("DoneReviving", 2.0f);
                    startedReviving = true;
                }
                break;
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

    public void Revive(Transform member)
    {
        controller.goal = member;
        state = FriendState.Reviving;
        startedReviving = false;
    }

    void DoneReviving()
    {
        state = FriendState.TakingCover;
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