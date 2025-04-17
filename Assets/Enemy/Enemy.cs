using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
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

    [Header("Ataque")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    [Header("Status")]
    public int Hp = 3;
    public int hits = 1;

    [Header("Drops")]
    protected int numMoedas;
    public int numMoedasMin = 2; // Número mínimo de moedas
    public int numMoedasMax = 5; // Número máximo de moedas

    protected Rigidbody2D rb;
    protected Transform player;
    public bool chasingPlayer = false; // Mantido público para ser alterado externamente
    protected float nextFireTime = 0f;
    protected SpriteRenderer spriteRenderer;
    protected bool isFacingRight = true;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Jogador não encontrado! Verifique se o Player tem a tag correta.");
        }
        numMoedas = Random.Range(numMoedasMin, numMoedasMax);

        // Ajuste inicial da direção do sprite
        if (transform.localScale.x < 0)
        {
            isFacingRight = false;
        }
    }

    protected virtual void Update()
    {
        DetectPlayer();
    }

    protected virtual void FixedUpdate()
    {
        if (chasingPlayer)
        {
            ChasePlayer();
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            Patrol();
        }
    }

    protected virtual void DetectPlayer()
    {
        playerDetected = Physics2D.OverlapCircle(transform.position, visionRange, playerLayer);

        if (playerDetected != null && playerDetected.CompareTag("Player"))
        {
            if (!chasingPlayer)
            {
                Debug.Log("Jogador detectado! Iniciando perseguição.");
            }
            chasingPlayer = true;
        }
    }

    protected virtual void Patrol()
    {
        if ((chasingPlayer || points.Count == 0)) return; // ❗ EVITA QUE A PATRULHA CONTINUE ENQUANTO PERSEGUE O JOGADOR

        Transform targetPoint = points[currentPointIndex];

        // Movimento no eixo X
        float directionX = Mathf.Sign(targetPoint.position.x - transform.position.x);
        rb.velocity = new Vector2(directionX * speed, rb.velocity.y);

        // Verifica se precisa dar flip
        if ((directionX > 0 && !isFacingRight) || (directionX < 0 && isFacingRight))
        {
            Flip();
        }

        // Se chegou ao ponto de patrulha
        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.2f)
        {
            currentPointIndex = (currentPointIndex + 1) % points.Count;
        }
    }

    protected virtual void ChasePlayer()
    {
        float stopMargin = 0.4f;
        if (player == null) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float directionX = Mathf.Sign(player.position.x - transform.position.x);
        
        if (distanceToPlayer > stoppingDistance + stopMargin)
        {
            safeStop = false;
            rb.velocity = new Vector2(directionX * chaseSpeed, rb.velocity.y);
        }
        else
        {
            if (!safeStop)
            {
                safeStop = true;
                rb.velocity = Vector2.zero;
            }
        }

        // Verifica se precisa dar flip
        if ((directionX > 0 && !isFacingRight) || (directionX < 0 && isFacingRight))
        {
            Flip();
        }
    }

    public virtual void TakeDamage(int hits)
    {
        Debug.Log("Inimigo levou dano.");
        Hp = Hp - hits;
        if (Hp <= 0)
        {
            Debug.Log("Inimigo morreu.");
            Die();
        }
    }

    public virtual void Die()
    {
        DropManager.Instance.SpawnMoeda(transform.position, numMoedas);
        Destroy(gameObject);
    }

    public virtual void ApplyKnockback(Vector2 force)
    {
        if (!isKnockbacked)
        {
            isKnockbacked = true;
            rb.velocity = Vector2.zero;
            rb.AddForce(force, ForceMode2D.Impulse);
            StartCoroutine(KnockbackRecovery());
        }
    }

    protected IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(0.3f);
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        Gizmos.color = Color.green; // Distância onde o inimigo para
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        Gizmos.color = Color.red; // Distância onde o inimigo começa a se afastar
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }

    // Método público para verificar se o jogador foi detectado
    public bool IsPlayerDetected()
    {
        return playerDetected != null && playerDetected.CompareTag("Player");
    }

    public Vector3 GetPlayerPosition()
    {
        return player != null ? player.position : Vector3.zero;
    }
}
