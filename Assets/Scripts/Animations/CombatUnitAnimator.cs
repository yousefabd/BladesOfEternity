using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using UnityEngine;

public class CombatUnitAnimator : MonoBehaviour
{
    [SerializeField] private CombatUnit combatUnit;
    private AnimationManager animationManager;
    private Character4D character4D;

    void Awake()
    {
        animationManager = GetComponent<AnimationManager>();
        character4D = GetComponent<Character4D>();
        character4D.SetDirection(GetRandom4D());
    }

    void OnEnable()
    {
        combatUnit.OnMovementStart += CombatUnit_OnMovementStart;
        combatUnit.OnMovementUpdate += CombatUnit_OnMovementUpdate;
        combatUnit.OnMovementEnd += CombatUnit_OnMovementEnd;
        combatUnit.OnAttack += CombatUnit_OnAttack;
        combatUnit.OnGetDamaged += CombatUnit_OnGetDamaged;
    }

    void OnDisable()
    {
        combatUnit.OnMovementStart -= CombatUnit_OnMovementStart;
        combatUnit.OnMovementUpdate -= CombatUnit_OnMovementUpdate;
        combatUnit.OnMovementEnd -= CombatUnit_OnMovementEnd;
        combatUnit.OnAttack -= CombatUnit_OnAttack;
        combatUnit.OnGetDamaged -= CombatUnit_OnGetDamaged;
    }

    private void CombatUnit_OnGetDamaged(float obj)
    {
        animationManager.Hit();
    }

    private void CombatUnit_OnAttack(CombatUnit attacker)
    {
        Vector3 attackPosition = attacker.transform.position;
        Vector2 attackDir = CastTo4D(attackPosition - transform.position);

        animationManager.Attack();
        character4D.SetDirection(attackDir);
    }

    private void CombatUnit_OnMovementStart(Vector2 direction)
    {
        animationManager.SetState(Assets.HeroEditor4D.Common.Scripts.Enums.CharacterState.Walk);
        character4D.SetDirection(CastTo4D(direction));
    }

    private void CombatUnit_OnMovementUpdate(Vector2 direction)
    {
        character4D.SetDirection(CastTo4D(direction));
    }

    private void CombatUnit_OnMovementEnd()
    {
        animationManager.SetState(Assets.HeroEditor4D.Common.Scripts.Enums.CharacterState.Idle);
    }

    private Vector2Int CastTo4D(Vector3 moveDir)
    {
        if (moveDir == Vector3.zero)
        {
            return Vector2Int.zero; 
        }

        return Mathf.Abs(moveDir.x) >= Mathf.Abs(moveDir.y)
            ? new Vector2Int((int)(moveDir.x / Mathf.Abs(moveDir.x)), 0)
            : new Vector2Int(0, (int)(moveDir.y / Mathf.Abs(moveDir.y)));
    }
    private Vector2Int GetRandom4D()
    {
        return new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }[Random.Range(0, 4)];
    }
}
