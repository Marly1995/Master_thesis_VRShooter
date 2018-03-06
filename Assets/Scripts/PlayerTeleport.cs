using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour {

    [Space]
    public GameObject player;
    
    GameObject teleportLocation = null;
    GameObject currentLocation = null;

    Ray ray;
    RaycastHit hit;

    void Update ()
    {
        ray = new Ray(transform.position, transform.forward);

        if (OVRInput.Get(OVRInput.Button.One))
        {
            LookForTeleport();
        }

        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            DoTeleport();
        }
    }

    void LookForTeleport()
    {
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "teleportPoint")
            {
                teleportLocation = hit.collider.gameObject;
                teleportLocation.GetComponent<TeleportLocation>().Active = true;
            }
            else
            {
                if (teleportLocation != null)
                {
                    teleportLocation.GetComponent<TeleportLocation>().Active = false;
                }
                teleportLocation = null;
            }
        }
        else 
        {
            if (teleportLocation != null)
            {
                teleportLocation.GetComponent<TeleportLocation>().Active = false;
            }
            teleportLocation = null;
        }
    }

    void DoTeleport()
    {
        if (teleportLocation)
        {
            player.transform.position = teleportLocation.transform.position;
            teleportLocation.GetComponent<TeleportLocation>().Active = false;

            if (currentLocation != null)
            { currentLocation.SetActive(true); }
            currentLocation = teleportLocation;
            currentLocation.SetActive(false);
            teleportLocation = null;
        }
    }
}
