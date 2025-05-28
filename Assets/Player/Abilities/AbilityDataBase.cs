using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDataBase : MonoBehaviour
{
    public static AbilityDataBase Instance;
    // Include All abilities, including unlocked and locked
    [SerializeField] private List<AbilityData> allAbilities;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // Garantir que o AbilityDatabase persista entre cenas
    }

    public AbilityData GetAbilityByName(string name)
    {
        return allAbilities.Find(a => a.abilityName == name);
    }
    
    // Novo método para obter todas as habilidades
    public List<AbilityData> GetAllAbilities()
    {
        return new List<AbilityData>(allAbilities); // Retorna uma cópia para evitar modificações acidentais
    }
    
    // Método para adicionar habilidades ao banco de dados
    public void AddAbility(AbilityData ability)
    {
        if (!allAbilities.Contains(ability))
        {
            allAbilities.Add(ability);
            Debug.Log($"Habilidade '{ability.abilityName}' adicionada ao banco de dados.");
        }
    }
    
    // Método para verificar se uma habilidade existe no banco
    public bool HasAbility(AbilityData ability)
    {
        return allAbilities.Contains(ability);
    }
    
    // Método para obter habilidades por categoria (se você tiver categorias)
    public List<AbilityData> GetAbilitiesByCategory(string category)
    {
        return allAbilities.FindAll(a => a.category == category); // Assumindo que AbilityData tem um campo category
    }
}