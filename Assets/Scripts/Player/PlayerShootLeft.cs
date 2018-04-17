using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootLeft : MonoBehaviour {

    public AudioSource lazerShot;

    public float fireRate;
    float shotTimer;

    [Space]
    public GameObject gun;
    public MeshRenderer gunRenderer;

    [Space]
    public GameObject hitParticles;
    public GameObject hitTrail;
    public Transform barrellStart;
    public Transform barrelEnd;

    [Space]
    [SerializeField]
    OculusHaptics haptics;

    Ray ray;
    RaycastHit hit;

    bool grabbed = true;

    void HandleInput()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            grabbed = true;
            //gun.transform.parent = transform;
            //gun.transform.localPosition = Vector3.zero;
            //gun.transform.localRotation = Quaternion.identity;
            StartCoroutine(DissolveIn());
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
        {
            grabbed = false;
            //gun.transform.parent = null;
            StartCoroutine(DissolveOut());
        }
        if (grabbed)
        {
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) &&
            shotTimer <= 0)
            {
                Fire();
                shotTimer = fireRate;
            }
        }
    }

    void FixedUpdate()
    {
        shotTimer -= Time.fixedDeltaTime;
        ray = new Ray(barrellStart.position, barrellStart.forward);

        HandleInput();
    }

    void Fire()
    {
        if (Physics.Raycast(ray, out hit))
        {
            haptics.Vibrate(VibrationForce.Medium);
            GameObject particles = Instantiate(hitParticles, hit.point, Quaternion.identity);
            Destroy(particles, 0.4f);
            if (hit.collider.tag == "Enemy")
            {
                hit.collider.gameObject.GetComponent<EnemyBehaviours>().State = EnemyState.Dead;
            }
            if (hit.collider.tag == "score")
            {
                hit.collider.gameObject.GetComponent<EnemyDie>().Die();
            }
        }
        GameObject trail = Instantiate(hitTrail);
        VolumetricLines.VolumetricLineBehavior vol = trail.GetComponent<VolumetricLines.VolumetricLineBehavior>();
        vol.StartPos = barrellStart.position;
        vol.EndPos = barrelEnd.position;
        lazerShot.Play();
    }

    IEnumerator DissolveOut()
    {
        for (float i = 0f; i <= 1f; i += 0.02f)
        {
            if (!grabbed)
            {
                gunRenderer.material.SetFloat("_SliceAmount", i);
                yield return new WaitForSeconds(0.001f);
            }
        }
    }

    IEnumerator DissolveIn()
    {
        for (float i = 1f; i >= -0.1f; i -= 0.02f)
        {
            if (grabbed)
            {
                gunRenderer.material.SetFloat("_SliceAmount", i);
                yield return new WaitForSeconds(0.001f);
            }
        }
    }
}
