using UnityEngine;
[RequireComponent(typeof(Projectile))]
public class ArrowProjectile : MonoBehaviour
{
    private Projectile projectile;
    private void Start()
    {
        projectile = GetComponent<Projectile>();
        projectile.OnReachTargetUnit += Projectile_OnReachTargetUnit;
    }

    private void Projectile_OnReachTargetUnit(CombatUnit targetUnit)
    {
        float damageAmount = projectile.GetProjectileSO().damage;
        targetUnit.Damage(damageAmount);
        Destroy(gameObject);
    }
}
