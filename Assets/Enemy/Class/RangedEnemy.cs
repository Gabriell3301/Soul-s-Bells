using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe que implementa um inimigo à distância, herdando comportamentos base do Enemy.
/// </summary>
public class RangedEnemy : Enemy
{
    [Header("Configurações de Ataque")]
    [SerializeField] private GameObject bulletPrefab; // Prefab do projétil
    [SerializeField] private Transform firePoint; // Ponto de onde o projétil será disparado
    [SerializeField] private float bulletSpeed = 10f; // Velocidade do projétil
    [SerializeField] private int bulletDamage = 5; // Dano do projétil
    [SerializeField] private float attackCooldown = 2f; // Tempo entre ataques
    [SerializeField] private float attackDelay = 0.7f; // Delay entre animação e disparo

    private float attackTimer; // Timer para cooldown do ataque
    private bool canFire = true; // Indica se pode atirar
    private bool isAttacking = false; // Indica se está em animação de ataque

    /// <summary>
    /// Inicializa componentes específicos do inimigo à distância
    /// </summary>
    protected override void Start()
    {
        base.Start();
        attackTimer = attackCooldown;
    }

    /// <summary>
    /// Atualiza o estado do inimigo à distância
    /// </summary>
    protected override void Update()
    {
        if (die) return;
        
        base.Update();

        // Atualiza o timer de ataque
        if (!canFire)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                canFire = true;
                attackTimer = attackCooldown;
            }
        }
    }

    /// <summary>
    /// Realiza o disparo
    /// </summary>
    private void Fire()
    {
        if (die || !canFire || bulletPrefab == null || firePoint == null || isAttacking) return;

        canFire = false;
        isAttacking = true;
        animator.SetTrigger("Attack");
        StartCoroutine(DelayedFire());
    }

    private IEnumerator DelayedFire()
    {
        yield return new WaitForSeconds(attackDelay);
        
        if (!die) // Verifica se ainda está vivo após o delay
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.Initialize(bulletSpeed, bulletDamage, player.position);
            }
        }
        
        isAttacking = false;
    }

    /// <summary>
    /// Persegue o jogador com comportamento específico para inimigo à distância
    /// </summary>
    protected override void ChasePlayer()
    {
        if (die || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Se estiver muito perto, recua
        if (distanceToPlayer < retreatDistance)
        {
            Vector2 direction = (transform.position - player.position).normalized;
            rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);
        }
        // Se estiver no alcance de ataque, para e atira
        else if (distanceToPlayer <= visionRange)
        {
            rb.velocity = Vector2.zero;
            if (canFire && !isAttacking)
            {
                Fire();
            }
        }
        // Caso contrário, persegue o jogador
        else
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);
        }

        // Vira o sprite se necessário
        if ((rb.velocity.x > 0 && !isFacingRight) || (rb.velocity.x < 0 && isFacingRight))
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}