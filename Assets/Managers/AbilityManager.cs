using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia as habilidades do jogador e seu desbloqueio.
/// </summary>
public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; } // Instância singleton

    [Header("Configurações")]
    [SerializeField] private List<Ability> abilities; // Lista de habilidades
    [SerializeField] private Transform abilityContainer; // Container das habilidades
    [SerializeField] private GameObject abilityButtonPrefab; // Prefab do botão de habilidade

    private Dictionary<string, Ability> abilityDictionary; // Dicionário de habilidades
    private Dictionary<string, AbilityButton> abilityButtons; // Dicionário de botões de habilidade

    /// <summary>
    /// Inicializa o singleton
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        abilityDictionary = new Dictionary<string, Ability>();
        abilityButtons = new Dictionary<string, AbilityButton>();

        // Inicializa o dicionário de habilidades
        foreach (Ability ability in abilities)
        {
            abilityDictionary[ability.id] = ability;
        }
    }

    /// <summary>
    /// Inicializa a UI das habilidades
    /// </summary>
    private void Start()
    {
        CreateAbilityButtons();
    }

    /// <summary>
    /// Cria os botões de habilidade na UI
    /// </summary>
    private void CreateAbilityButtons()
    {
        foreach (Ability ability in abilities)
        {
            GameObject buttonObj = Instantiate(abilityButtonPrefab, abilityContainer);
            AbilityButton button = buttonObj.GetComponent<AbilityButton>();
            if (button != null)
            {
                button.Initialize(ability);
                abilityButtons[ability.id] = button;
            }
        }
    }

    /// <summary>
    /// Desbloqueia uma habilidade
    /// </summary>
    /// <param name="abilityId">ID da habilidade</param>
    public void UnlockAbility(string abilityId)
    {
        Ability ability = GetAbility(abilityId);
        if (ability != null && !ability.isUnlocked)
        {
            ability.isUnlocked = true;
            if (abilityButtons.ContainsKey(abilityId))
            {
                abilityButtons[abilityId].UpdateState(true);
            }
            EventManager.Instance.TriggerAbilityUnlocked(abilityId);
        }
    }

    /// <summary>
    /// Verifica se uma habilidade está desbloqueada
    /// </summary>
    /// <param name="abilityId">ID da habilidade</param>
    public bool IsAbilityUnlocked(string abilityId)
    {
        return abilityDictionary.TryGetValue(abilityId, out Ability ability) && ability.isUnlocked;
    }

    /// <summary>
    /// Atualiza a UI dos botões de habilidade
    /// </summary>
    private void UpdateAbilityButtons()
    {
        foreach (var button in abilityButtons.Values)
        {
            if (button != null)
            {
                button.UpdateState(abilityDictionary[button.AbilityId].isUnlocked);
            }
        }
    }

    /// <summary>
    /// Retorna uma habilidade pelo ID
    /// </summary>
    /// <param name="abilityId">ID da habilidade</param>
    public Ability GetAbility(string abilityId)
    {
        abilityDictionary.TryGetValue(abilityId, out Ability ability);
        return ability;
    }

    /// <summary>
    /// Retorna todas as habilidades
    /// </summary>
    public List<Ability> GetAllAbilities()
    {
        return new List<Ability>(abilities);
    }

    /// <summary>
    /// Retorna todas as habilidades desbloqueadas
    /// </summary>
    public List<Ability> GetUnlockedAbilities()
    {
        return abilities.FindAll(a => a.isUnlocked);
    }

    /// <summary>
    /// Reseta todas as habilidades
    /// </summary>
    public void ResetAllAbilities()
    {
        foreach (Ability ability in abilities)
        {
            ability.isUnlocked = false;
        }
        UpdateAbilityButtons();
    }

    /// <summary>
    /// Adiciona uma nova habilidade
    /// </summary>
    /// <param name="ability">Nova habilidade</param>
    public void AddAbility(Ability ability)
    {
        if (!abilityDictionary.ContainsKey(ability.id))
        {
            abilities.Add(ability);
            abilityDictionary[ability.id] = ability;
            
            // Cria o botão da nova habilidade
            GameObject buttonObj = Instantiate(abilityButtonPrefab, abilityContainer);
            AbilityButton button = buttonObj.GetComponent<AbilityButton>();
            
            if (button != null)
            {
                button.Initialize(ability);
                abilityButtons[ability.id] = button;
            }
        }
    }

    /// <summary>
    /// Remove uma habilidade
    /// </summary>
    /// <param name="abilityId">ID da habilidade</param>
    public void RemoveAbility(string abilityId)
    {
        Ability ability = GetAbility(abilityId);
        if (ability != null)
        {
            abilities.Remove(ability);
            abilityDictionary.Remove(abilityId);
            
            // Remove o botão da habilidade
            if (abilityButtons.TryGetValue(abilityId, out AbilityButton button))
            {
                abilityButtons.Remove(abilityId);
                Destroy(button.gameObject);
            }
        }
    }

    public void UseAbility(string abilityId)
    {
        Ability ability = GetAbility(abilityId);
        if (ability != null && ability.isUnlocked)
        {
            // Implement ability usage logic here
            EventManager.Instance.TriggerAbilityUnlocked(abilityId);
        }
    }
} 