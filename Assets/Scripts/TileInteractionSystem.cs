using CodeMonkey.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TileInteractionSystem : MonoBehaviour
{
    [SerializeField] private ActionTileVisual actionTileVisualPrefab;
    [SerializeField] private Vector2 interactionBoxSize;

    private Grid<ActionTile> grid;

    private ActionTile lastSelectedTile;

    private List<ActionTile> currentActionTiles = new();

    public event Action<CombatUnit> OnClickUnitMove;
    public event Action<CombatUnit> OnClickUnitAttack;
    public event Action<Vector3> OnMoveUnit;
    public event Action OnClickEmptyTile;
    private void Update()
    {
        HandleTileHover();
        HandleMouseClick();
    }
    private void HandleTileHover()
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        ActionTile selectedActionTile = grid.GetGridObject(mousePosition);
        lastSelectedTile?.ToggleSelect(false);
        selectedActionTile?.ToggleSelect(true);
        lastSelectedTile = selectedActionTile;
    }
    private void HandleMouseClick()
    {
        if(lastSelectedTile is null || (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)))
        {
            return;
        }
        Vector3 selectedTileWorldPosition = grid.GetCellWorldPosition(lastSelectedTile.x, lastSelectedTile.y);
        switch (lastSelectedTile.GetTileType())
        {
            case ActionTile.TileType.None:
                ClearActionTilesList();
                if (TryGetUnit(selectedTileWorldPosition, out var unit)) 
                {
                    HandleUnitActions(unit);
                }
                else
                {
                    OnClickEmptyTile?.Invoke();
                }
                break;
            case ActionTile.TileType.Walk:
                OnMoveUnit?.Invoke(selectedTileWorldPosition);
                ClearActionTilesList();
                break;
        }
    }
    private void HandleUnitActions(CombatUnit combatUnit)
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClickUnitMove?.Invoke(combatUnit);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            OnClickUnitAttack?.Invoke(combatUnit); 
        }
    }
    private bool TryGetUnit(Vector3 worldPosition, out CombatUnit unit)
    {
        unit = Physics2D.OverlapBox(worldPosition, interactionBoxSize,0f)?.GetComponent<CombatUnit>();
        return unit != null;
    }
    private void InitActionTileGrid(int width, int height, float cellSize, Vector3 originPosition)
    {
        grid = new(
            width,
            height,
            cellSize,
            originPosition,
            (x, y) => new ActionTile(x, y, grid)
        );
        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetHeight(); j++)
            {
                Vector3 tileWorldPosition = grid.GetCellWorldPosition(i, j);
                ActionTileVisual actionTileVisual = ActionTileVisual.Init(actionTileVisualPrefab, tileWorldPosition, grid.GetGridObject(i, j));
                actionTileVisual.transform.SetParent(transform);
                actionTileVisual.name = $"ActionTile ${grid.GetGridObject(i, j)}";
            }
        }
    }
    public void ClearActionTilesList()
    {
        foreach(var actionTile in currentActionTiles)
        {
            actionTile.SetTileType(ActionTile.TileType.None);
        }
        currentActionTiles.Clear();
    }
    public void SetActionTilesList(List<Vector2Int> actionTilesIndicesList,ActionTile.TileType tileType)
    {
        foreach(var actionTileIndices in actionTilesIndicesList)
        {
            ActionTile actionTile = grid.GetGridObject(actionTileIndices.x, actionTileIndices.y);
            if(actionTile is null)
                continue;
            actionTile.SetTileType(tileType);
            currentActionTiles.Add(actionTile);
        }
    }
    public static TileInteractionSystem Init(TileInteractionSystem prefab, Transform parent,int width,int height, float cellSize, Vector3 originPosition)
    {
        TileInteractionSystem tileInteractionSystem = Instantiate(prefab, parent);
        tileInteractionSystem.InitActionTileGrid(width, height, cellSize, originPosition);

        return tileInteractionSystem;
    }
}
