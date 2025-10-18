using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    [SerializeField] private CombatUnitSO combatUnitSO;

    private HealthSystem HealthSystem { get; set; }
    [SerializeField] private HealthBar HealthBar { get; set; }

    public enum State
    {
        Idle,
        Moving,
        Attacking
    }
    public event Action<Vector2> OnMovementStart;
    public event Action<Vector2> OnMovementUpdate;
    public event Action OnMovementEnd;
    public event Action<float> OnGetDamaged;
    public event Action<CombatUnit> OnAttack;
    public event Action OnAttackEnd;

    private TeamSO team;
    private State currentState = State.Idle;

    //move state
    private Vector3 currentTargetPosition;
    private List<Vector3> currentPath;
    private int currentPathIndex;

    //attack state
    private float attackTime;

    private void Awake()
    {
        team = combatUnitSO.team;
    }

    private void Start() {
        this.HealthSystem = new HealthSystem();
        this.HealthBar = new HealthBar();
        this.HealthBar.Setup(this.HealthSystem);

        OnGetDamaged += HandleDamage;

        Debug.Log("health " + this.HealthSystem.GetHealth());
    }

    private void HandleDamage(float damageAmount) {
        HealthSystem.Damage((int)damageAmount);
        this.HealthBar.Setup(this.HealthSystem);

        Debug.Log($"Unit took {damageAmount} damage. Current health: {HealthSystem.GetHealth()}");
    }

    public TeamSO GetTeam()
    {
        return team;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                break;

            case State.Moving:
                HandleMovement();
                break;
            case State.Attacking:
                HandleAttacking();
                break;
        }
    }

    private void HandleMovement()
    {
        if (currentPathIndex < currentPath.Count)
        {
            currentTargetPosition = currentPath[currentPathIndex];
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, Time.deltaTime * 5f); // Adjust speed as needed

            if (Vector3.Distance(transform.position, currentTargetPosition) < 0.1f)
            {
                currentPathIndex++;
                if (currentPathIndex >= currentPath.Count)
                {
                    currentState = State.Idle;
                    OnMovementEnd?.Invoke();
                    return;
                }
                OnMovementUpdate?.Invoke(currentPath[currentPathIndex] - transform.position);
            }
        }
    }

    private void HandleAttacking()
    {
        attackTime -= Time.deltaTime;
        if(attackTime < 0f)
        {
            currentState = State.Idle;
            OnAttackEnd?.Invoke();
        }
    }

    public void MovePath(List<Vector3> path)
    {
        if (path.Count == 0) return;

        this.currentPath = path;
        currentPathIndex = 0;
        currentState = State.Moving;
        OnMovementStart?.Invoke(currentPath[currentPathIndex] - transform.position);
    }

    public CombatUnitSO GetCombatUnitSO()
    {
        return combatUnitSO;
    }

    public void Damage(float amount)
    {
        OnGetDamaged?.Invoke(amount);
    }

    public void Attack(CombatUnit targetUnit)
    {
        attackTime = combatUnitSO.attackCooldown;
        currentState = State.Attacking;
        OnAttack?.Invoke(targetUnit);
    }

}
