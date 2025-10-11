using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    [SerializeField] private CombatUnitSO combatUnitSO;
    public enum State
    {
        Idle,
        Moving
    }

    public event Action<Vector2> OnMovementStart;
    public event Action<Vector2> OnMovementUpdate;
    public event Action OnMovementEnd;

    private TeamSO team;
    private State currentState = State.Idle;
    private Vector3 currentTargetPosition;
    private List<Vector3> currentPath;
    private int currentPathIndex;

    private void Awake()
    {
        team = combatUnitSO.team;
    }

    public void SetTeam(TeamSO team)
    {
        this.team = team;
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
    public void MovePath(List<Vector3> path)
    {
        if (path.Count == 0) return;

        this.currentPath = path;
        currentPathIndex = 0;
        currentState = State.Moving;
        OnMovementStart?.Invoke(currentPath[currentPathIndex] - transform.position);
    }
}
