using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    private Vector3 scale;

    [Header("Horizontal Move")]
    [SerializeField] private float playerSpeed;
    [NonSerialized] public float direction;
    private float moveX;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Vertical Move")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    public static PlayerMovement Instance;

    private PlayerStateList pstates;
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
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scale = transform.localScale;
        pstates = GetComponent<PlayerStateList>();
        direction = 1;
        StartCoroutine(ChangeStates());
    }
    private IEnumerator ChangeStates()
    {
        while (true)
        {
            //Set State Walking
            if (moveX != 0)
            {
                pstates.SetWalking(true);
            }
            else
            {
                pstates.SetWalking(false);
            }

            //Set State Jumping
            if (!Grounded())
            {
                pstates.SetJumping(true);
            }
            else
            {
                pstates.SetJumping(false);
            }
            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (pstates.isDashing)
        {
            return;
        }
        if(!pstates.isCharging)
        {
            GetAxis();
            MoveX();
            Flip();
            Jump();
        }
    }
    private void GetAxis()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveX = 1; // Mover para Direita
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveX = -1; // Mover para Esquerda
        }
        else
        {
            moveX = 0; // Sem movimento Horizontal
        }
    }
    private void MoveX()
    {
        rb.velocity = new Vector2(moveX * playerSpeed, rb.velocity.y);
    }
    private void Flip()
    {
        if ((moveX > 0 && direction < 0) || (moveX < 0 && direction > 0))
        {
            direction = moveX > 0 ? 1 : -1;
            transform.localScale = new Vector3(scale.x * direction, scale.y, scale.z);
        }
    }
    private void Jump()
    {
        // Pular
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && Grounded())
        {
            // Aplica a força de pulo
            Debug.Log("Tentando Pular");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        // Verifica se a tecla de pulo foi solta antes de atingir o pico do pulo
        if ((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow)) && rb.velocity.y > 0)
        {
            // Aplica gravidade extra imediatamente ao soltar a tecla
            Debug.Log("Aplicando pulo reduzido");
            rb.velocity = new Vector2(rb.velocity.x, 0); // Zera a velocidade vertical
        }
        // Aplicar gravidade extra ao cair
        if (rb.velocity.y < 0)
        {
            rb.AddForce((fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up, ForceMode2D.Force);
        }
        // Aplicar gravidade extra para pequenos pulos
        else if (rb.velocity.y > 0 && (!Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && !pstates.isDashing)
        {
            Debug.Log("Aplicando gravidade para pequenos pulos");
            //Aqui está o erro, porque quando clica no spaço, ele muda a velocidade e freia o personagem. (Ou não)
            rb.AddForce((lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up, ForceMode2D.Force);
        }

    }
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
