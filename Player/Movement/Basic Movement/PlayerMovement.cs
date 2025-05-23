using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla todo o movimento e ações do jogador, incluindo movimento básico, pulo e wall slide.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    private Vector3 scale;

    [Header("Player Move")]
    [SerializeField] private float playerSpeed;
    [NonSerialized] public float direction;
    [NonSerialized] public Vector2 moveX;
    private InputAction move;
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Vertical Move")]
    private InputAction jump;
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    public static PlayerMovement Instance;
    private PlayerControls playerControls;
    private PlayerStateList pstates;
    private Animator animator;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Se já existe uma instância, destrua o novo objeto
        }
        else
        {
            Instance = this;  // Define esta instância como a instância global
        }
        playerControls = new PlayerControls();
        playerControls.Player.Jump.performed += Jump;
    }
    private void OnEnable()
    {
        playerControls.Enable();
        move = playerControls.Player.Move;
        move.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
        move = playerControls.Player.Move;
        move.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        scale = transform.localScale;
        pstates = GetComponent<PlayerStateList>();
        direction = 1;
        StartCoroutine(ChangeStates());
    }
    /// <summary>
    /// Gerencia as animações do jogador
    /// </summary>
    private void HandleAnimation()
    {
        // Animação de andar
        if (direction != 0 && moveX != Vector2.zero)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        // Animação de pulo
        if (!Grounded() && rb.velocity.y > 0)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

        // Animação de queda
        if (!Grounded() && rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }
    }

    /// <summary>
    /// Gerencia os estados do jogador
    /// </summary>
    private IEnumerator ChangeStates()
    {
        while (true)
        {
            UpdateMovementState();
            HandleAnimation();
            yield return null;
        }
    }

    private void UpdateMovementState()
    {
        if (rb.velocity.x != 0)
        {
            pstates.SetWalking(true);
        }
        else
        {
            pstates.SetWalking(false);
        }

        if (rb.velocity.y > 0)
        {
            pstates.SetJumping(true);
            pstates.SetFalling(false);
        }
        else if (rb.velocity.y < 0)
        {
            pstates.SetJumping(false);
            pstates.SetFalling(true);
        }
        else
        {
            pstates.SetFalling(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (pstates.IsDashing())
        {
            return;
        }
        if(!pstates.IsCharging())
        {
            GetValueForMove();
            MoveX();
            Flip();
        }
    }
    /// <summary>
    /// Pega o valor do input do jogador
    /// </summary>
    private void GetValueForMove()
    {
        moveX = move.ReadValue<Vector2>();
    }
    /// <summary>
    /// Move o jogador de acordo com o input
    /// </summary>
    private void MoveX()
    {
        rb.velocity = new Vector2(moveX.x * playerSpeed, rb.velocity.y);
    }
    /// <summary>
    /// Vira o Sprite do jogador
    /// </summary>
    private void Flip()
    {
        if ((moveX.x > 0 && direction < 0) || (moveX.x < 0 && direction > 0))
        {
            direction = moveX.x > 0 ? 1 : -1;
            transform.localScale = new Vector3(scale.x * direction, scale.y, scale.z);
        }
    }
    /// <summary>
    /// Executa o pulo, aplica gravidade
    /// </summary>
    /// <param name="context"></param>
    private void Jump(InputAction.CallbackContext context)
    {
        // Pular
        if (Grounded())
        {
            // Aplica a força de pulo
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        // Verifica se a tecla de pulo foi solta antes de atingir o pico do pulo
        if (context.canceled && rb.velocity.y > 0)
        {
            // Aplica gravidade extra imediatamente ao soltar a tecla
            rb.velocity = new Vector2(rb.velocity.x, 0); // Zera a velocidade vertical
        }
        // Aplicar gravidade extra ao cair
        if (rb.velocity.y < 0)
        {
            rb.AddForce((fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up, ForceMode2D.Force);
        }
        // Aplicar gravidade extra para pequenos pulos
        else if (rb.velocity.y > 0 && context.performed && !pstates.IsDashing())
        {
            //Aqui está o erro, porque quando clica no spaço, ele muda a velocidade e freia o personagem. (Ou não)
            rb.AddForce((lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up, ForceMode2D.Force);
        }

    }
    /// <summary>
    /// Verifica se o jogador está no chão
    /// </summary>
    /// <returns></returns>
    private bool Grounded()
    {
        // Distância do Raycast (pequena o suficiente para detectar o chão próximo)
        float rayLength = 0.1f;

        // Posicionamentos dos Raycasts (ajuste conforme o tamanho do seu personagem)
        Vector3 leftRayPosition = groundCheck.position + new Vector3(-1, 0, 0);
        Vector3 rightRayPosition = groundCheck.position + new Vector3(1, 0, 0);

        // Lançar os Raycasts (central, esquerdo e direito)
        bool centralRay = Physics2D.Raycast(groundCheck.position, Vector2.down, rayLength, whatIsGround);
        bool leftRay = Physics2D.Raycast(leftRayPosition, Vector2.down, rayLength, whatIsGround);
        bool rightRay = Physics2D.Raycast(rightRayPosition, Vector2.down, rayLength, whatIsGround);


        pstates.SetGrounded(centralRay || leftRay || rightRay);
        // Se qualquer um dos raios detectar o chão, retorna true
        return centralRay || leftRay || rightRay;
    }
}