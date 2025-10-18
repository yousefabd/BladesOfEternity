using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnBasedSystem
{
    private List<TeamSO> teams;
    private Dictionary<TeamSO, List<CombatUnit>> teamCombatUnits;

    //turn variables
    private List<CombatUnit> availableUnits;
    private Dictionary<CombatUnit, List<MoveAction>> unitMoveActions;

    private int currentTeamIndex = 0;
    private int currentSelectedUnitIndex = 0;
    private bool IsUnitMoveActionInProgress = false;

    public event Action<CombatUnit, Vector3> OnMoveUnit;
    public event Action<CombatUnit, MoveAction> OnAutoSelectUnit;

    public TurnBasedSystem(List<TeamSO> teams,List<CombatUnit> combatUnits)
    {
        teamCombatUnits = new();
        availableUnits = new();
        unitMoveActions = new();
        this.teams = teams;
        foreach(TeamSO team in teams)
        {
            List<CombatUnit> teamUnits = combatUnits.Where(unit => unit.GetTeam().Equals(team)).ToList();
            teamCombatUnits[team] = teamUnits;
        }
        currentTeamIndex = -1;
        SwitchTurn();
    }

    public bool SelectUnit(CombatUnit unit,MoveAction moveAction)
    {
        if (!unit.GetTeam().Equals(GetCurrentTeam()))
        {
            return false;
        }
        else if (!CanPerformAction(unit, moveAction) || IsUnitMoveActionInProgress)
        {
            return false;
        }
        for (int i = 0; i < availableUnits.Count(); i++)
        {
            if (availableUnits[i].Equals(unit))
            {
                currentSelectedUnitIndex = i;
            }
        }
        return true;
    }
    public void DeselectUnit()
    {
        currentSelectedUnitIndex = -1;
    }
    private bool HasMoveActions(CombatUnit unit)
    {
        return unitMoveActions[unit].Any();
    }
    private bool CanPerformAction(CombatUnit unit, MoveAction moveAction)
    {
        return unitMoveActions[unit].Contains(moveAction);
    }
    private MoveAction GetAnyMoveAction(CombatUnit unit)
    {
        if (unitMoveActions.Any())
        {
            return unitMoveActions[unit].First();
        }
        return default; 
    }
    public void RemoveUnitMoveAction(CombatUnit unit, MoveAction moveAction)
    {
        unitMoveActions[unit].Remove(moveAction);
        if (!HasMoveActions(unit))
        {
            availableUnits.Remove(unit);
            if (availableUnits.Count == 0)
            {
                SwitchTurn();
                return;
            }
            currentSelectedUnitIndex = 0;
            CombatUnit nextUnit = availableUnits[currentSelectedUnitIndex];
            OnAutoSelectUnit?.Invoke(nextUnit, GetAnyMoveAction(nextUnit));
            return;
        }
        OnAutoSelectUnit?.Invoke(unit, GetAnyMoveAction(unit));

    }
    public void ClearCurrentUnitMoveActions()
    {
        if(currentSelectedUnitIndex == -1)
        {
            currentSelectedUnitIndex = 0;
        }
        CombatUnit unit = availableUnits[currentSelectedUnitIndex];
        unitMoveActions[unit].Clear();
        availableUnits.Remove(unit);
        if (availableUnits.Count == 0)
        {
            SwitchTurn();
            return;
        }
        currentSelectedUnitIndex = 0;
        CombatUnit nextUnit = availableUnits[currentSelectedUnitIndex];
        OnAutoSelectUnit?.Invoke(nextUnit,GetAnyMoveAction(nextUnit));
    }
    public void MoveUnit(Vector3 destination)
    {
        if(currentSelectedUnitIndex == -1 || IsUnitMoveActionInProgress)
        {
            return;
        }
        CombatUnit unit = availableUnits[currentSelectedUnitIndex];
        OnMoveUnit?.Invoke(unit, destination);
        ApplyMoveAction(unit, MoveAction.Move);
    }
    public void AttackUnit(CombatUnit targetUnit)
    {
        if(currentSelectedUnitIndex == -1 || IsUnitMoveActionInProgress)
        {
            return;
        }
        CombatUnit unit = availableUnits[currentSelectedUnitIndex];
        unit.Attack(targetUnit);
        ApplyMoveAction(unit, MoveAction.Attack);

    }

    public TeamSO GetCurrentTeam()
    {
        return teams[currentTeamIndex];
    }
    private void ApplyMoveAction(CombatUnit unit, MoveAction moveAction)
    {
        if (!CanPerformAction(unit, moveAction))
        {
            return;
        }

        IsUnitMoveActionInProgress = true;
        void OnUnitActionEnd()
        {
            OnUnitCompletedMoveAction(unit, moveAction);
            unit.OnMovementEnd -= OnUnitActionEnd;
            unit.OnAttackEnd -= OnUnitActionEnd;
        }

        unit.OnMovementEnd += OnUnitActionEnd;
        unit.OnAttackEnd += OnUnitActionEnd;
    }
    private void OnUnitCompletedMoveAction(CombatUnit unit,MoveAction moveAction)
    {
        IsUnitMoveActionInProgress = false;
        RemoveUnitMoveAction(unit, moveAction);
    }
    private void SwitchTurn()
    {
        availableUnits.Clear();
        currentTeamIndex = (currentTeamIndex + 1) % teams.Count;
        foreach(var combatUnit in teamCombatUnits[teams[currentTeamIndex]])
        {
            availableUnits.Add(combatUnit);
            unitMoveActions[combatUnit] = new List<MoveAction> { MoveAction.Move, MoveAction.Attack}; 
        }
        if (availableUnits.Any())
        {
            currentSelectedUnitIndex = 0;
            CombatUnit nextUnit = availableUnits[currentSelectedUnitIndex];
            OnAutoSelectUnit?.Invoke(nextUnit,GetAnyMoveAction(nextUnit));
        }
    }
}
