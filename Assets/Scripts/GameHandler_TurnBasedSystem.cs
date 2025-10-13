using CodeMonkey.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameHandler_TurnBasedSystem : MonoBehaviour
{
    public static GameHandler_TurnBasedSystem Instance { get; private set; }
    [SerializeField] private Transform Map;
    [SerializeField] private List<TeamCombatUnits> teamsCombatUnits;

    private PathFindingGrid pathFindingGrid;
    private TurnBasedSystem turnBasedSystem;

    MapSettings mapSettings;
    private Grid<EmptyGridObject> mapGrid;
    
    private void Awake() {
        Instance = this;

        Transform mapTransform = Instantiate(Map);
        mapSettings = mapTransform.GetComponent<MapSettings>();

        pathFindingGrid = new PathFindingGrid(mapSettings.Width, mapSettings.Height,mapSettings.cellSize, mapSettings.Origin.position);
        mapGrid = new Grid<EmptyGridObject>(mapSettings.Width, mapSettings.Height, mapSettings.cellSize, mapSettings.Origin.position, (int x, int y) => new EmptyGridObject(x,y));

    }
    private void Start()
    {
        List<CombatUnit> spawnedUnits = new List<CombatUnit>();
        List<TeamSO> gameTeams = teamsCombatUnits.Select(tcu => tcu.team).ToList();
        List<SpawnPoint> teamSpawnPoints = mapSettings.GetTeamSpawnPoints();
        for (int k = 0; k < teamSpawnPoints.Count; k++) 
        {
            Vector2Int startIndices = mapGrid.GetIJ(teamSpawnPoints[k].start.position);
            Vector2Int endIndices = mapGrid.GetIJ(teamSpawnPoints[k].end.position);
            List<CombatUnitSO> unitsToSpawn = teamsCombatUnits[k].units;
            for (int i = startIndices.y, u = 0; i >= endIndices.y && u < unitsToSpawn.Count; i -= UnityEngine.Random.Range(2, 5)) 
            {
                for (int j = startIndices.x; j <= endIndices.x && u < unitsToSpawn.Count; j += UnityEngine.Random.Range(2, 5), u++) 
                {
                    Vector3 unitSpawnPosition = mapGrid.GetCellWorldPosition(j, i);
                    Transform unitPrefab = unitsToSpawn[u].prefab;
                    Transform unitTransform = Instantiate(unitPrefab, unitSpawnPosition, Quaternion.identity);
                    CombatUnit spawnedUnit = unitTransform.GetComponent<CombatUnit>();
                    spawnedUnits.Add(spawnedUnit);
                }
            }
        }
        
        turnBasedSystem = new TurnBasedSystem(gameTeams, spawnedUnits);
        turnBasedSystem.OnMoveUnit += TurnBasedSystem_OnMoveUnit;
    }

    private void TurnBasedSystem_OnMoveUnit(CombatUnit unit, Vector3 destination)
    {
        List<Vector3> movePath = GetMovePath(unit.transform.position, destination);
        unit.MovePath(movePath);
    }

    public List<Vector3> GetMovePath(Vector3 start,Vector3 end)
    {
        return pathFindingGrid.GetPath(start, end);
    }
}
