using UnityEngine;

[CreateAssetMenu(fileName = "CombatUnit", menuName = "ScriptableObjects/CombatUnitSO")]

public class CombatUnitSO : ScriptableObject
{
    public string unitName;
    public Transform prefab;
    public Sprite spriteIcon;
    public TeamSO team;
    public float moveSpeed;
    public int moveTiles;
    public int attackTiles;
    public float attackCooldown;
    public int maxHealth = 100;
}
