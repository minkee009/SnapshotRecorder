using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;



public class SnapshotManager : MonoBehaviour
{
    List<Snapshot> testQueue = new List<Snapshot>();

    public Rigidbody rb;

    public bool isRewind;

    public float accumlator;

    float _frameTime;

    private void Update()
    {
        if (isRewind)
        {
            return;
        }

        accumlator += Time.deltaTime;

        if (accumlator > 1/5)
        {
            _frameTime += accumlator;
            Debug.Log(_frameTime);
            accumlator = 0f;
            testQueue.Add(new Snapshot(rb.position, rb.rotation));
            if(testQueue.Count > 50)
            {
                testQueue.Remove(testQueue[0]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (testQueue.Count <= 0) return;

        foreach(Snapshot s in testQueue)
        {
            Gizmos.DrawCube(s.position, Vector3.one * 2f);
        }
        
    }

}

public struct Snapshot
{
    public Snapshot(Vector3 p, Quaternion r)
    {
        position = p;
        rotation = r;
    }

    public Vector3 position;
    public Quaternion rotation;
}
