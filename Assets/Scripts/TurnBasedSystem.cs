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

    public event Action<CombatUnit> OnSelectUnit;
    public event Action<CombatUnit,Vector3> OnMoveUnit;

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
        //InteractionSystem.Instance.OnClickUnit += InteractionSystem_OnClickUnit;
        //InteractionSystem.Instance.OnMoveUnit += InteractionSystem_OnMoveUnit;
        //InteractionSystem.Instance.OnRightClickUnit += InteractionSystem_OnRightClickUnit;
    }

    private void InteractionSystem_OnRightClickUnit(CombatUnit unit)
    {
        if (unit.GetTeam().Equals(teams[currentTeamIndex]))
        {
            return;
        }
        AttackUnit(unit);
    }

    private void InteractionSystem_OnMoveUnit(Vector3 movePosition)
    {
        MoveUnit(movePosition);
    }

    private void InteractionSystem_OnClickUnit(CombatUnit unit)
    {
        if (unit.GetTeam().Equals(teams[currentTeamIndex]))
        {
            SelectUnit(unit);
        }
        else
        {
            Debug.Log("Selected unit is of team: " + unit.GetTeam().teamName + " but current team is: " + teams[currentTeamIndex].teamName);
        }
    }
    public bool SelectUnit(CombatUnit unit)
    {
        if (!unit.GetTeam().Equals(teams[currentTeamIndex]))
        {
            return false;
        }
        for (int i = 0; i < availableUnits.Count(); i++)
        {
            if (availableUnits[i].Equals(unit))
            {
                currentSelectedUnitIndex = i;
                OnSelectUnit?.Invoke(unit);
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
    private bool CanPerformAction(CombatUnit unit,MoveAction moveAction)
    {
        return unitMoveActions[unit].Contains(moveAction);
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
        Debug.Log("Selected unit is " + currentSelectedUnitIndex);
        CombatUnit unit = availableUnits[currentSelectedUnitIndex];
        unit.Attack(targetUnit);
        ApplyMoveAction(unit, MoveAction.Attack);

    }
    private void ApplyMoveAction(CombatUnit unit, MoveAction moveAction)
    {
        if (!CanPerformAction(unit, moveAction))
        {
            return;
        }

        unitMoveActions[unit].Remove(moveAction);
        IsUnitMoveActionInProgress = true;
        void OnUnitActionEnd()
        {
            OnUnitCompletedMoveAction(unit);
            unit.OnMovementEnd -= OnUnitActionEnd;
            unit.OnAttackEnd -= OnUnitActionEnd;
        }

        unit.OnMovementEnd += OnUnitActionEnd;
        unit.OnAttackEnd += OnUnitActionEnd;
    }
    private void OnUnitCompletedMoveAction(CombatUnit unit)
    {
        IsUnitMoveActionInProgress = false;
        if (!HasMoveActions(unit))
        {
            availableUnits.Remove(unit);
            currentSelectedUnitIndex = 0;
        }

        if (availableUnits.Count == 0)
        {
            SwitchTurn();
        }
    }
    private void SwitchTurn()
    {
        availableUnits.Clear();
        currentTeamIndex = (currentTeamIndex + 1) % teams.Count;
        DeselectUnit();
        foreach(var combatUnit in teamCombatUnits[teams[currentTeamIndex]])
        {
            availableUnits.Add(combatUnit);
            unitMoveActions[combatUnit] = new List<MoveAction> { MoveAction.Move, MoveAction.Attack}; 
        }
        
    }
}
