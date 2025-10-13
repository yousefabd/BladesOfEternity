using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MapSettings : MonoBehaviour
{
    [SerializeField] public int Width;
    [SerializeField] public int Height;
    [SerializeField] public int cellSize = 1;
    [SerializeField] public Transform Origin;
    [SerializeField] private List<SpawnPoint> TeamSpawnPoints;
    public List<SpawnPoint> GetTeamSpawnPoints()
    {
        return TeamSpawnPoints;
    }

}
