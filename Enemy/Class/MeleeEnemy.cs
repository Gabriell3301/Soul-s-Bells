using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe que implementa um inimigo corpo a corpo, herdando comportamentos base do Enemy.
/// </summary>
public class MeleeEnemy : Enemy
{
    [Header("Configurações de Ataque")]
    [SerializeField] private float attackRadius = 0.5f; // Raio de detecção do ataque
    [SerializeField] private int attackDamage = 10; // Dano do ataque
    [SerializeField] private float attackCooldown = 1f; // Tempo entre ataques
    [SerializeField] private float attackRange = 1f; // Alcance do ataque
    [SerializeField] private float attackDuration = 0.5f; // Duração do ataque

    private float attackTimer; // Timer para cooldown do ataque
    private bool canAttack = true; // Indica se pode atacar

    /// <summary>
    /// Inicializa componentes específicos do inimigo corpo a corpo
    /// </summary>
    protected override void Start()
    {
        base.Start();
        attackTimer = attackCooldown;
    }

    /// <summary>
    /// Atualiza o estado do inimigo corpo a corpo
    /// </summary>
    protected override void Update()
    {
        base.Update();

        if (die) return;

        // Atualiza o timer de ataque
        if (!canAttack)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                canAttack = true;
                attackTimer = attackCooldown;
            }
        }
    }

    /// <summary>
    /// Inicia o ataque do inimigo
    /// </summary>
    private void StartAttack()
    {
        if (!canAttack) return;

        canAttack = false;
        animator.SetTrigger("Attack");

        // Detecta jogadores no alcance do ataque
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRadius, playerLayer);

        // Aplica dano em todos os jogadores atingidos
        foreach (Collider2D player in hitPlayers)
        {
            if (player.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        // Reseta o estado de ataque após a duração
        StartCoroutine(ResetAttackState());
    }

    /// <summary>
    /// Reseta o estado de ataque após a duração
    /// </summary>
    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(attackDuration);
        canAttack = true;
    }

    /// <summary>
    /// Persegue o jogador com comportamento específico para inimigo corpo a corpo
    /// </summary>
    protected override void ChasePlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Se estiver no alcance de ataque, para e ataca
        if (distanceToPlayer <= attackRange)
        {
            rb.velocity = Vector2.zero;
            if (canAttack)
            {
                StartAttack();
            }
            return;
        }

        // Caso contrário, persegue o jogador
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);

        // Vira o sprite se necessário
        if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
        {
            Flip();
        }
    }

    /// <summary>
    /// Desenha gizmos para debug
    /// </summary>
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
} 