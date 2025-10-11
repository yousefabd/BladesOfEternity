using CodeMonkey.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler_TurnBasedSystem : MonoBehaviour
{
    public static GameHandler_TurnBasedSystem Instance { get; private set; }
    [SerializeField] private Transform Map;
    [SerializeField] private List<TeamSO> teams;

    [SerializeField] private Vector2Int gridTestPosition1;
    [SerializeField] private Vector2Int gridTestPosition2;
    [SerializeField] private CombatUnit testHuman;
    [SerializeField] private CombatUnit testGoblin;

    private PathFindingGrid pathFindingGrid;
    private TurnBasedSystem turnBasedSystem;
    private Grid<EmptyGridObject> mapGrid;
    
    private void Awake() {
        Instance = this;

        Transform mapTransform = Instantiate(Map);
        MapSettings mapSettings = mapTransform.GetComponent<MapSettings>();

        pathFindingGrid = new PathFindingGrid(mapSettings.Width, mapSettings.Height,mapSettings.cellSize, mapSettings.Origin.position);
        mapGrid = new Grid<EmptyGridObject>(mapSettings.Width, mapSettings.Height, mapSettings.cellSize, mapSettings.Origin.position, (int x, int y) => new EmptyGridObject(x,y));

    }
    private void Start()
    {
        Vector3 testPosition1 = mapGrid.GetCellWorldPosition(gridTestPosition1.x, gridTestPosition1.y);
        Vector3 testPosition2 = mapGrid.GetCellWorldPosition(gridTestPosition2.x, gridTestPosition2.y);
        CombatUnit playerTest = Instantiate(testHuman, testPosition1, Quaternion.identity);
        CombatUnit enemyTest = Instantiate(testGoblin, testPosition2, Quaternion.identity);
        turnBasedSystem = new TurnBasedSystem(teams, new List<CombatUnit> { playerTest, enemyTest });
    }

    public List<Vector3> GetMovePath(Vector3 start,Vector3 end)
    {
        return pathFindingGrid.GetPath(start, end);
    }
}
