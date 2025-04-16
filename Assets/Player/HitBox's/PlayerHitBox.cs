using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerStateList playerState;

    private void Start()
    {
        // Pega as referências necessárias
        playerHealth = GetComponentInParent<PlayerHealth>();
        playerState = GetComponentInParent<PlayerStateList>();

        // Verifica se encontrou todas as dependências
        if (playerHealth == null || playerState == null)
        {
            Debug.LogError("PlayerHitBox: PlayerHealth ou PlayerStateList não encontrado!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se não for um ataque inimigo ou o player estiver invulnerável, ignora
        if (!collision.CompareTag("EnemyAttack") || playerState.isInvulnerable)
            return;

        EnemyAttack attackComponent = collision.GetComponent<EnemyAttack>();
        if (attackComponent != null)
        {
            // Aplica o dano ao player
            playerHealth.TakeHit(attackComponent.Hits);
            
            // Destroi o ataque
            attackComponent.Destroythis();
            
            Debug.Log($"Player tomou {attackComponent.Hits} de dano por colisão direta.");
        }
    }
}
