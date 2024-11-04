using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry Settings")]
    [SerializeField] private float parryWindow = 0.2f; // Tempo de reação do parry
    private bool canParry = false;

    [Header("Parry Cooldown")]
    [SerializeField] private float parryCooldown = 0.4f;
    private bool parryOnCooldown = false;
    private bool windowParry;

    //private Animator animator; // Para animações de parry // Ajeitar as animações
    private PlayerStateList pState;
    private SpriteRenderer warningEnemyAttack;
    private bool enemyAttackDetected = false;
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

        //animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Checa se o jogador apertou "C"
        if (!parryOnCooldown && !pState.parring && Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(PerformParry());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            enemyAttackDetected = true;
            if (!parryOnCooldown)
            {
                // Inicia a janela curta de tempo para o parry
                StartCoroutine(StartParryWindow());
            }
        }
    }

    private IEnumerator StartParryWindow()
    {
        // Ativa o aviso visual de parry
        warningEnemyAttack.color = Color.yellow;
        canParry = true;
        // Janela curta de parry
        yield return new WaitForSeconds(parryWindow);

        if (!pState.parring || (pState.parring && !canParry))
        {
            // Jogador recebe o ataque
            Debug.Log("Recebeu ataque");
        }
        // Fecha a janela de parry
        enemyAttackDetected = false;
        canParry = false;
        warningEnemyAttack.color = Color.clear;
    }

    private IEnumerator PerformParry()
    {
        pState.SetParring(true);
        // Sucesso no parry
        if (enemyAttackDetected && canParry)
        {
            Debug.Log("Parry bem-sucedido!");
            warningEnemyAttack.color = Color.green; // Sinaliza parry bem-sucedido
        }
        // Inicia o cooldown do parry
        StartCoroutine(ParryCooldown());
        yield return null;
    }

    private IEnumerator ParryCooldown()
    {
        // O jogador não pode parryar por um tempo
        parryOnCooldown = true;

        // Espera o tempo do cooldown
        yield return new WaitForSeconds(parryCooldown);

        pState.SetParring(false);
        warningEnemyAttack.color = Color.clear;
        // Reseta o cooldown
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
