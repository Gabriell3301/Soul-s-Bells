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
            
            // Ativa o script da habilidade quando equipada
            ActivateAbilityScript(ability);
            
            Debug.Log($"Habilidade '{ability.abilityName}' equipada!");
        }
    }
    
    // Novo método para desequipar habilidade
    public void UnequipAbility(AbilityData ability)
    {
        if (equippedAbilities.Contains(ability))
        {
            equippedAbilities.Remove(ability);
            ability.equipped = false;
            
            // Desativa o script da habilidade quando desequipada
            DeactivateAbilityScript(ability);
            
            Debug.Log($"Habilidade '{ability.abilityName}' desequipada!");
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
            
            // Desativa todos os scripts quando limpa as habilidades
            DeactivateAbilityScript(ability);
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
            // Reativa o componente se já existir
            if (existingComponent is MonoBehaviour mono)
            {
                mono.enabled = true;
            }
            Debug.Log($"O script '{ability.abilityScript.name}' já está ativo no player.");
        }
    }
    
    // Novo método para desativar o script da habilidade
    private void DeactivateAbilityScript(AbilityData ability)
    {
        if (ability == null || ability.abilityScript == null)
            return;

        Type abilityType = ability.abilityScript.GetClass();
        
        if (abilityType == null)
            return;

        Component existingComponent = gameObject.GetComponent(abilityType);

        if (existingComponent != null)
        {
            // Opção 1: Apenas desabilitar o componente
            if (existingComponent is MonoBehaviour mono)
            {
                mono.enabled = false;
                Debug.Log($"Script '{ability.abilityScript.name}' desabilitado.");
            }
            
            // Opção 2: Remover completamente o componente (descomente se preferir)
            // Destroy(existingComponent);
            // Debug.Log($"Script '{ability.abilityScript.name}' removido do player.");
        }
    }
}