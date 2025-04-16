using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            EnemyAttack attackComponent = collision.GetComponent<EnemyAttack>();
            if (attackComponent != null)
            {
                // Aplica o dano diretamente quando o ataque atinge o jogador
                playerHealth.TakeHit(attackComponent.Hits);
                attackComponent.Destroythis();
            }
        }
    }
}
