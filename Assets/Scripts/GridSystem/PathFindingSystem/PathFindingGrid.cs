using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PathFindingGrid
{
    private PathFinding pathFinding;
    private Grid<PathNode> grid;

    public PathFindingGrid(int width,int height,float cellSize,Vector3 originPosition) {
        grid = new Grid<PathNode>(width, height, cellSize, originPosition, (i, j) => new PathNode(i, j));
        pathFinding = new PathFinding(grid);
    }
    public List<Vector3> GetPath(Vector3 startPosition,Vector3 endPosition)
    {
        List<PathNode> pathNodes = pathFinding.FindPath(startPosition, endPosition);
        List<Vector3> pathPositions = new List<Vector3>();
        foreach(PathNode pathNode in pathNodes)
        {
            Vector3 nodePosition = grid.GetWorldPosition(pathNode.x, pathNode.y);
            pathPositions.Add(nodePosition);
        }
        return pathPositions;
    }
}
