using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyBullet : MonoBehaviour
{
    public GameObject hitParticles;
    Transform player;

    Rigidbody rb;

    public float velLimit;
    public float upperVelLimit = 8f;

    private void Awake()
    {
        player = FindObjectOfType<OVRPlayerController>().transform;
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyBehaviours>().State = EnemyState.Dead;
        }
        Hit();
    }

    private void Hit()
    {
        Destroy(Instantiate(hitParticles, transform.position, transform.rotation), 0.4f);
        Destroy(gameObject, 0.1f);
    }
}
