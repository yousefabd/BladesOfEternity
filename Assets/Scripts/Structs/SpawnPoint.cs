using System;
using UnityEngine;
[Serializable]
public struct SpawnPoint
{
    public Transform start;
    public Transform end;
    public readonly void GetSpawnPoint(out Vector3 startPosition, out Vector3 endPosition)
    {
        startPosition = start.transform.position;
        endPosition = end.transform.position;
    }
}