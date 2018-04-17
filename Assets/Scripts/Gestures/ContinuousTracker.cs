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
            positions.Enqueue(rec.rightHand.position);
            if (positions.Count > 10)
            {
                positions.Dequeue();
            }


            vels.Enqueue((rec.rightHand.position - lastPos).magnitude);
            lastPos = rec.rightHand.position;
            if (vels.Count > 10)
            {
                vels.Dequeue();
            }

            if (Time.time - checkTime >= lastTime)
            {
                lastTime = Time.time;
                if (!started)
                {
                    //if (!VelocityCheck())
                    {
                        if (!PositionCheck())
                        {
                            rec.BeginRecording();
                            StartCoroutine(GestureStarted());
                        }
                    }
                }
            }
        }
    }

    IEnumerator GestureStarted()
    {
        rec.continuousGesturing = true;
        started = true;

        yield return new WaitForSeconds(2.0f);

        rec.continuousGesturing = false;
        rec.ContinuousCheckRecognized();
        rec.constantPositions.Clear();
        started = false;
        rec.EndRecording();
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