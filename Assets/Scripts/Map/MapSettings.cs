using UnityEngine;

public class MapSettings : MonoBehaviour
{
    [SerializeField] public int Width;
    [SerializeField] public int Height;
    [SerializeField] public int cellSize = 1;
    [SerializeField] public Transform Origin;
}
