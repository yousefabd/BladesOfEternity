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
    private int currentTeamIndex = 0;
    private int currentSelectedUnitIndex = 0;

    public event Action<CombatUnit> OnSelectUnit;

    public TurnBasedSystem(List<TeamSO> teams,List<CombatUnit> combatUnits)
    {
        teamCombatUnits = new Dictionary<TeamSO, List<CombatUnit>>();
        availableUnits = new List<CombatUnit>();
        this.teams = teams;
        foreach(TeamSO team in teams)
        {
            List<CombatUnit> teamUnits = combatUnits.Where(unit => unit.GetTeam().Equals(team)).ToList();
            teamCombatUnits[team] = teamUnits;
        }
        currentSelectedUnitIndex = -1;
        currentTeamIndex = -1;
        SwitchTurn();
        InteractionSystem.Instance.OnClickUnit += InteractionSystem_OnClickUnit;
        InteractionSystem.Instance.OnMoveUnit += InteractionSystem_OnMoveUnit;
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
    private void SelectUnit(CombatUnit combatUnit)
    {
        Debug.Log("Selecting unit at" + combatUnit.transform.position);
        for (int i = 0; i < availableUnits.Count(); i++)
        {
            if (availableUnits[i].Equals(combatUnit))
            {
                currentSelectedUnitIndex = i;
                OnSelectUnit?.Invoke(combatUnit);
            }
        }
    }
    private void MoveUnit(Vector3 destination)
    {
        if(currentSelectedUnitIndex == -1)
        {
            //no unit is selected.. 
            return;
        }
        CombatUnit unit = availableUnits[currentSelectedUnitIndex];
        List<Vector3> movePath = GameHandler_TurnBasedSystem.Instance.GetMovePath(unit.transform.position, destination);
        unit.MovePath(movePath);
        //when unit is moved it's no longer available(for now)
        availableUnits.RemoveAt(currentSelectedUnitIndex);
        if(availableUnits.Count == 0)
        {
            SwitchTurn();
        }
    }
    private void SwitchTurn()
    {
        availableUnits.Clear();
        currentTeamIndex = (currentTeamIndex + 1) % teams.Count;
        currentSelectedUnitIndex = -1;
        foreach(var combatUnit in teamCombatUnits[teams[currentTeamIndex]])
        {
            availableUnits.Add(combatUnit);
        }
    }
}
