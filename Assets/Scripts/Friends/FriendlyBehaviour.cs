﻿using System.Collections;
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
    AudioSource audioSource;
    public AudioClip laserSound;

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
    MeshRenderer gunRenderer;
    Transform barrell;
    public GameObject hitParticles;
    public GameObject hitTrail;

    public Transform target;

    public Transform[] coverPositions;
    bool hasCoverPoint = false;

    public float fireRate;
    public float rotationSpeed;
    float shotTime;
    
    public bool CeaseFire { get; set; }

    bool startedReviving = false;
    FriendlyBehaviour reviveTarget;

    public bool downed = false;

    private void Start()
    {
        manager = FindObjectOfType<FriendStateManager>();
        gunRenderer = GetComponentInChildren<MeshRenderer>();
        barrell = gunRenderer.transform.GetChild(0);
        controller = GetComponent<AgentMove>();
        audioSource = GetComponent<AudioSource>();
        CeaseFire = false;
    }

    private void Update()
    {
        if (downed)
        {
            state = FriendState.Downed;
        }
        switch (state)
        {
            case FriendState.Idle:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Dead", false);
                controller.Animator.SetBool("Shoot", false);
                break;
            case FriendState.Cover:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", true);
                controller.Animator.SetBool("Shoot", false);
                controller.Animator.SetBool("Dead", false);
                break;
            case FriendState.TakingCover:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", false);
                controller.Animator.SetBool("Dead", false);
                if (!hasCoverPoint)
                { TakeCover(); }
                if (controller.goal != null &&
                    Vector3.Distance(transform.position, controller.goal.position) <= 1f)
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
                controller.Animator.SetBool("Dead", false);
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
                controller.Animator.SetBool("Dead", false);
                ShootTarget();
                break;
            case FriendState.CoverShooting:
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Crouch", true);
                controller.Animator.SetBool("Shoot", true);
                controller.Animator.SetBool("Dead", false);
                ShootTarget();
                break;
            case FriendState.Downed:
                controller.Agent.isStopped = true;
                controller.Animator.SetBool("Dead", true);
                downed = true;
                break;
            case FriendState.Reviving:
                if (gunShown)
                    StartCoroutine(DissolveOut());
                gunShown = false;
                controller.Agent.isStopped = false;
                controller.Animator.SetBool("Crouch", false);
                controller.Animator.SetBool("Shoot", false);
                controller.Animator.SetBool("Dead", false);
                if (Vector3.Distance(transform.position, controller.goal.position) <= 4f &&
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

    public void RemoveCurrentCover()
    {
        hasCoverPoint = false;
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

    public void Revive(FriendlyBehaviour member)
    {
        controller.goal = member.transform;
        reviveTarget = member;
        state = FriendState.Reviving;
        startedReviving = false;
    }

    void DoneReviving()
    {
        state = FriendState.TakingCover;
        reviveTarget.State = FriendState.TakingCover;
        reviveTarget.downed = false;
        reviveTarget = null;
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
                StartCoroutine(DissolveIn());
            }
            gunShown = true;

            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            if (shotTime <= 0f)
            {
                Vector3 shotDir = (target.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-2f, 2f), Random.Range(-1f, 1f))) - barrell.position;
                Ray ray = new Ray(barrell.position, shotDir);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject particles = Instantiate(hitParticles, hit.point, Quaternion.identity);
                    Destroy(particles, 0.4f);
                    if (hit.collider.tag == "Enemy")
                    {
                        hit.collider.gameObject.GetComponent<EnemyBehaviours>().points = 1000;
                        hit.collider.gameObject.GetComponent<EnemyBehaviours>().State = EnemyState.Dead;
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