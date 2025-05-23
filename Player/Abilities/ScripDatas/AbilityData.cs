using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Ability/New")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    [TextArea]
    public string description;

    public bool unlocked;
    public bool autoEquip;
    public bool equipped;

    public int level;
    public int maxLevel;

    public int cost;
    public float cooldown;

    [Tooltip("Script da habilidade que será adicionado ao player")]
    public MonoScript abilityScript;

    public AbilityType abilityType;

}
public enum AbilityType
{
    Passive,
    Active,
    Ultimate
}
