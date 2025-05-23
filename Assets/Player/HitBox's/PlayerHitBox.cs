using UnityEngine;

/// <summary>
/// Gerencia a hitbox do jogador para detecção de dano.
/// </summary>
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
            enabled = false;
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerState.IsInvincible()) return;

        var attackComponent = other.GetComponent<AttackComponent>();
        if (attackComponent != null)
        {
            playerHealth.TakeDamage(attackComponent.Hits);
        }
    }
}
