using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Team",menuName ="ScriptableObjects/TeamSO")]
public class TeamSO : ScriptableObject
{
    public string teamName;
    public Color teamColor;
}
