using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDashAbility", menuName = "Abilities/DashAbility")]
public class DashData : AbilityData
{
    public float dashSpeed;
    public float dashDuration;
}