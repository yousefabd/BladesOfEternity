using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    private TeamSO team;

    public void SetTeam(TeamSO team)
    {
        this.team = team;
    }
    public TeamSO GetTeam()
    {
        return team;
    }
    public void MovePath(List<Vector3> path)
    {

    }
}
