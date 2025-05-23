using System;
using UnityEngine;

public class ItensForAbilities : MonoBehaviour
{
    [SerializeField] private AbilityData abilityToUnlock;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var abilitySystem = other.GetComponent<PlayerAbilitySystem>();
            if (abilitySystem != null && abilityToUnlock != null)
            {
                abilitySystem.UnlockAbility(abilityToUnlock);
                Destroy(gameObject); // Remove o item do mundo
            }
        }
    }
}
