using CodeMonkey.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler_TurnBasedSystem : MonoBehaviour
{
    public static GameHandler_TurnBasedSystem Instance { get; private set; }
    [SerializeField] private Transform Map;
    [SerializeField] private List<TeamSO> teams;

    [SerializeField] private Vector3 testPosition1;
    [SerializeField] private Vector3 testPosition2;
    [SerializeField] private CombatUnit testUnit;

    private PathFindingGrid pathFindingGrid;
    private TurnBasedSystem turnBasedSystem;
    
    private void Awake() {
        Instance = this;

        Transform mapTransform = Instantiate(Map);
        MapSettings mapSettings = mapTransform.GetComponent<MapSettings>();

        CombatUnit playerTest = Instantiate(testUnit, testPosition1, Quaternion.identity);
        playerTest.SetTeam(teams[0]);
        CombatUnit enemyTest  = Instantiate(testUnit, testPosition2, Quaternion.identity);
        enemyTest.SetTeam(teams[1]);   
        pathFindingGrid = new PathFindingGrid(mapSettings.Width, mapSettings.Height,mapSettings.cellSize, mapSettings.Origin.position);
        turnBasedSystem = new TurnBasedSystem(teams, new List<CombatUnit> { playerTest, enemyTest });
    }

    public List<Vector3> GetMovePath(Vector3 start,Vector3 end)
    {
        return pathFindingGrid.GetPath(start, end);
    }
}
