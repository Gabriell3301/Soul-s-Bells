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
    // Método para adicionar habilidades ao banco de dados
    public void AddAbility(AbilityData ability)
    {
        if (!allAbilities.Contains(ability))
        {
            allAbilities.Add(ability);
            Debug.Log($"Habilidade '{ability.abilityName}' adicionada ao banco de dados.");
        }
    }
}
