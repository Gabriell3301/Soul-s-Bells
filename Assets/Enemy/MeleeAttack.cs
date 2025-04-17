using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float attackRange = 1.5f; // Alcance do ataque
    public int attackDamage = 2; // Dano causado ao jogador
    public float attackCooldown = 1.5f; // Tempo entre ataques

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
            playerHealth.TakeHit(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha o alcance do ataque no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
