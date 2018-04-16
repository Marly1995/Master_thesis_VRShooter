using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour {

    [Space]
    public GameObject player;
    
    GameObject teleportLocation = null;
    GameObject currentLocation = null;

    public GameObject locationsParent;
    bool visivble = false;

    Ray ray;
    RaycastHit hit;

    public VolumetricLines.VolumetricLineBehavior line;

    void Update ()
    {
        ray = new Ray(transform.position, transform.forward);
        if (OVRInput.Get(OVRInput.Button.Three))
        {
            if (!visivble)
            {
                visivble = true;
                locationsParent.SetActive(visivble);
                line.LineWidth = 0.1f;
            }
            LookForTeleport();
        }

        if (OVRInput.GetUp(OVRInput.Button.Three))
        {
            visivble = false;
            line.LineWidth = 0.0f;
            StartCoroutine(SwitchVisibility());
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
                line.LineColor = Color.cyan;
            }
            else
            {
                if (teleportLocation != null)
                {
                    teleportLocation.GetComponent<TeleportLocation>().Active = false;
                }
                teleportLocation = null;
                line.LineColor = Color.magenta;
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
            teleportLocation.GetComponent<TeleportLocation>().SetIndex();

            if (currentLocation != null)
            { currentLocation.SetActive(true); }
            currentLocation = teleportLocation;
            currentLocation.SetActive(false);
            teleportLocation = null;
        }
    }

    IEnumerator SwitchVisibility()
    {
        yield return new WaitForSeconds(0.3f);
        locationsParent.SetActive(visivble);
    }
}
