using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
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

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= 4f)
        {
            if(rb.velocity.magnitude > velLimit)
                rb.velocity *= 0.2f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Friendly")
        {
            collision.gameObject.GetComponent<FriendlyBehaviour>().State = FriendState.Downed;
        }
        if (collision.transform.tag == "MainCamera")
        {
            WorldState.PlayerDown = true;
        }
        Hit();
    }

    private void Hit()
    {
        Destroy(Instantiate(hitParticles, transform.position, transform.rotation), 0.4f);
        Destroy(gameObject, 0.1f);
    }
}
