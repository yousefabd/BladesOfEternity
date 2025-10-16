using CodeMonkey.Utils;
using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private ProjectileSO projectileSO;
    private CombatUnit targetUnit;

    public event Action<CombatUnit> OnReachTargetUnit;
    public event Action<CombatUnit> OnHitUnit;

    public static Projectile Shoot(ProjectileSO projectileSO,Vector3 position,CombatUnit targetUnit)
    {
        Projectile projectile = Instantiate(projectileSO.prefab, position, Quaternion.identity).GetComponent<Projectile>();
        projectile.targetUnit = targetUnit;
        projectile.projectileSO = projectileSO;
        projectile.RotateToTarget();
        return projectile;
    }
    private void RotateToTarget()
    {
        Vector3 targetDir = (targetUnit.transform.position - transform.position).normalized;
        float rotationAngle = UtilsClass.GetAngleFromVector180(targetDir);
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
    }
    private void Start()
    {
        Destroy(gameObject,1f);
    }
    private void Update()
    {
        MoveToTarget();
    }
    private void MoveToTarget()
    {
        Vector3 moveDir = (targetUnit.transform.position - transform.position).normalized;
        float moveSpeed = projectileSO.speed;
        transform.position += moveSpeed * Time.deltaTime * moveDir;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out CombatUnit combatUnit))
            return;
        if (combatUnit.Equals(targetUnit))
            OnReachTargetUnit?.Invoke(combatUnit);
        else if(combatUnit.GetTeam().Equals(targetUnit.GetTeam()))
            OnHitUnit?.Invoke(combatUnit);
    }
    public ProjectileSO GetProjectileSO()
    {
        return projectileSO;
    }
}
