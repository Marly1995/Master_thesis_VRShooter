using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinuousTracker : MonoBehaviour
{
    public float checkTime;
    float lastTime;

    public float velocityThreshold;
    public float distanceThreshold;

    public float vel;
    public float dist;

    MousePositionRecorder rec;

    Vector3 lastPos;

    Queue<Vector3> positions;
    Queue<float> vels;

    public bool started = false;

    void Start()
    {
        positions = new Queue<Vector3>();
        vels = new Queue<float>();
        rec = GetComponent<MousePositionRecorder>();
        lastPos = rec.rightHand.transform.position;
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            positions.Clear();
            vels.Clear();
        }
        else
        {
			if (!started)
			{
				positions.Enqueue(rec.rightHand.position);
				if (positions.Count > 4)
				{
					positions.Dequeue();
				}


				vels.Enqueue((rec.rightHand.position - lastPos).magnitude);
				lastPos = rec.rightHand.position;
				if (vels.Count > 4)
				{
					vels.Dequeue();
				}
                
				if (Time.time - checkTime >= lastTime)
				{
					lastTime = Time.time;
					if (!PositionCheck())
					{
						rec.BeginRecording();
						StartCoroutine(GestureStarted());
					}
				}
			}
			else
			{
				positions.Clear();
				vels.Clear();
			}
        }
    }

    IEnumerator GestureStarted()
    {
        rec.continuousGesturing = true;
        started = true;

        yield return new WaitForSeconds(1.2f);

        bool hit = rec.ContinuousCheckRecognized();
		
			rec.continuousGesturing = false;
			rec.constantPositions.Clear();
			started = false;
			rec.EndRecording();
            positions.Clear();
            vels.Clear();
    }

    bool VelocityCheck()
    {
        float total = 0f;
        foreach (float item in vels)
        {
            total += item;
        }
        total /= vels.Count;
        vel = total;
        return total > velocityThreshold ? true : false;
    }

    bool PositionCheck()
    {
        Vector3 average = Vector3.zero;
        foreach (Vector3 item in positions)
        {
            average += item;
        }
        average = new Vector3(average.x / (float)positions.Count, average.y / (float)positions.Count, average.z / (float)positions.Count);

        foreach (Vector3 item in positions)
        {
            dist = Vector3.Distance(average, item);
            if (dist > distanceThreshold)
            { return true; }
        }
        return false;
    }
}