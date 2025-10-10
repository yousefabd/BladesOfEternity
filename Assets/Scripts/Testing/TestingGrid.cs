using CodeMonkey.Utils;
using UnityEngine;

public class TestingGrid : MonoBehaviour
{
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;
    [SerializeField] private float cellSize = 1f;

    private Grid<string> grid;

    private void Start() {
        grid = new Grid<string>(width, height,cellSize,transform.position,(i,j)=> $"({i}, {j})",true);
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            grid.SetGridObject(mousePosition, "Joe");
        }
        if (Input.GetMouseButtonDown(1)) {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            Debug.Log(grid.GetGridObject(mousePosition));
        }
    }
}
