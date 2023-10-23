using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;



public class SnapshotManager : MonoBehaviour
{
    List<Snapshot> snapshotBuffer = new List<Snapshot>();

    public Rigidbody rb;

    public bool wasRewind;
    public bool isRewind;

    public float accumlator;
    public float accumlatorTiming = 0.1f;

    public float snapshotTimeLimit = 5f;

    public Snapshot currentSnapshot = new Snapshot();
    public float currentTiming;


    private void Start()
    {
        snapshotBuffer.Add(new Snapshot(rb.position, rb.rotation, rb.velocity, rb.angularVelocity));
    }

    private void Update()
    {
        wasRewind = isRewind;
        isRewind = Input.GetKey(KeyCode.Space);
        RecordSnapshot();
        RewindSnapshot();
    }

    private void OnDrawGizmos()
    {
        if (snapshotBuffer.Count <= 0) return;

        foreach(Snapshot s in snapshotBuffer)
        {
            Gizmos.DrawCube(s.position, Vector3.one * 2f);
        }
    }

    public void RecordSnapshot()
    {
        if (isRewind)
        {
            return;
        }
        rb.isKinematic = false;

        accumlator += Time.deltaTime;

        if (accumlator > accumlatorTiming)
        {
            accumlator = 0f;
            snapshotBuffer.Add(new Snapshot(rb.position, rb.rotation, rb.velocity,rb.angularVelocity));//, Time.time));
            if (snapshotBuffer.Count * accumlatorTiming > snapshotTimeLimit)
            {
                snapshotBuffer.RemoveAt(0);
            }
        }
    }

    public void RewindSnapshot()
    {
        if (!isRewind || snapshotBuffer.Count - 1 <= 0)
        {
            return;
        }
        rb.isKinematic = true;

        if (isRewind && !wasRewind)
        {
            currentSnapshot = new Snapshot(rb.position, rb.rotation, rb.velocity, rb.angularVelocity);
            currentTiming = accumlator;
        }

        accumlator -= Time.deltaTime;

        if (Mathf.Approximately(0.0f, accumlator) || accumlator < 0)
        {
            accumlator = accumlatorTiming;
            if (snapshotBuffer.Count - 1 <= 0)
                return;
            currentSnapshot = snapshotBuffer[snapshotBuffer.Count - 1];
            snapshotBuffer.RemoveAt(snapshotBuffer.Count - 1);
            currentTiming = accumlatorTiming;
        }

        var t = accumlator / currentTiming;

        var rewindSnapshot = Snapshot.Lerp(snapshotBuffer[snapshotBuffer.Count - 1], currentSnapshot, t);

        rb.position = rewindSnapshot.position;
        rb.rotation = rewindSnapshot.rotation;
    }
}

public struct Snapshot
{
    public Vector3 velocity;
    public Vector3 angularVelocity;
    public Vector3 position;
    public Quaternion rotation;

    public Snapshot(Vector3 p, Quaternion r, Vector3 v, Vector3 a)
    {
        position = p;
        rotation = r;
        velocity = v;
        angularVelocity = a;
    }

    public static Snapshot Lerp(Snapshot pre, Snapshot next, float t)
    {
        var lerpVel = Vector3.Lerp(pre.velocity, next.velocity, t);
        var lerpAngVel = Vector3.Lerp(pre.angularVelocity, next.angularVelocity, t);
        var lerpPos = Vector3.Lerp(pre.position, next.position, t);
        var lerpRot = Quaternion.Slerp(pre.rotation, next.rotation, t);

        return new Snapshot(lerpPos, lerpRot, lerpVel, lerpAngVel);
    }
}
