using UnityEngine;

public class PathNode
{
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int FCost => gCost + hCost;

    public PathNode parent;

    public PathNode(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public override string ToString() {
        return $"{x}, {y}";
    }
}
