using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia o ataque corpo a corpo dos inimigos.
/// </summary>
public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private LayerMask playerLayer;

    private float nextAttackTime = 0f;
    private Transform player;
    private Enemy enemy;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = GetComponent<Enemy>();
    }

    public void TryAttack()
    {
        if (player != null && Time.time >= nextAttackTime)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= attackRange)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void Attack()
    {
        Debug.Log("Inimigo atacou o jogador!");

        // Verifica se o jogador tem um script de vida
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha o alcance do ataque no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}
