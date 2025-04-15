using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MecanoSalto : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject invisiblePlatformPrefab; // Prefab do chão invisível
    [SerializeField] private float maxChargeTime = 3f; // Tempo máximo de carregamento
    [SerializeField] private float chargeRate = 10f; // Taxa de carregamento de força
    [SerializeField] private float maxJumpForce = 20f; // Força máxima do pulo
    [SerializeField] private Transform pointLocal;

    private float originalGravity;
    private float currentChargeTime = 0f; // Tempo atual de carregamento
    private GameObject currentPlatform; // Referência ao chão invisível criado
    private Rigidbody2D rb;
    private PlayerStateList pState;
    private bool isFullyCharged = false; // Controle para evitar múltiplos disparos
    private Coroutine chargeCoroutine; // Referência para a corrotina de carregamento

    private PlayerControls playerControls; // Referência ao sistema de controle

    private void Awake()
    {
        playerControls = new PlayerControls(); // Inicializa o controle
    }

    private void OnEnable()
    {
        playerControls.Enable(); // Ativa o controle
        playerControls.Player.Jump.started += OnChargeStarted;   // Quando começa a carregar
        playerControls.Player.Jump.canceled += OnChargeReleased; // Quando solta
    }

    private void OnDisable()
    {
        playerControls.Player.Jump.started -= OnChargeStarted;
        playerControls.Player.Jump.canceled -= OnChargeReleased;
        playerControls.Disable(); // Desativa o controle
    }

    private void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    private void OnChargeStarted(InputAction.CallbackContext context)
    {
        if (pState.jumping)
        {
            StartCharging();
        }
    }

    private void OnChargeReleased(InputAction.CallbackContext context)
    {
        if (pState.isCharging)
        {
            StopCharging();
            PerformJump();
        }
    }

    private void StartCharging()
    {
        pState.SetCharging(true);
        currentChargeTime = 0f; // Reseta o tempo de carregamento
        isFullyCharged = false;

        // Cria o chão invisível e imobiliza o jogador
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        rb.isKinematic = true; // Imobiliza completamente o jogador
        currentPlatform = Instantiate(invisiblePlatformPrefab, pointLocal.position, pointLocal.rotation);

        // Inicia a corrotina de carregamento
        chargeCoroutine = StartCoroutine(ChargeJump());
    }

    private void StopCharging()
    {
        if (chargeCoroutine != null)
        {
            StopCoroutine(chargeCoroutine);
            chargeCoroutine = null;
        }
    }

    private IEnumerator ChargeJump()
    {
        Debug.Log("Carregando...");

        while (currentChargeTime < maxChargeTime)
        {
            currentChargeTime += Time.deltaTime * chargeRate; // Incrementa o carregamento
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0, maxChargeTime); // Limita ao tempo máximo

            // Disparo automático ao atingir o máximo de carregamento
            if (currentChargeTime >= maxChargeTime && !isFullyCharged)
            {
                isFullyCharged = true; // Evita múltiplos disparos
                PerformJump();
                yield break; // Sai da corrotina após disparar o pulo
            }

            yield return null; // Aguarda o próximo frame
        }
    }

    private void PerformJump()
    {
        Debug.Log("Pulo executado");
        pState.SetCharging(false);

        // Calcula a força do pulo com base no carregamento
        float jumpForce = Mathf.Lerp(0, maxJumpForce, currentChargeTime / maxChargeTime);
        Debug.Log($"Força do pulo: {jumpForce}");
        rb.gravityScale = originalGravity;
        rb.isKinematic = false; // Restaura o movimento normal do jogador

        // Aplica um impulso vertical para o pulo
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Remove o chão invisível após o pulo
        if (currentPlatform != null)
        {
            Destroy(currentPlatform);
        }
    }
}
