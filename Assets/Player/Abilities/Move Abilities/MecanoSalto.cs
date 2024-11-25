using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecanoSalto : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject invisiblePlatformPrefab; // Prefab do chão invisível
    [SerializeField] private float maxChargeTime = 2f; // Tempo máximo de carregamento
    [SerializeField] private float chargeRate = 10f; // Taxa de carregamento de força
    [SerializeField] private float maxJumpForce = 20f; // Força máxima do pulo
    [SerializeField] private Transform pointLocal;

    private float originalGravity;
    private Transform playerTransform;
    private float currentChargeTime = 0f; // Tempo atual de carregamento
    private bool isCharging = false; // Está carregando o pulo?
    private GameObject currentPlatform; // Referência ao chão invisível criado
    private Rigidbody2D rb;
    private PlayerStateList pState;

    private void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        playerTransform = transform; // Pega a posição do jogador
        originalGravity = rb.gravityScale;
    }

    private void Update()
    {
        // Início do carregamento
        if (Input.GetKeyDown(KeyCode.Space) && pState.jumping)
        {
            StartCharging();
        }

        // Carregamento contínuo
        if (Input.GetKey(KeyCode.Space) && isCharging)
        {
            ChargeJump();
        }

        // Liberar o pulo
        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            PerformJump();
        }
    }

    private void StartCharging()
    {
        pState.SetCharging(true);
        isCharging = true;
        currentChargeTime = 0f; // Reseta o tempo de carregamento

        // Cria o chão invisível
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        currentPlatform = Instantiate(invisiblePlatformPrefab, pointLocal.position, pointLocal.rotation);
        //Problema com o salto duplo com charge, provavelmente incompatibilidade com o jump padrão
        
    }

    private void ChargeJump()
    {
        currentChargeTime += Time.deltaTime * chargeRate; // Incrementa o carregamento
        currentChargeTime = Mathf.Clamp(currentChargeTime, 0, maxChargeTime); // Limita o carregamento ao tempo máximo
    }

    private void PerformJump()
    {
        isCharging = false;
        pState.SetCharging(false);
        // Calcula a força do pulo com base no carregamento
        float jumpForce = Mathf.Lerp(0, maxJumpForce, currentChargeTime / maxChargeTime);
        rb.gravityScale = originalGravity;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Aplica a força no eixo Y

        // Remove o chão invisível após o pulo
        if (currentPlatform != null)
        {
            Destroy(currentPlatform);
        }
    }
}