using CodeMonkey.Utils;
using System;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Grid<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private bool showDebug = false;

    private TGridObject[,] gridArray;
    private TextMeshPro[,] worldTextGridArray;

    public event Action<Vector2Int> OnGridObjectChanged;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<int,int,TGridObject> CreateGridObject, bool showDebug = false) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.showDebug = showDebug;
        gridArray = new TGridObject[width, height];
        for(int i=0; i < width; i++) {
            for(int j=0; j < height; j++) {
                gridArray[i,j] = CreateGridObject(i,j);
            }
        }

        if (showDebug) {
            worldTextGridArray = new TextMeshPro[width, height];
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    worldTextGridArray[i, j] = UtilsClass.CreateWorldText(
                        text: gridArray[i, j]?.ToString(),
                        parent: null,
                        localPosition: GetWorldPosition(i, j) + new Vector3(cellSize, cellSize) * 0.5f,
                        fontSize: 4,
                        color: Color.white,
                        textAnchor: TextAnchor.MiddleCenter
                    );
                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
            OnGridObjectChanged += (indices) => {
                worldTextGridArray[indices.x, indices.y].text = gridArray[indices.x, indices.y]?.ToString();
            };
        }
    }
    public Vector3 GetWorldPosition(int i,int j) {
        return new Vector3(i, j) * cellSize + originPosition;
    }
    public Vector3 GetCellWorldPosition(int i,int j) {
        return GetWorldPosition(i, j) + new Vector3(cellSize, cellSize) * 0.5f;
    }
    public Vector2Int GetIJ(Vector3 worldPosition) {
        Vector2Int ij = new Vector2Int();
        ij.x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        ij.y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        return ij;
    }
    public void SetGridObject(int i,int j,TGridObject value) {
        if(i < 0 || j < 0 || i >= width || j >= height) {
            return;
        }
        gridArray[i, j] = value;
        OnGridObjectChanged?.Invoke(new Vector2Int(i, j));

    }
    public void SetGridObject(Vector3 worldPosition,TGridObject value) {
        Vector2Int ij = GetIJ(worldPosition);
        SetGridObject(ij.x, ij.y, value);
    }
    public void TriggerGridObjectChanged(int i,int j) {
        OnGridObjectChanged?.Invoke(new Vector2Int(i, j));
    }
    public TGridObject GetGridObject(int i,int j) {
        if (i < 0 || j < 0 || i >= width || j >= height) {
            return default;
        }
        return gridArray[i, j];
    }
    public TGridObject GetGridObject(Vector3 worldPosition) {
        Vector2Int ij = GetIJ(worldPosition);
        return GetGridObject(ij.x, ij.y);
    }

    public int GetWidth() {
        return width;
    }
    public int GetHeight() {
        return height;
    }
    public float GetCellSize() {
        return cellSize;
    }
}
