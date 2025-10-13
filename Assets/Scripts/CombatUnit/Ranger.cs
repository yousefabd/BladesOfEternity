using UnityEngine;
[RequireComponent(typeof(CombatUnit))]
public class Ranger : MonoBehaviour
{
    [SerializeField] private Transform projectileSpawn;
    private CombatUnit combatUnit;

    private void Start()
    {
        combatUnit = GetComponent<CombatUnit>();
        combatUnit.OnAttack += CombatUnit_OnAttack;
    }

    private void CombatUnit_OnAttack(CombatUnit targetUnit)
    {
        ProjectileSO projectileType = (combatUnit.GetCombatUnitSO() as RangerSO).projectileType;
        Projectile.Shoot(projectileType, projectileSpawn.position, targetUnit);
    }
}
