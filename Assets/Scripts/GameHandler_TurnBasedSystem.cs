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
    [SerializeField] private TileInteractionSystem tileInteractionSystemPrefab;

    private PathFindingSystem pathFindingSystem;
    private TurnBasedSystem turnBasedSystem;
    private TileInteractionSystem tileInteractionSystem;

    MapSettings mapSettings;
    private Grid<EmptyGridObject> mapGrid;
    
    private void Awake() {
        Instance = this;

        Transform mapTransform = Instantiate(Map);
        mapSettings = mapTransform.GetComponent<MapSettings>();
        pathFindingSystem = new PathFindingSystem(mapSettings.Width, mapSettings.Height,mapSettings.cellSize, mapSettings.Origin.position);
        mapGrid = new Grid<EmptyGridObject>(mapSettings.Width, mapSettings.Height, mapSettings.cellSize, mapSettings.Origin.position, (int x, int y) => new EmptyGridObject(x,y));
    }
    private void Start()
    {
        List<TeamSO> gameTeams = teamsCombatUnits.Select(tcu => tcu.team).ToList();
        List<CombatUnit> spawnedUnits = SpawnUnitsOnMap(mapSettings);
        SetupTurnBasedSystem(gameTeams, spawnedUnits);
        SetupTileInteractionSystem(mapSettings);
    }
    private List<CombatUnit> SpawnUnitsOnMap(MapSettings mapSettings)
    {
        List<CombatUnit> spawnedUnits = new List<CombatUnit>();
        List<SpawnPoint> teamSpawnPoints = mapSettings.GetTeamSpawnPoints();
        for (int k = 0; k < teamSpawnPoints.Count; k++)
        {
            Vector2Int startIndices = mapGrid.GetIJ(teamSpawnPoints[k].start.position);
            Vector2Int endIndices = mapGrid.GetIJ(teamSpawnPoints[k].end.position);
            List<CombatUnitSO> unitsToSpawn = teamsCombatUnits[k].units;
            for (int i = startIndices.y, u = 0; i >= endIndices.y && u < unitsToSpawn.Count; i -= 2)
            {
                for (int j = startIndices.x; j <= endIndices.x && u < unitsToSpawn.Count; j += 2, u++)
                {
                    Vector3 unitSpawnPosition = mapGrid.GetCellWorldPosition(j, i);
                    Transform unitPrefab = unitsToSpawn[u].prefab;
                    Transform unitTransform = Instantiate(unitPrefab, unitSpawnPosition, Quaternion.identity);
                    CombatUnit spawnedUnit = unitTransform.GetComponent<CombatUnit>();
                    spawnedUnits.Add(spawnedUnit);
                }
            }
        }
        return spawnedUnits;
    }
    private void SetupTurnBasedSystem(List<TeamSO> gameTeams, List<CombatUnit> combatUnits)
    {
        turnBasedSystem = new TurnBasedSystem(gameTeams, combatUnits);
        turnBasedSystem.OnMoveUnit += TurnBasedSystem_OnMoveUnit;
        turnBasedSystem.OnAutoSelectUnit += TurnBasedSystem_OnAutoSelectUnit;
    }


    private void SetupTileInteractionSystem(MapSettings mapSettings)
    {
        tileInteractionSystem = TileInteractionSystem.Init(
            tileInteractionSystemPrefab,
            transform, mapSettings.Width,
            mapSettings.Height,
            mapSettings.cellSize,
            mapSettings.Origin.position);
        tileInteractionSystem.OnClickUnit += TileInteractionSystem_OnClickUnit;
        tileInteractionSystem.OnClickEmptyTile += TileInteractionSystem_OnClickEmptyTile;
        tileInteractionSystem.OnMoveUnit += TileInteractionSystem_OnMoveUnit;
        tileInteractionSystem.OnAttackUnit += TileInteractionSystem_OnAttackUnit;
        tileInteractionSystem.OnSkipTurn += TileInteractionSystem_OnSkipTurn;

    }

    private void TileInteractionSystem_OnSkipTurn()
    {
        turnBasedSystem.ClearCurrentUnitMoveActions();
    }

    private void TileInteractionSystem_OnAttackUnit(CombatUnit targetUnit)
    {
        turnBasedSystem.AttackUnit(targetUnit);
    }

    private void TileInteractionSystem_OnClickUnit(CombatUnit unit,MoveAction moveAction)
    {
        if (turnBasedSystem.SelectUnit(unit, moveAction))
        {
            switch(moveAction)
            {
                case MoveAction.Move:
                    int unitMoveTiles = unit.GetCombatUnitSO().moveTiles;
                    List<Vector2Int> walkTiles = pathFindingSystem.GetAvailableMoveIndices(unit.transform.position, unitMoveTiles);
                    tileInteractionSystem.SetActionTilesList(walkTiles, ActionTile.TileType.Walk);
                    break;
                case MoveAction.Attack:
                    int unitAttackTiles = unit.GetCombatUnitSO().attackTiles;
                    List<Vector2Int> attackTiles = pathFindingSystem.GetTeamAvailableInteractIndices(unit.transform.position, unit.GetTeam(), unitAttackTiles);
                    tileInteractionSystem.SetActionTilesList(attackTiles, ActionTile.TileType.Attack);
                    break;
            }
        }
    }

    private void TileInteractionSystem_OnMoveUnit(Vector3 destination)
    {
        turnBasedSystem.MoveUnit(destination);
    }

    private void TileInteractionSystem_OnClickEmptyTile()
    {
        turnBasedSystem.DeselectUnit();
    }
    private void TurnBasedSystem_OnMoveUnit(CombatUnit unit, Vector3 destination)
    {
        List<Vector3> movePath = GetMovePath(unit.transform.position, destination);
        unit.MovePath(movePath);
    }
    private void TurnBasedSystem_OnAutoSelectUnit(CombatUnit unit, MoveAction moveAction)
    {
        switch (moveAction)
        {
            case MoveAction.Move:
                int unitMoveTiles = unit.GetCombatUnitSO().moveTiles;
                List<Vector2Int> walkTiles = pathFindingSystem.GetAvailableMoveIndices(unit.transform.position, unitMoveTiles);
                tileInteractionSystem.SetActionTilesList(walkTiles, ActionTile.TileType.Walk);
                break;
            case MoveAction.Attack:
                int unitAttackTiles = unit.GetCombatUnitSO().attackTiles;
                List<Vector2Int> attackTiles = pathFindingSystem.GetTeamAvailableInteractIndices(unit.transform.position, unit.GetTeam(), unitAttackTiles);
                tileInteractionSystem.SetActionTilesList(attackTiles, ActionTile.TileType.Attack);
                break;
        }
    }

    private List<Vector3> GetMovePath(Vector3 start,Vector3 end)
    {
        return pathFindingSystem.GetPath(start, end);
    }
}
