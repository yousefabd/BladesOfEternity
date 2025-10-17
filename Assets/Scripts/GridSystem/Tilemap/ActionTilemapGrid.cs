using UnityEngine;

public class ActionTilemapGrid
{
    private Grid<ActionTile> grid;
    public ActionTilemapGrid(int width, int height, float cellSize, Vector3 originPosition,ActionTileVisual actionTilePrefab)
    {
        grid = new Grid<ActionTile>(width, height, cellSize, originPosition, (x, y) => new ActionTile(x, y, grid));
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Vector3 tileWorldPosition = grid.GetCellWorldPosition(i, j);
                ActionTileVisual.Init(actionTilePrefab, tileWorldPosition, grid.GetGridObject(i, j));
            }
        }
    }
}
