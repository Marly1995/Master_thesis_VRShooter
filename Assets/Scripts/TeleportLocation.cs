using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportLocation : MonoBehaviour
{
    public int index;

    public GameObject particles;
    public MeshRenderer mesh;

    public Material activeMat;
    public Material basicMat;
    public Material oldMat;

    bool active;
    public bool Active
    {
        get { return active; }
        set { active = value; }
    }

    bool old;
    
	void Start ()
    {
        active = false;
        old = false;
	}
	
	void Update ()
    {
        if (!old)
        {
            if (active)
            {
                particles.SetActive(true);
                mesh.material = activeMat;
            }
            else
            {
                particles.SetActive(false);
                mesh.material = basicMat;
            }
        }
    }

    public void SetIndex()
    {
        WorldState.TeleportIndex = index;
        old = true;
        mesh.material = oldMat;
    }
}
