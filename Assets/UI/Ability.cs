using UnityEngine;

/// <summary>
/// Representa uma habilidade que pode ser desbloqueada e usada pelo jogador.
/// </summary>
[System.Serializable]
public class Ability
{
    public string id;
    public string name;
    public string description;
    public Sprite icon;
    public bool isUnlocked;
    public int cost;
    public GameObject abilityPrefab;

    public Ability(string id, string name, string description, Sprite icon, int cost, GameObject abilityPrefab)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.icon = icon;
        this.isUnlocked = false;
        this.cost = cost;
        this.abilityPrefab = abilityPrefab;
    }
} 