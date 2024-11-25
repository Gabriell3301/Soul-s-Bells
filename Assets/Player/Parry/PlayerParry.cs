using System.Collections;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry Settings")]
    [SerializeField] private float parryWindow = 0.2f; // Tempo de reação do parry
    private bool canParry = false;

    [Header("Parry Cooldown")]
    [SerializeField] private float parryCooldown = 0.4f;
    private bool parryOnCooldown = false;

    private PlayerStateList pState;
    private SpriteRenderer warningEnemyAttack;
    private bool enemyAttackDetected = false;
    private EnemyAttack attackEnemy;

    private void Start()
    {
        pState = GetComponentInParent<PlayerStateList>();
        warningEnemyAttack = GetComponentInChildren<SpriteRenderer>();

        if (warningEnemyAttack != null)
        {
            warningEnemyAttack.color = Color.clear;
        }
        else
        {
            Debug.LogWarning("Nenhum SpriteRenderer encontrado no filho.");
        }
    }

    private void Update()
    {
        // Checa se o jogador apertou "C" e se pode realizar o parry
        if (!parryOnCooldown && Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(PerformParry());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            attackEnemy = collision.GetComponent<EnemyAttack>();
            if (attackEnemy != null)
            {
                enemyAttackDetected = true;

                if (!parryOnCooldown)
                {
                    StartCoroutine(StartParryWindow());
                }
            }
        }
    }

    private IEnumerator StartParryWindow()
    {
        // Abre a janela de parry
        canParry = true;
        warningEnemyAttack.color = Color.yellow;

        yield return new WaitForSeconds(parryWindow);

        // Fecha a janela de parry
        canParry = false;
        warningEnemyAttack.color = Color.clear;

        if (enemyAttackDetected) // O jogador não realizou o parry a tempo
        {
            Debug.Log("Recebeu ataque!");
        }
    }

    private IEnumerator PerformParry()
    {
        pState.SetParring(true);

        if (canParry && enemyAttackDetected)
        {
            // Sucesso no parry
            Debug.Log("Parry bem-sucedido!");
            attackEnemy.Destroythis();
            warningEnemyAttack.color = Color.green;

            // Evite que o jogador tome dano do ataque detectado
            enemyAttackDetected = false;
        }
        else
        {
            Debug.Log("Falhou o parry!");
            if (enemyAttackDetected)
            {
                Debug.Log("Recebeu ataque!");
            }
        }

        yield return new WaitForSeconds(parryWindow); // Garante que o parry termine antes de iniciar o cooldown
        pState.SetParring(false);

        StartCoroutine(ParryCooldown());
    }

    private IEnumerator ParryCooldown()
    {
        parryOnCooldown = true;
        yield return new WaitForSeconds(parryCooldown);
        parryOnCooldown = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            // Ataque saiu da zona de detecção
            enemyAttackDetected = false;
        }
    }
}
