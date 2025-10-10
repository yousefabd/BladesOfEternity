using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathFinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private Grid<PathNode> grid;

    public PathFinding(Grid<PathNode> grid) {
        this.grid = grid;
    }

    public List<PathNode> FindPath(Vector3 start,Vector3 end)
    {
        Vector2Int startIndices = grid.GetIJ(start);
        Vector2Int endIndices = grid.GetIJ(end);
        return FindPath(startIndices.x, startIndices.y, endIndices.x, endIndices.y);
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY) {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);
        if(endNode == null) {
            return new List<PathNode>();
        }

        List<PathNode> openList = new List<PathNode> { startNode };
        List<PathNode> closedList = new List<PathNode>();

        for (int i=0;i<grid.GetWidth();i++) {
            for(int j=0;j<grid.GetHeight();j++) {
                PathNode pathNode = grid.GetGridObject(i, j);
                pathNode.gCost = int.MaxValue;
                pathNode.parent = null;
            }
        }
        startNode.gCost = 0;
        startNode.hCost = CalculateManhattanDistance(startNode, endNode);

        while(openList.Count > 0) {
            PathNode currentNode = openList.OrderBy(node => node.FCost).FirstOrDefault();
            if (currentNode.Equals(endNode)) {
                return CalculatePath(endNode);
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode adjacent in GetAdjacentNodes(currentNode)) {
                if (closedList.Contains(adjacent)) continue;
                if (IsObstacle(adjacent)) {
                    closedList.Add(adjacent);
                    continue;
                }
                int tentativeGCost = currentNode.gCost + CalculateManhattanDistance(currentNode, adjacent);
                if(tentativeGCost < adjacent.gCost) {
                    adjacent.parent = currentNode;
                    adjacent.gCost = tentativeGCost;
                    adjacent.hCost = CalculateManhattanDistance(adjacent, endNode);

                    if(!openList.Contains(adjacent)) openList.Add(adjacent);
                }
            }

        }
        return new List<PathNode>();
    }
    public bool IsObstacle(PathNode pathNode) {
        Vector3 worldPosition = grid.GetCellWorldPosition(pathNode.x, pathNode.y);
        float boxSize = grid.GetCellSize() - grid.GetCellSize() * 0.5f;
        if (Physics2D.OverlapBox(worldPosition, new Vector2(boxSize,boxSize),0f)) { 
            return true;
        }
        return false;
    }
    private List<PathNode> GetAdjacentNodes(PathNode node) {
        List<PathNode> adjacentNodes = new List<PathNode>();

        List<Vector2Int> moveDirs = new List<Vector2Int> {
            new Vector2Int(-1, -1),
            new Vector2Int( 0, -1),
            new Vector2Int( 1, -1),
            new Vector2Int(-1,  0),
            new Vector2Int( 1,  0),
            new Vector2Int(-1,  1),
            new Vector2Int( 0,  1),
            new Vector2Int( 1,  1),
        };
        foreach(var dir in moveDirs) {
            int adjX = node.x + dir.x;
            int adjY = node.y + dir.y;
            if (adjX < 0 || adjY < 0 || adjX >= grid.GetWidth() || adjY >= grid.GetHeight()) {
                continue;
            }
            adjacentNodes.Add(grid.GetGridObject(adjX, adjY));
        }
        return adjacentNodes;
    }
    private int CalculateManhattanDistance(PathNode a,PathNode b) {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return Mathf.Min(xDistance, yDistance) * MOVE_DIAGONAL_COST + remaining * MOVE_STRAIGHT_COST;
    }
    private List<PathNode> CalculatePath(PathNode endNode) {
        List<PathNode> path = new List<PathNode> { endNode };
        PathNode currentNode = endNode;
        while(currentNode.parent != null) {
            path.Add(currentNode.parent);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
}