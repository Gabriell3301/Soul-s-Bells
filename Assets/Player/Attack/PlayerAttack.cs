using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 1; // Dano do ataque
    [SerializeField] private Transform sideAttackPoint; // Referência ao ponto de ataque lateral
    [SerializeField] private Transform upAttackPoint; // Referência ao ponto de ataque de cima
    [SerializeField] private Transform downAttackPoint; // Referência ao ponto de ataque de baixo
    [SerializeField] private Vector2 attackRange = new(3f, 3.79999995f); // Tamanho da área de ataque
    [SerializeField] private Vector2 attackRangeUpAndDown = new(2.17499995f, 3.79999995f); // Tamanho da área de ataque
    [SerializeField] private LayerMask enemyLayers; // Camadas que representam os inimigos

    private Animator animator; // Referência ao Animator do jogador
    private float moveY;
    private PlayerStateList pStates;

    private void Start()
    {
        animator = GetComponent<Animator>();
        pStates = GetComponent<PlayerStateList>();
    }

    private void Update()
    {
        // Obtém a direção de ataque com base no eixo vertical
        GetAxis();
        HandleAttack();
    }

    // Detecta a direção de ataque com base no eixo vertical do jogador
    private void GetAxis()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveY = 1; // Mover para cima
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveY = -1; // Mover para baixo
        }
        else
        {
            moveY = 0; // Sem movimento vertical
        }
    }

    // Gerencia o ataque, dependendo da entrada do jogador
    private void HandleAttack()
    {
        
        if (Input.GetKeyDown(KeyCode.Z) && pStates.attacking == false && pStates.isDashing == false)
        {
            StartCoroutine(AttackRoutine());
        }
    }
    private IEnumerator AttackRoutine()
    {
        pStates.SetAttacking(true);
        Debug.Log("Ataque começou");

        // Realiza o ataque
        PerformAttack();

        // Aguardar 0.5 segundos antes de permitir um novo ataque
        yield return new WaitForSeconds(0.2f);

        pStates.SetAttacking(false);
        animator.SetBool("isAttacking", false);
    }
    // Realiza o ataque com base na direção do jogador (cima, baixo ou lateral)
    private void PerformAttack()
    {

        // Determina o ponto de ataque e a animação correta com base na direção
        Transform attackPoint = sideAttackPoint; // Ataque lateral por padrão
        if (moveY > 0) // Ataque para cima
        {
            attackPoint = upAttackPoint;
            attackRange = attackRangeUpAndDown;
            Debug.Log("Ataque para cima");
        }
        else if (moveY < 0) // Ataque para baixo
        {
            attackPoint = downAttackPoint;
            attackRange = attackRangeUpAndDown;
            Debug.Log("Ataque para baixo");
        }
        else
        {
            animator.SetBool("isAttacking", true);
        }

        // Detecta inimigos na área de ataque
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, attackRange, 0, enemyLayers);

        // Aplica dano aos inimigos atingidos
        DamageEnemies(hitEnemies);
    }

    // Aplica dano aos inimigos detectados
    private void DamageEnemies(Collider2D[] enemies)
    {
        foreach (Collider2D enemyCollider in enemies)
        {
            // Evita destruir o próprio jogador
            if (!enemyCollider.CompareTag("Player"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null) // Garante que o objeto tem o componente Enemy
                {
                    enemy.TakeDamage(attackDamage);
                    // Deixa o inimigo gerenciar sua destruição (morte) no script dele
                }
            }
        }
    }

    // Desenha as áreas de ataque no editor
    private void OnDrawGizmosSelected()
    {
        // Verifica se os pontos de ataque estão atribuídos
        if (sideAttackPoint != null && upAttackPoint != null && downAttackPoint != null)
        {
            Gizmos.color = Color.red;

            // Desenha os cubos representando as áreas de ataque no editor
            Gizmos.DrawCube(sideAttackPoint.position, attackRange);
            Gizmos.DrawCube(upAttackPoint.position, attackRangeUpAndDown);
            Gizmos.DrawCube(downAttackPoint.position, attackRangeUpAndDown);
        }
    }
}
