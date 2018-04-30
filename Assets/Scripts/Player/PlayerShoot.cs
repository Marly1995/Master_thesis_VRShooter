using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerShoot : MonoBehaviour
{
    public Texture grey;
    public Texture gunTex;

    public AudioClip lazer;
    public AudioClip disabled;

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
    bool down = false;

    public GameObject noShoot;
    public GameObject noShoot2;

    public PostProcessingProfile posteffects;
    ColorGradingModel.Settings colorComp;

    float postExposure = 5f;
    float targetExposure = -1.45f;

    void HandleInput()
    {
        if (WorldState.PlayerDown && !down)
        {
            gunRenderer.material.SetTexture("_MainTex", grey);
            lazerShot.clip = disabled;
            lazerShot.volume = 1.0f;
            lazerShot.Play();
            down = true;
            noShoot.SetActive(true);
            noShoot2.SetActive(true);
            postExposure = 5.0f;

            //copy current bloom settings from the profile into a temporary variable
            colorComp = posteffects.colorGrading.settings;

            //change the intensity in the temporary settings variable
            colorComp.basic.postExposure = postExposure;

            //set the bloom settings in the actual profile to the temp settings with the changed value
            posteffects.colorGrading.settings = colorComp;

            StartCoroutine(DownTime());
            StartCoroutine(Flash());
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
        {
            grabbed = true;
            //gun.transform.parent = transform;
            //gun.transform.localPosition = Vector3.zero;
            //gun.transform.localRotation = Quaternion.identity;
            StartCoroutine(DissolveIn());
        }
        else if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            grabbed = false;
            //gun.transform.parent = null;
            StartCoroutine(DissolveOut());
        }
        if (grabbed)
        {
            if (!down)
            {
                if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) &&
                    shotTimer <= 0)
                {
                    Fire();
                    shotTimer = fireRate;
                }
            }
        }
    }
	
	void FixedUpdate ()
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
            if (hit.collider.tag == "bullet")
            {
                Destroy(hit.collider.gameObject);
            }
        }
        GameObject trail = Instantiate(hitTrail);
        VolumetricLines.VolumetricLineBehavior vol = trail.GetComponent<VolumetricLines.VolumetricLineBehavior>();
        vol.StartPos = barrellStart.position;
        vol.EndPos = barrelEnd.position;
        lazerShot.clip = lazer;
        lazerShot.volume = 0.5f;
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

    IEnumerator Flash()
    {
        while(postExposure > targetExposure)
        {
            postExposure -= 0.2f;
            //copy current bloom settings from the profile into a temporary variable
            colorComp = posteffects.colorGrading.settings;

            //change the intensity in the temporary settings variable
            colorComp.basic.postExposure = postExposure;

            //set the bloom settings in the actual profile to the temp settings with the changed value
            posteffects.colorGrading.settings = colorComp;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DownTime()
    {
        yield return new WaitForSeconds(3.0f);
        gunRenderer.material.SetTexture("_MainTex", gunTex);
        down = false;
        WorldState.PlayerDown = false;
        noShoot.SetActive(false);
        noShoot2.SetActive(false);
    }
}
