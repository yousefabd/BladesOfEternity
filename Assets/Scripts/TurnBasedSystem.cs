using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnBasedSystem
{
    private List<TeamSO> teams;

    private Dictionary<TeamSO, List<CombatUnit>> teamCombatUnits;

    private TeamSO currentTeam;
    private int currentUnitIndex = 0;

    public event Action<CombatUnit> OnSelectUnit;

    public TurnBasedSystem(List<TeamSO> teams,List<CombatUnit> combatUnits)
    {
        this.teams = teams;
        foreach(TeamSO team in teams)
        {
            List<CombatUnit> teamUnits = combatUnits.Where(unit => unit.GetTeam().Equals(team)).ToList();
            teamCombatUnits[team] = teamUnits;
        }
        InteractionSystem.Instance.OnClickUnit += InteractionSystem_OnClickUnit;
    }

    private void InteractionSystem_OnClickUnit(CombatUnit unit)
    {
        if (unit.GetTeam().Equals(currentTeam))
        {
            SelectUnit(unit);
        }
    }
    private void SelectUnit(CombatUnit combatUnit)
    {
        List<CombatUnit> combatUnits = teamCombatUnits[currentTeam];
        for (int i = 0; i < combatUnits.Count(); i++)
        {
            if (combatUnits[i].Equals(combatUnit))
            {
                currentUnitIndex = i;
                OnSelectUnit?.Invoke(combatUnit);
            }
        }
    }
    private void MoveUnit(Vector3 destination)
    {
        if(currentUnitIndex == -1)
        {
            //no unit is selected.. 
            return;
        }
        CombatUnit unit = teamCombatUnits[currentTeam][currentUnitIndex];
        List<Vector3> movePath = GameHandler_TurnBasedSystem.Instance.GetMovePath(unit.transform.position, destination);
        unit.MovePath(movePath);
    }
}
