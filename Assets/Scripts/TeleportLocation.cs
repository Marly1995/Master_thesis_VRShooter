using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportLocation : MonoBehaviour
{
    public GameObject particles;
    public MeshRenderer mesh;

    public Material activeMat;
    public Material basicMat;

    bool active;
    public bool Active
    {
        get { return active; }
        set { active = value; }
    }
    
	void Start ()
    {
        active = false;
	}
	
	void Update ()
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
