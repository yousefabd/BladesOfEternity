using CodeMonkey.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TestingPathFinding : MonoBehaviour
{
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;
    [SerializeField] private float cellSize = 1f;

    private Grid<PathNode> grid;
    PathFinding pathFinding;

    private void Start() {
        grid = new Grid<PathNode>(width, height, cellSize, transform.position, (i, j) => new PathNode(i, j), true);
        pathFinding = new PathFinding(grid);
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            Vector2Int endIndices = grid.GetIJ(mousePosition);
            List<PathNode> path = pathFinding.FindPath(0,0,endIndices.x,endIndices.y);
            for(int i=0;i<path.Count - 1; i++) {
                Vector3 pathNodePosition = grid.GetCellWorldPosition(path[i].x,path[i].y);
                Vector3 nextPathPosition = grid.GetCellWorldPosition(path[i + 1].x, path[i + 1].y);
                Debug.DrawLine(pathNodePosition, nextPathPosition, Color.green, 4f);
            }
        }
    }
}
