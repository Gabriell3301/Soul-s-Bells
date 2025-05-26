using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject unlockedAbilityButtonPrefab; // Prefab para habilidades desbloqueadas
    [SerializeField] private GameObject lockedAbilityButtonPrefab; // Prefab para habilidades bloqueadas (com cadeado)
    [SerializeField] private GameObject categoryHeaderPrefab; // Prefab para cabeçalhos de categoria (opcional)
    [SerializeField] private Transform abilityListParent; // Parent onde os botões serão instanciados
    [SerializeField] private Button refreshButton; // Botão para atualizar a lista (opcional)
    [SerializeField] private bool organizeByCategory = false; // Se true, organiza as habilidades por categoria
    
    [Header("Player Reference")]
    [SerializeField] private PlayerAbilitySystem playerAbilitySystem;
    
    private List<AbilityUIButton> abilityUIButtons = new List<AbilityUIButton>();
    
    private void Start()
    {
        // Se não foi definido no inspector, tenta encontrar o PlayerAbilitySystem
        if (playerAbilitySystem == null)
            playerAbilitySystem = FindObjectOfType<PlayerAbilitySystem>();
            
        // Configura o botão de refresh se existir
        if (refreshButton != null)
            refreshButton.onClick.AddListener(RefreshAbilityList);
            
        RefreshAbilityList();
    }
    
    public void RefreshAbilityList()
    {
        ClearAbilityList();
        CreateAbilityButtons();
    }
    
    private void ClearAbilityList()
    {
        // Remove todos os botões existentes
        foreach (var button in abilityUIButtons)
        {
            if (button != null && button.gameObject != null)
                Destroy(button.gameObject);
        }
        abilityUIButtons.Clear();
    }
    
    private void CreateAbilityButtons()
    {
        if (playerAbilitySystem == null || AbilityDataBase.Instance == null) return;
        
        // Pega todas as habilidades do banco de dados
        List<AbilityData> allAbilities = GetAllAbilitiesFromDatabase();
        List<AbilityData> unlockedAbilities = playerAbilitySystem.GetUnlockedAbilities();
        List<AbilityData> equippedAbilities = playerAbilitySystem.GetEquippedAbilities();
        
        if (organizeByCategory && categoryHeaderPrefab != null)
        {
            CreateAbilitiesWithCategories(allAbilities, unlockedAbilities, equippedAbilities);
        }
        else
        {
            CreateAbilitiesSimple(allAbilities, unlockedAbilities, equippedAbilities);
        }
    }
    private List<AbilityData> GetAllAbilitiesFromDatabase()
    {
        return AbilityDataBase.Instance.GetAllAbilities();
    }
    private void CreateAbilitiesSimple(List<AbilityData> allAbilities, List<AbilityData> unlockedAbilities, List<AbilityData> equippedAbilities)
    {
        foreach (AbilityData ability in allAbilities)
        {
            CreateAbilityButton(ability, unlockedAbilities, equippedAbilities);
        }
    }
    
    private void CreateAbilitiesWithCategories(List<AbilityData> allAbilities, List<AbilityData> unlockedAbilities, List<AbilityData> equippedAbilities)
    {
        // Agrupa habilidades por categoria
        Dictionary<string, List<AbilityData>> categorizedAbilities = new Dictionary<string, List<AbilityData>>();
        
        foreach (AbilityData ability in allAbilities)
        {
            string category = string.IsNullOrEmpty(ability.category) ? "Geral" : ability.category;
            
            if (!categorizedAbilities.ContainsKey(category))
                categorizedAbilities[category] = new List<AbilityData>();
                
            categorizedAbilities[category].Add(ability);
        }
        
        // Cria UI para cada categoria
        foreach (var categoryPair in categorizedAbilities)
        {
            // Cria cabeçalho da categoria
            if (categoryHeaderPrefab != null)
            {
                GameObject headerObj = Instantiate(categoryHeaderPrefab, abilityListParent);
                AbilityCategoryHeader header = headerObj.GetComponent<AbilityCategoryHeader>();
                
                if (header == null)
                    header = headerObj.AddComponent<AbilityCategoryHeader>();
                    
                header.Setup(categoryPair.Key);
            }
            
            // Cria botões das habilidades da categoria
            foreach (AbilityData ability in categoryPair.Value)
            {
                CreateAbilityButton(ability, unlockedAbilities, equippedAbilities);
            }
        }
    }
    
    private void CreateAbilityButton(AbilityData ability, List<AbilityData> unlockedAbilities, List<AbilityData> equippedAbilities)
    {
        bool isUnlocked = unlockedAbilities.Contains(ability);
        bool isEquipped = equippedAbilities.Contains(ability);
        
        GameObject buttonPrefab = isUnlocked ? unlockedAbilityButtonPrefab : lockedAbilityButtonPrefab;
        GameObject buttonObj = Instantiate(buttonPrefab, abilityListParent);
        
        if (isUnlocked)
        {
            // Configura botão desbloqueado
            AbilityUIButton abilityButton = buttonObj.GetComponent<AbilityUIButton>();
            
            if (abilityButton == null)
                abilityButton = buttonObj.AddComponent<AbilityUIButton>();
            
            abilityButton.Setup(ability, isEquipped, this);
            abilityUIButtons.Add(abilityButton);
        }
        else
        {
            // Configura botão bloqueado
            LockedAbilityUIButton lockedButton = buttonObj.GetComponent<LockedAbilityUIButton>();
            
            if (lockedButton == null)
                lockedButton = buttonObj.AddComponent<LockedAbilityUIButton>();
            
            lockedButton.Setup(ability);
        }
    }
    
    public void OnAbilityButtonClicked(AbilityData ability, bool currentlyEquipped)
    {
        if (currentlyEquipped)
        {
            UnequipAbility(ability);
        }
        else
        {
            EquipAbility(ability);
        }
        
        // Atualiza apenas o estado visual dos botões sem recriar tudo
        UpdateButtonStates();
    }
    
    private void EquipAbility(AbilityData ability)
    {
        playerAbilitySystem.EquipAbility(ability);
        Debug.Log($"Habilidade '{ability.abilityName}' equipada!");
    }
    
    private void UnequipAbility(AbilityData ability)
    {
        playerAbilitySystem.UnequipAbility(ability);
        Debug.Log($"Habilidade '{ability.abilityName}' desequipada!");
    }
    
    private void UpdateButtonStates()
    {
        List<AbilityData> equippedAbilities = playerAbilitySystem.GetEquippedAbilities();
        
        foreach (var button in abilityUIButtons)
        {
            if (button != null && button.AbilityData != null)
            {
                bool isEquipped = equippedAbilities.Contains(button.AbilityData);
                button.UpdateEquippedState(isEquipped);
            }
        }
    }
}