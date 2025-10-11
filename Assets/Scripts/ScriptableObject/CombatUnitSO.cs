using UnityEngine;

[CreateAssetMenu(fileName = "CombatUnit", menuName = "ScriptableObjects/CombatUnitSO")]

public class CombatUnitSO : ScriptableObject
{
    public string unitName;
    public Transform prefab;
    public Sprite spriteIcon;
    public TeamSO team;
    public float moveSpeed;
}
