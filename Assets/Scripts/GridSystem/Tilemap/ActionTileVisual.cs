using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
public class ActionTileVisual : MonoBehaviour
{
    [SerializeField] private Transform selectedTile;
    [SerializeField] private List<TileTypeGameObject> tileTypeGameObjectList;

    private ActionTile actionTile;

    private Dictionary<ActionTile.TileType, GameObject> tileGameObjectDictionary;
    private void Awake()
    {
        tileGameObjectDictionary = new();
        foreach(var tyleTypeSprite in tileTypeGameObjectList)
        {
            tileGameObjectDictionary[tyleTypeSprite.tileType] = tyleTypeSprite.tileGameObject;
        }
        ToggleSelected(false);
        SetActiveTileGameObject(ActionTile.TileType.None);
    }
    private void Start()
    {
        actionTile.OnChangeTileType += ActionTile_OnChangeTileType;
        actionTile.OnToggleSelect += ToggleSelected;

    }

    private void ActionTile_OnChangeTileType(ActionTile.TileType type)
    {
        SetActiveTileGameObject(type);
    }
    private void SetActiveTileGameObject(ActionTile.TileType tileType)
    {
        foreach(var tileTypeGO in tileTypeGameObjectList)
        {
            tileTypeGO.tileGameObject.SetActive(false);
        }
        if (!tileGameObjectDictionary.ContainsKey(tileType))
        {
            return;
        }
        tileGameObjectDictionary[tileType].SetActive(true);
    }
    private void ToggleSelected(bool isSlected)
    {
        selectedTile.gameObject.SetActive(isSlected);
    }
    public static ActionTileVisual Init(ActionTileVisual prefab,Vector3 position, ActionTile interactionTile)
    {
        ActionTileVisual interactionTileVisual = Instantiate(prefab, position, Quaternion.identity);
        interactionTileVisual.actionTile = interactionTile;
        return interactionTileVisual;
    }
}
