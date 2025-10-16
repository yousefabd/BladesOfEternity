using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct TeamCombatUnits
{
    public TeamSO team;
    public List<CombatUnitSO> units;
}
