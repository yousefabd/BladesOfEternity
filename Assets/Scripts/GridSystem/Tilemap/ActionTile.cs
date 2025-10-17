using System;
using UnityEngine;
public class ActionTile
{
    [Serializable]
    public enum TileType
    {
        None,
        Walk,
        Attack
    }
    public int x;
    public int y;
    private TileType type;
    private Grid<ActionTile> grid;

    public event Action<TileType> OnChangeTileType;
    public event Action<bool> OnToggleSelect;
    public ActionTile(int x, int y,Grid<ActionTile> grid)
    {
        this.x = x;
        this.y = y;
        this.grid = grid;
        type = TileType.None;
    }
    public void SetTileType(TileType type)
    {
        this.type = type;
        OnChangeTileType?.Invoke(type);
    }
    public TileType GetTileType()
    {
        return type;
    }
    public void ToggleSelect(bool isSelected)
    {
        OnToggleSelect?.Invoke(isSelected);
    }
    public override string ToString()
    {
        return $"({x},{y})";
    }

}
