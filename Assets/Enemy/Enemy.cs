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
    private int currentPointIndex = 0;
    public float stoppingDistance = 2f; // Distância mínima antes de parar
    public float retreatDistance = 1f;  // Distância onde ele começa a se afastar
    private bool safeStop = false;
    private bool isKnockbacked = false; // 🔹 Impede que o knockback seja cancelado

    [Header("Detecção do Jogador")]
    public float visionRange = 5f;
    public LayerMask playerLayer;
    public float lostPlayerTime = 2f;  // Tempo antes de voltar a patrulha
    public Collider2D playerDetected;

    [Header("Ataque")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    [Header("Status")]
    public int Hp = 3;
    public int hits = 1;

    [Header("Drops")]
    public GameObject coinPrefab; // Prefab da moeda
    public int numMoedasMin = 2; // Número mínimo de moedas
    public int numMoedasMax = 5; // Número máximo de moedas

    private Rigidbody2D rb;
    private Transform player;
    public bool chasingPlayer = false; // Mantido público para ser alterado externamente
    private float nextFireTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Jogador não encontrado! Verifique se o Player tem a tag correta.");
        }
    }

    void Update()
    {
        DetectPlayer();
    }

    void FixedUpdate()
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

    private void DropCoins()
    {
        // Determina o número aleatório de moedas a serem droppadas
        int numMoedas = Random.Range(numMoedasMin, numMoedasMax);

        for (int i = 0; i < numMoedas; i++)
        {
            // Posiciona a moeda no local do inimigo
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, 0); // Aleatoriza um pouco a posição

            // Instancia a moeda
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        }
    }
    void DetectPlayer()
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

    void Patrol()
    {
        if ((chasingPlayer || points.Count == 0)) return; // ❗ EVITA QUE A PATRULHA CONTINUE ENQUANTO PERSEGUE O JOGADOR

        Transform targetPoint = points[currentPointIndex];

        // Movimento no eixo X
        float directionX = Mathf.Sign(targetPoint.position.x - transform.position.x);
        rb.velocity = new Vector2(directionX * speed, rb.velocity.y);

        // Se chegou ao ponto de patrulha
        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.2f)
        {
            currentPointIndex = (currentPointIndex + 1) % points.Count;
            Flip();
        }
    }
    public void TakeDamage(int hits)
    {
        Debug.Log("Inimigo levou dano.");
        Hp = Hp - hits;
        if (Hp <= 0)
        {
            Debug.Log("Inimigo morreu.");
            Die();
        }
    }
    public void Die()
    {
        DropCoins();
        Destroy(gameObject);
    }
    public void ApplyKnockback(Vector2 force)
    {
        if (!isKnockbacked) // Só aplica knockback se não estiver sendo empurrado
        {
            isKnockbacked = true;
            rb.velocity = Vector2.zero; // Reseta o movimento antes do knockback
            rb.AddForce(force, ForceMode2D.Impulse);
            StartCoroutine(KnockbackRecovery());
        }
    }

    private IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(0.3f); // Tempo de knockback antes de retomar o movimento
        isKnockbacked = false;
    }
    void ChasePlayer()
    {
        float stopMargin = 0.4f;
        if (player == null) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float directionX = Mathf.Sign(player.position.x - transform.position.x);
        if (true)
        {
            if (distanceToPlayer > stoppingDistance + stopMargin) // Persegue o jogador até uma certa distância
            {
                Debug.Log("Se movendo perto");
                safeStop = false;  // Agora pode se mover novamente
                rb.velocity = new Vector2(directionX * chaseSpeed, rb.velocity.y);
            }
            else if (distanceToPlayer < retreatDistance - stopMargin) // Se estiver muito perto, se afasta
            {
                Debug.Log("Se movendo longe");
                safeStop = false;  // Resetamos para que ele possa se afastar
                rb.velocity = new Vector2(-directionX * chaseSpeed, rb.velocity.y);
            }
            else // Se estiver dentro do intervalo aceitável, para completamente
            {
                if (!safeStop) // Apenas define `safeStop` se ainda não estiver ativado
                {
                    Debug.Log("Parado");
                    safeStop = true;
                    rb.velocity = Vector2.zero;
                }
            }
        }

        Flip(); // Mantém o inimigo virado para o jogador
    }

    void Flip()
    {
        if (player != null && chasingPlayer)
        {
            if ((player.position.x > transform.position.x && transform.localScale.x < 0) ||
                (player.position.x < transform.position.x && transform.localScale.x > 0))
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        Gizmos.color = Color.green; // Distância onde o inimigo para
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        Gizmos.color = Color.red; // Distância onde o inimigo começa a se afastar
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}
