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
    }

    void OnDisable()
    {
        combatUnit.OnMovementStart -= CombatUnit_OnMovementStart;
        combatUnit.OnMovementUpdate -= CombatUnit_OnMovementUpdate;
        combatUnit.OnMovementEnd -= CombatUnit_OnMovementEnd;
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

    private Vector2Int CastTo4D(Vector2 moveDir)
    {
        if (moveDir == Vector2.zero)
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
