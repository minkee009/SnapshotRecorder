using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SnapshotManager : MonoBehaviour
{
    Queue<Snapshot> testQueue = new Queue<Snapshot>();

    private void Start()
    {
        
    }
}

public struct Snapshot
{
    public Vector3 position;
    public Quaternion rotation;
}
