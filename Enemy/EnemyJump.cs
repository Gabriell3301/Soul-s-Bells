using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJump : MonoBehaviour
{
    public Transform player; // Referência ao player
    public float detectionHeight = 2f; // Diferença mínima de altura para considerar um pulo
    public LayerMask groundLayer; // Camada do chão
    public float jumpDelay = 0.5f; // Tempo de espera antes de pular

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isPreparingJump = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckGround();

        if (ShouldJump() && !isPreparingJump)
        {
            StartCoroutine(PrepareJump()); // Inicia o tempo de "pensamento" antes de pular
        }
    }

    private void CheckGround()
    {
        // Raycast abaixo do inimigo para verificar se está no chão
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, groundLayer);
        isGrounded = hit.collider != null;
    }

    private bool ShouldJump()
    {
        return isGrounded && player.position.y > transform.position.y + detectionHeight;
    }

    private IEnumerator PrepareJump()
    {
        isPreparingJump = true;
        yield return new WaitForSeconds(jumpDelay); // Aguarda 0.5 segundos antes de pular

        float jumpForce = CalculateJumpForce();
        Jump(jumpForce);

        isPreparingJump = false;
    }

    private float CalculateJumpForce()
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float heightDifference = player.position.y - transform.position.y;
        return Mathf.Sqrt(2 * gravity * heightDifference); // Fórmula do pulo
    }

    private void Jump(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, force);
    }
}
