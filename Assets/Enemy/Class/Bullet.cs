using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla o comportamento de projéteis disparados por inimigos à distância.
/// </summary>
public class Bullet : MonoBehaviour, IEnemyAttack
{
    [Header("Configurações")]
    [SerializeField] private float speed = 10f; // Velocidade do projétil
    [SerializeField] private int damage = 5; // Dano causado
    [SerializeField] private float lifetime = 5f; // Tempo de vida do projétil
    [SerializeField] private LayerMask playerLayer; // Layer do jogador
    [SerializeField] private LayerMask groundLayer; // Layer do chão
    [SerializeField] private LayerMask enemyLayer; // Layer dos inimigos

    private Vector2 direction; // Direção do movimento
    private bool isReflected = false; // Indica se o projétil foi refletido

    public float Speed => speed;
    public int Damage => damage;

    /// <summary>
    /// Inicializa o projétil com as configurações necessárias
    /// </summary>
    /// <param name="bulletSpeed">Velocidade do projétil</param>
    /// <param name="bulletDamage">Dano causado</param>
    /// <param name="targetPosition">Posição do alvo</param>
    public void Initialize(float bulletSpeed, int bulletDamage, Vector2 targetPosition)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        
        // Calcula a direção para o alvo
        direction = (targetPosition - (Vector2)transform.position).normalized;

        // Destrói o projétil após o tempo de vida
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Atualiza a posição do projétil a cada frame
    /// </summary>
    private void Update()
    {
        // Move o projétil
        transform.Translate(direction * speed * Time.deltaTime);
    }

    /// <summary>
    /// Detecta colisões do projétil
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se o projétil foi refletido, verifica colisão com inimigos
        if (isReflected)
        {
            if (((1 << other.gameObject.layer) & enemyLayer) != 0)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null && !enemy.die)
                {
                    enemy.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }
        // Se não foi refletido, verifica colisão com jogador
        else
        {
            if (((1 << other.gameObject.layer) & playerLayer) != 0)
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }

        // Verifica se atingiu o chão
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnParried()
    {
        // O projétil será refletido pelo PlayerParry
    }

    public void OnReflected(Enemy target)
    {
        isReflected = true;
        // Atualiza a direção para o inimigo alvo
        direction = (target.transform.position - transform.position).normalized;
        // Muda a layer do projétil para não colidir com o jogador
        gameObject.layer = LayerMask.NameToLayer("EnemyAttack");
    }

    /// <summary>
    /// Desenha gizmos para debug
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
} 