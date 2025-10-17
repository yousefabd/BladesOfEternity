using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PathFindingSystem
{
    private PathFinding pathFinding;
    private Grid<PathNode> grid;

    public PathFindingSystem(int width,int height,float cellSize,Vector3 originPosition) {
        grid = new Grid<PathNode>(width, height, cellSize, originPosition, (i, j) => new PathNode(i, j));
        pathFinding = new PathFinding(grid);
    }
    public List<Vector3> GetPath(Vector3 startPosition,Vector3 endPosition)
    {
        List<PathNode> pathNodes = pathFinding.FindPath(startPosition, endPosition);
        List<Vector3> pathPositions = new List<Vector3>();
        foreach(PathNode pathNode in pathNodes)
        {
            Vector3 nodePosition = grid.GetCellWorldPosition(pathNode.x, pathNode.y);
            pathPositions.Add(nodePosition);
        }
        return pathPositions;
    }
    public List<Vector2Int> GetAvailableMoveIndices(Vector3 position, int moveTiles)
    {
        Vector2Int startIndices = grid.GetIJ(position);

        var visited = new HashSet<Vector2Int>();
        var result = new List<Vector2Int>();
        var queue = new Queue<(Vector2Int, int)>();

        queue.Enqueue((startIndices, moveTiles));
        visited.Add(startIndices);

        while (queue.Count > 0)
        {
            var (currentIndices, remainingMoves) = queue.Dequeue();

            if (remainingMoves < 0) continue;

            result.Add(currentIndices);

            Vector2Int[] directions = new Vector2Int[] {
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1)
            };

            foreach (var dir in directions)
            {
                Vector2Int neighbour = currentIndices + dir;

                // Bounds check
                if (neighbour.x < 0 || neighbour.y < 0 || neighbour.x >= grid.GetWidth() || neighbour.y >= grid.GetHeight())
                    continue;

                // Skip visited or obstacle tiles
                if (visited.Contains(neighbour) || pathFinding.IsObstacle(grid.GetGridObject(neighbour.x, neighbour.y)))
                    continue;

                queue.Enqueue((neighbour, remainingMoves - 1));
                visited.Add(neighbour);
            }
        }

        return result;
    }
    public List<Vector2Int> GetAvailableInteractIndices(Vector3 position, int interactTiles)
    {
        Vector2Int startIndices = grid.GetIJ(position);
        List<Vector2Int> result = new();

        for (int i = startIndices.x - interactTiles; i <= startIndices.x + interactTiles; i++)
        {
            for (int j = startIndices.y - interactTiles; j < startIndices.y + interactTiles; j++)
            {
                int distance = Mathf.Abs(i - startIndices.x) + Mathf.Abs(j - startIndices.y);
                if (i < 0 || i >= grid.GetWidth() || j < 0 || j >= grid.GetHeight() || distance > interactTiles)
                    continue;
                if (pathFinding.IsObstacle(grid.GetGridObject(i, j)))
                    continue;
                result.Add(new Vector2Int(i, j));
            }
        }
        return result;
    }
}
