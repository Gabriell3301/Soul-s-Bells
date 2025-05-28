using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 1; // Dano do ataque
    [SerializeField] private Transform sideAttackPoint, upAttackPoint, downAttackPoint; // Referência aos pontos de ataque
    [SerializeField] private Vector2 attackRange = new(3f, 3.8f); // Tamanho da área de ataque lateral
    [SerializeField] private Vector2 attackRangeUpAndDown = new(2.17f, 3.8f); // Tamanho da área de ataque vertical
    [SerializeField] private LayerMask enemyLayers; // Camadas que representam os inimigos

    [Header("Animator")]
    private Animator animator;
    private PlayerStateList pStates;
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Fire.performed += HandleAttack;
    }

    private void OnEnable() => playerControls.Enable();
    private void OnDisable() => playerControls.Disable();

    private void Start()
    {
        animator = GetComponent<Animator>();
        pStates = GetComponent<PlayerStateList>();
    }

    // Gerencia o ataque, dependendo da entrada do jogador
    private void HandleAttack(InputAction.CallbackContext context)
    {
        if (!pStates.IsAttacking() && !pStates.IsDashing())
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        pStates.SetAttacking(true);
        Debug.Log("Ataque começou");

        PerformAttack();

        yield return new WaitForSeconds(0.2f);

        pStates.SetAttacking(false);
        animator.SetBool("isAttacking", false);
    }

    // Realiza o ataque com base na direção do jogador (cima, baixo ou lateral)
    private void PerformAttack()
    {
        Transform attackPoint;
        Vector2 currentAttackRange = attackRange;

        // Define a direção do ataque
        if (pStates.GetDirectionLook().y > 0.3f) // Ataque para cima
        {
            attackPoint = upAttackPoint;
            currentAttackRange = attackRangeUpAndDown;
        }
        else if (pStates.GetDirectionLook().y < -0.3f) // Ataque para baixo
        {
            attackPoint = downAttackPoint;
            currentAttackRange = attackRangeUpAndDown;
        }
        else // Ataque lateral
        {
            attackPoint = sideAttackPoint;
            animator.SetBool("isAttacking", true);
        }

        // Detecta inimigos na área de ataque
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, currentAttackRange, 0, enemyLayers);

        // Aplica dano aos inimigos atingidos
        DamageEnemies(hitEnemies);
    }

    // Aplica dano e knockback aos inimigos detectados
    private void DamageEnemies(Collider2D[] enemies)
    {
        foreach (Collider2D enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            Rigidbody2D enemyRb = enemyCollider.GetComponent<Rigidbody2D>();

            if (enemy != null && enemyRb != null && !enemy.die)
            {
                enemy.TakeDamage(attackDamage, DamageType.Slashing, transform.position, 5f);
            }
        }
    }

    // Desenha as áreas de ataque no editor
    private void OnDrawGizmosSelected()
    {
        if (sideAttackPoint == null || upAttackPoint == null || downAttackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawCube(sideAttackPoint.position, attackRange);
        Gizmos.DrawCube(upAttackPoint.position, attackRangeUpAndDown);
        Gizmos.DrawCube(downAttackPoint.position, attackRangeUpAndDown);
    }
}
