using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAbilitySystem : MonoBehaviour
{
    private List<AbilityData> unlockedAbilities = new List<AbilityData>();
    private List<AbilityData> equippedAbilities = new List<AbilityData>();

    // Método para desbloquear uma habilidade
    public void UnlockAbility(AbilityData ability)
    {
        if (!unlockedAbilities.Contains(ability))
        {
            unlockedAbilities.Add(ability);
            Debug.Log("Habilidade desbloqueada: " + ability.abilityName);

            // Adiciona a habilidade ao AbilityDatabase quando desbloqueada
            AbilityDataBase.Instance.AddAbility(ability);  // Método para adicionar habilidades ao banco

            if (ability.autoEquip && !equippedAbilities.Contains(ability))
            {
                equippedAbilities.Add(ability);
                Debug.Log("Habilidade equipada: " + ability.abilityName);
                // Ativar a habilidade automaticamente quando ela for equipada
                ActivateAbilityScript(ability);
            }
        }
    }
    public void EquipAbility(AbilityData ability)
    {
        if (ability.unlocked && !equippedAbilities.Contains(ability))
        {
            equippedAbilities.Add(ability);
            ability.equipped = true;
        }
    }
    public List<AbilityData> GetEquippedAbilities() => equippedAbilities;
    public List<AbilityData> GetUnlockedAbilities() => unlockedAbilities;
    public void ClearAbilities()
    {
        foreach (var ability in unlockedAbilities)
        {
            ability.unlocked = false;
            ability.equipped = false;
        }

        unlockedAbilities.Clear();
        equippedAbilities.Clear();
    }
    // Método para ativar o script da habilidade
    private void ActivateAbilityScript(AbilityData ability)
    {
        if (ability == null)
        {
            Debug.LogWarning("Habilidade inválida.");
            return;
        }

        if (ability.abilityScript == null)
        {
            Debug.LogWarning($"A habilidade '{ability.abilityName}' não tem um script associado.");
            return;
        }

        // Obtém o tipo do MonoBehaviour a partir do MonoScript
        Type abilityType = ability.abilityScript.GetClass(); // Isso retorna o tipo da classe MonoBehaviour associada ao MonoScript

        if (abilityType == null || !typeof(MonoBehaviour).IsAssignableFrom(abilityType))
        {
            Debug.LogWarning($"O script '{ability.abilityName}' não é válido.");
            return;
        }

        Component existingComponent = gameObject.GetComponent(abilityType);

        if (existingComponent == null)
        {
            Component newComponent = gameObject.AddComponent(abilityType);
            Debug.Log($"Script '{ability.abilityScript.name}' adicionado ao player.");

            // Inicializa, se suportar a interface
            if (newComponent is IAbility abilityComponent)
            {
                abilityComponent.Initialize(ability);
            }
            else
            {
                Debug.LogWarning($"O script '{ability.abilityScript.name}' não implementa IAbility.");
            }
        }
        else
        {
            Debug.Log($"O script '{ability.abilityScript.name}' já está ativo no player.");
        }
    }
}
