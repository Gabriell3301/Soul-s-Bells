using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Ability/New")]
public class AbilityData : ScriptableObject
{
    [Header("Basic Info")]
    public string abilityName;
    [TextArea(3, 5)]
    public string description;
    
    [Header("Visual")]
    public Sprite abilityIcon;
    public Color abilityColor = Color.white;
    public string category = "Geral"; // Para organização na UI
    
    [Header("States")]
    public bool unlocked = false;
    public bool autoEquip = false;
    public bool equipped = false;
    
    [Header("Level System")]
    public int level = 1;
    public int maxLevel = 5;
    [TextArea(2, 4)]
    public string levelUpDescription; // Descrição do que melhora ao subir de level
    
    [Header("Requirements")]
    public int cost = 100; // Custo para desbloquear
    public int upgradeCost = 50; // Custo para fazer upgrade
    public List<AbilityData> requiredAbilities; // Habilidades necessárias para desbloquear
    public int requiredPlayerLevel = 1; // Level mínimo do player
    
    [Header("Stats")]
    public float cooldown = 0f;
    public float duration = 0f; // Duração do efeito (se aplicável)
    public int manaCost = 0; // Custo de mana/energia
    public int damage = 0; // Dano base (se aplicável)
    
    [Header("Script")]
    [Tooltip("Script da habilidade que será adicionado ao player")]
    public MonoScript abilityScript;
    
    [Header("Type & Rarity")]
    public AbilityType abilityType;
    public AbilityRarity rarity = AbilityRarity.Common;
    
    [Header("Scaling (Por Level)")]
    public int damagePerLevel = 0;
    public float cooldownReductionPerLevel = 0f;
    public float durationIncreasePerLevel = 0f;
    public int manaCostIncreasePerLevel = 0;
    
    [Header("Audio")]
    public AudioClip unlockSound;
    public AudioClip equipSound;
    public AudioClip activationSound;
    
    // Propriedades calculadas
    public int TotalDamage => damage + (damagePerLevel * (level - 1));
    public float TotalCooldown => Mathf.Max(0.1f, cooldown - (cooldownReductionPerLevel * (level - 1)));
    public float TotalDuration => duration + (durationIncreasePerLevel * (level - 1));
    public int TotalManaCost => manaCost + (manaCostIncreasePerLevel * (level - 1));
    public int TotalUpgradeCost => upgradeCost * level;
    
    // Métodos de utilidade
    public bool CanUnlock(int playerLevel, int playerCurrency, List<AbilityData> playerAbilities)
    {
        // Verifica se já está desbloqueada
        if (unlocked) return false;
        
        // Verifica level do player
        if (playerLevel < requiredPlayerLevel) return false;
        
        // Verifica se tem dinheiro suficiente
        if (playerCurrency < cost) return false;
        
        // Verifica habilidades requeridas
        if (requiredAbilities != null && requiredAbilities.Count > 0)
        {
            foreach (var requiredAbility in requiredAbilities)
            {
                if (!playerAbilities.Contains(requiredAbility) || !requiredAbility.unlocked)
                    return false;
            }
        }
        
        return true;
    }
    
    public bool CanUpgrade(int playerCurrency)
    {
        return unlocked && level < maxLevel && playerCurrency >= TotalUpgradeCost;
    }
    
    public string GetStatusText()
    {
        if (!unlocked) return "Bloqueada";
        if (equipped) return "Equipada";
        return "Desbloqueada";
    }
    
    public Color GetRarityColor()
    {
        switch (rarity)
        {
            case AbilityRarity.Common: return Color.white;
            case AbilityRarity.Uncommon: return Color.green;
            case AbilityRarity.Rare: return Color.blue;
            case AbilityRarity.Epic: return Color.magenta;
            case AbilityRarity.Legendary: return Color.yellow;
            default: return Color.white;
        }
    }
    
    public string GetFormattedDescription()
    {
        string formattedDesc = description;
        
        // Substitui placeholders com valores atuais
        formattedDesc = formattedDesc.Replace("{damage}", TotalDamage.ToString("F1"));
        formattedDesc = formattedDesc.Replace("{cooldown}", TotalCooldown.ToString("F1"));
        formattedDesc = formattedDesc.Replace("{duration}", TotalDuration.ToString("F1"));
        formattedDesc = formattedDesc.Replace("{manaCost}", TotalManaCost.ToString());
        formattedDesc = formattedDesc.Replace("{level}", level.ToString());
        
        return formattedDesc;
    }
    
    // Método para criar uma cópia da habilidade (útil para sistemas de save)
    public AbilityData CreateCopy()
    {
        AbilityData copy = CreateInstance<AbilityData>();
        copy.abilityName = abilityName;
        copy.description = description;
        copy.abilityIcon = abilityIcon;
        copy.abilityColor = abilityColor;
        copy.category = category;
        copy.unlocked = unlocked;
        copy.autoEquip = autoEquip;
        copy.equipped = equipped;
        copy.level = level;
        copy.maxLevel = maxLevel;
        copy.cost = cost;
        copy.upgradeCost = upgradeCost;
        copy.requiredAbilities = new List<AbilityData>(requiredAbilities ?? new List<AbilityData>());
        copy.requiredPlayerLevel = requiredPlayerLevel;
        copy.cooldown = cooldown;
        copy.duration = duration;
        copy.manaCost = manaCost;
        copy.damage = damage;
        copy.abilityScript = abilityScript;
        copy.abilityType = abilityType;
        copy.rarity = rarity;
        copy.damagePerLevel = damagePerLevel;
        copy.cooldownReductionPerLevel = cooldownReductionPerLevel;
        copy.durationIncreasePerLevel = durationIncreasePerLevel;
        copy.manaCostIncreasePerLevel = manaCostIncreasePerLevel;
        copy.unlockSound = unlockSound;
        copy.equipSound = equipSound;
        copy.activationSound = activationSound;
        
        return copy;
    }
    
#if UNITY_EDITOR
    [ContextMenu("Test Unlock Requirements")]
    private void TestUnlockRequirements()
    {
        Debug.Log($"Ability: {abilityName}");
        Debug.Log($"Required Level: {requiredPlayerLevel}");
        Debug.Log($"Cost: {cost}");
        Debug.Log($"Required Abilities: {(requiredAbilities?.Count ?? 0)}");
    }
#endif
}

public enum AbilityType
{
    Passive,
    Active,
    Ultimate,
    Buff,
    Debuff,
    Healing,
    Movement,
    Attack
}

public enum AbilityRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}