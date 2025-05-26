using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Classe base para todos os inimigos do jogo.
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    [Header("Movimentação")]
    public float speed = 2f;
    public float chaseSpeed = 3f;  // Velocidade aumentada na perseguição
    public List<Transform> points;
    protected int currentPointIndex = 0;
    public float stoppingDistance = 2f; // Distância mínima antes de parar
    public float retreatDistance = 1f;  // Distância onde ele começa a se afastar
    protected bool safeStop = false;
    protected bool isKnockbacked = false; // 🔹 Impede que o knockback seja cancelado

    [Header("Detecção do Jogador")]
    public float visionRange = 5f;
    public LayerMask playerLayer;
    public float lostPlayerTime = 2f;  // Tempo antes de voltar a patrulha
    protected Collider2D playerDetected;
    private float lostPlayerTimer = 0f; // Timer para perder o jogador

    [Header("Status")]
    private bool canBeHit = true;
    public int Hp = 3;
    public int hits = 1;

    [Header("Drops")]
    protected int numMoedas;
    public int numMoedasMin = 2; // Número mínimo de moedas
    public int numMoedasMax = 5; // Número máximo de moedas

    [Header("Animator")]
    [SerializeField] protected Animator animator;
    protected Rigidbody2D rb;
    protected Collider2D enemyCollider; // Referência ao colisor
    protected Transform player;
    public bool chasingPlayer = false; // Mantido público para ser alterado externamente
    protected SpriteRenderer spriteRenderer;
    protected bool isFacingRight = true;
    public bool die = false;
    private float deathTimer = 0f;
    
    // Constantes para melhor organização
    private const float DEATH_DESTROY_TIME = 4f;
    private const float HIT_COOLDOWN_TIME = 0.7f;
    private const float KNOCKBACK_RECOVERY_TIME = 0.3f;
    private const float PATROL_REACH_THRESHOLD = 0.2f;
    private const float CHASE_STOP_MARGIN = 0.4f;


    protected virtual void Start()
    {
        InitializeComponents();
        InitializeValues();
        ValidateSetup();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>(); // Pega o colisor
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void InitializeValues()
    {
        numMoedas = Random.Range(numMoedasMin, numMoedasMax + 1); // +1 porque Range é exclusivo no max para int
        
        // Ajuste inicial da direção do sprite
        if (transform.localScale.x < 0)
        {
            isFacingRight = false;
        }
    }

    private void ValidateSetup()
    {
        if (player == null)
        {
            Debug.LogError($"Jogador não encontrado em {gameObject.name}! Verifique se o Player tem a tag correta.");
        }
        
        if (enemyCollider == null)
        {
            Debug.LogError($"Collider2D não encontrado em {gameObject.name}!");
        }
        
        if (points.Count == 0)
        {
            Debug.LogWarning($"Nenhum ponto de patrulha definido para {gameObject.name}!");
        }
    }

    protected virtual void Update()
    {
        if (!die)
        {
            DetectPlayer();
            HandlePlayerLost();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!die)
        {
            if (chasingPlayer)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }
            HandleAnimation();
        }
        else
        {
            HandleDeathTimer();
        }
    }

    private void HandleDeathTimer()
    {
        deathTimer += Time.deltaTime;
        if (deathTimer >= DEATH_DESTROY_TIME)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void DetectPlayer()
    {
        playerDetected = Physics2D.OverlapCircle(transform.position, visionRange, playerLayer);

        if (playerDetected != null && playerDetected.CompareTag("Player"))
        {
            if (!chasingPlayer)
            {
                Debug.Log($"{gameObject.name}: Jogador detectado! Iniciando perseguição.");
                chasingPlayer = true;
            }
            lostPlayerTimer = 0f; // Reset do timer quando detecta o jogador
        }
    }

    private void HandlePlayerLost()
    {
        if (chasingPlayer && (playerDetected == null || !playerDetected.CompareTag("Player")))
        {
            lostPlayerTimer += Time.deltaTime;
            if (lostPlayerTimer >= lostPlayerTime)
            {
                Debug.Log($"{gameObject.name}: Perdeu o jogador, voltando à patrulha.");
                chasingPlayer = false;
                lostPlayerTimer = 0f;
            }
        }
    }

    protected virtual void Patrol()
    {
        if (chasingPlayer || points.Count == 0 || isKnockbacked) return;

        Transform targetPoint = points[currentPointIndex];

        // Movimento no eixo X
        float directionX = Mathf.Sign(targetPoint.position.x - transform.position.x);
        
        // Só move se não estiver em knockback
        if (!isKnockbacked)
        {
            rb.velocity = new Vector2(directionX * speed, rb.velocity.y);
        }

        // Verifica se precisa dar flip
        HandleFlip(directionX);

        // Se chegou ao ponto de patrulha
        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < PATROL_REACH_THRESHOLD)
        {
            currentPointIndex = (currentPointIndex + 1) % points.Count;
        }
    }

    protected virtual void HandleAnimation()
    {
        // Walk Animation - só anima se não estiver morto
        if (animator != null)
        {
            bool isMoving = Mathf.Abs(rb.velocity.x) > 0.1f; // Pequena margem para evitar oscilação
            animator.SetBool("isWalking", isMoving);
        }
    }

    protected virtual void ChasePlayer()
    {
        if (player == null || isKnockbacked) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float directionX = Mathf.Sign(player.position.x - transform.position.x);
        
        if (distanceToPlayer > stoppingDistance + CHASE_STOP_MARGIN)
        {
            safeStop = false;
            rb.velocity = new Vector2(directionX * chaseSpeed, rb.velocity.y);
        }
        else if (distanceToPlayer < retreatDistance)
        {
            // Se muito próximo, recua um pouco
            rb.velocity = new Vector2(-directionX * speed * 0.5f, rb.velocity.y);
            safeStop = false;
        }
        else
        {
            if (!safeStop)
            {
                safeStop = true;
                rb.velocity = new Vector2(0, rb.velocity.y); // Mantém a velocidade Y para gravidade
            }
        }

        // Verifica se precisa dar flip
        HandleFlip(directionX);
    }

    private void HandleFlip(float directionX)
    {
        if ((directionX > 0 && !isFacingRight) || (directionX < 0 && isFacingRight))
        {
            Flip();
        }
    }
    public void TakeDamage(int amount, DamageType type, Vector2 sourcePosition, float knockbackForce)
    {
        if (!canBeHit || die) return;

        // Aplica dano com tipo específico
        Hp = Mathf.Max(0, Hp - amount);
        Debug.Log($"{gameObject.name} levou {amount} de dano do tipo {type}");

        // Animação de hit
        if (animator != null)
        {
            animator.SetTrigger("Hit");
            animator.SetBool("isAlive", true);
        }

        // Aplica knockback
        ApplyKnockback(sourcePosition, knockbackForce);

        // Verifica morte
        if (Hp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HitCooldown());
        }
    }
        // Versão simplificada mantendo compatibilidade
    public virtual void TakeDamage(int damage)
    {
        TakeDamage(damage, DamageType.Slashing, transform.position, 0f);
    }

    public virtual void Die()
    {
        if (die) return; // Evita múltiplas chamadas de Die()

        Debug.Log($"{gameObject.name} morreu.");
        die = true;
        
        // Para o movimento completamente
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; // Kinematic em vez de Static para manter a posição sem física
        
        // Remove a hitbox (desabilita o colisor)
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }
        
        // Para de perseguir o jogador
        chasingPlayer = false;
        
        // Spawna as moedas
        if (DropManager.Instance != null)
        {
            DropManager.Instance.SpawnMoeda(transform.position, numMoedas);
        }
        
        // Trigger da animação de morte
        if (animator != null)
        {
            animator.SetBool("isAlive", false);
            animator.SetTrigger("Dying");
        }
        
        // Chama método virtual para comportamentos específicos de cada inimigo
        OnDeath();
    }

    // Método virtual para ser sobrescrito por inimigos específicos
    protected virtual void OnDeath()
    {
        // Implementação específica de cada inimigo
    }

    private void ApplyKnockback(Vector2 sourcePosition, float knockbackForce)
    {
        if (knockbackForce <= 0 || isKnockbacked) return;

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            // Calcula direção do knockback (da fonte do dano para o inimigo)
            Vector2 knockbackDirection = ((Vector2)transform.position - sourcePosition).normalized;
            
            // Aplica força
            rb.velocity = Vector2.zero;
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            
            // Ativa estado de knockback
            isKnockbacked = true;
            StartCoroutine(KnockbackRecovery());
            
            Debug.Log($"Knockback aplicado: Força {knockbackForce}, Direção {knockbackDirection}");
        }
        else
        {
            Debug.LogWarning("Knockback não aplicado: Rigidbody2D não encontrado");
        }
    }

    private IEnumerator HitCooldown()
    {
        canBeHit = false;
        yield return new WaitForSeconds(HIT_COOLDOWN_TIME);
        canBeHit = true;
    }

    protected IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(KNOCKBACK_RECOVERY_TIME);
        isKnockbacked = false;
    }

    protected virtual void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // Alcance de visão
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        
        // Distância onde o inimigo para
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        
        // Distância onde o inimigo começa a se afastar
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
        
        // Linha conectando os pontos de patrulha
        if (points != null && points.Count > 1)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i] != null)
                {
                    Vector3 currentPoint = points[i].position;
                    Vector3 nextPoint = points[(i + 1) % points.Count].position;
                    
                    Gizmos.DrawLine(currentPoint, nextPoint);
                    Gizmos.DrawWireSphere(currentPoint, 0.3f);
                }
            }
        }
    }

    // Métodos públicos para acesso externo
    public bool IsPlayerDetected()
    {
        return playerDetected != null && playerDetected.CompareTag("Player");
    }

    public Vector3 GetPlayerPosition()
    {
        return player != null ? player.position : Vector3.zero;
    }

    public bool IsDead()
    {
        return die;
    }

    public bool IsChasing()
    {
        return chasingPlayer;
    }
    
    public float GetHealthPercentage()
    {
        return Hp > 0 ? (float)Hp / 3f : 0f; // Assumindo HP máximo de 3
    }
}