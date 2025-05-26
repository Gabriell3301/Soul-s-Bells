using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : BaseAbility
{
    [Header("Dash References")]
    [SerializeField] private TrailRenderer trailRender;
    private Rigidbody2D rb;
    private PlayerStateList pState;
    private PlayerControls playerControls;

    [Header("Dash Parameters")]
    private float dashSpeed;
    private float dashDuration;
    private float dashCooldown;

    private bool hasDashedInAir;
    private bool dashInCoolDown = false;
    private bool canDash = true;
    private float originalGravity;
    private void Awake()
    {
        Debug.Log("PlayerDash - Awake chamado");
        playerControls = new PlayerControls();
        
        // Verifica se a ação existe
        try
        {
            var dashAction = playerControls.Player.Dash;
            Debug.Log("Ação Dash encontrada no Input System");
            dashAction.performed += ctx => 
            {
                Debug.Log("Input de dash detectado! (Tecla pressionada)");
                Activate();
            };
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao configurar input: {e.Message}");
        }
    }
    protected override void OnInitialize()
    {
        Debug.Log("OnInitialize - Iniciando dash ability");
        
        // Get required components
        rb = GetComponent<Rigidbody2D>();
        Debug.Log(rb != null ? "Rigidbody2D encontrado" : "ERRO: Rigidbody2D não encontrado!");

        pState = GetComponent<PlayerStateList>();
        Debug.Log(pState != null ? "PlayerStateList encontrado" : "ERRO: PlayerStateList não encontrado!");
        
        if (trailRender == null)
        {
            trailRender = GetComponentInChildren<TrailRenderer>();
            Debug.Log(trailRender != null ? "TrailRenderer encontrado nos children" : "TrailRenderer não encontrado");
        }

        // Initialize input
        playerControls = new PlayerControls();
        playerControls.Player.Dash.performed += ctx => 
        {
            Debug.Log("Input de dash detectado!");
            Activate();
        };
        
        // Set dash parameters from data
        if (abilityData is DashData dashData)
        {
            dashSpeed = dashData.dashSpeed;
            dashDuration = dashData.dashDuration;
            dashCooldown = dashData.TotalCooldown;
            originalGravity = rb.gravityScale;
            
            Debug.Log($"Dados do dash carregados - Speed: {dashSpeed}, Duration: {dashDuration}, Cooldown: {dashCooldown}");
        }
        else
        {
            Debug.LogError("Dados de habilidade não são do tipo DashData ou estão faltando!");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        Debug.Log("PlayerDash habilitado");
        playerControls?.Enable();
    }

    private void OnDisable()
    {
        Debug.Log("PlayerDash desabilitado");
        playerControls?.Disable();
    }

    void Update()
    {
        if (pState.IsGrounded())
        {
            if (!canDash) Debug.Log("Resetando dash - jogador está no chão");
            canDash = true;
            hasDashedInAir = false;
        }
    }

    protected override void OnActivate()
    {
        Debug.Log("OnActivate chamado");
        if (CanDash())
        {
            Debug.Log("Condições para dash satisfeitas - iniciando dash");
            StartCoroutine(PerformDash());
        }
        else
        {
            Debug.Log("Dash não pode ser ativado agora. Razões:");
            if (!canDash) Debug.Log("- canDash é false");
            if (hasDashedInAir && !pState.IsGrounded()) Debug.Log("- Já deu dash no ar e não está no chão");
            if (dashInCoolDown) Debug.Log("- Dash em cooldown");
            if (pState.IsCharging()) Debug.Log("- Jogador está carregando");
        }
    }

    private bool CanDash()
    {
        bool canPerformDash = canDash && (!hasDashedInAir || pState.IsGrounded()) 
                           && !dashInCoolDown && !pState.IsCharging();
        
        Debug.Log($"CanDash retornando: {canPerformDash}");
        return canPerformDash;
    }

    private IEnumerator PerformDash()
    {
        Debug.Log("PerformDash iniciado");
        
        // Setup dash state
        canDash = false;
        dashInCoolDown = true;
        hasDashedInAir = !pState.IsGrounded();
        pState.SetDashing(true);
        
        Debug.Log($"Estado do dash - No ar: {!pState.IsGrounded()}, Dashing: {pState.IsDashing()}");

        if (trailRender != null)
        {
            trailRender.emitting = true;
            Debug.Log("TrailRenderer ativado");
        }

        // Store original values
        float originalGravity = rb.gravityScale;
        Vector2 originalVelocity = rb.velocity;
        Debug.Log($"Valores originais - Gravity: {originalGravity}, Velocity: {originalVelocity}");

        // Apply dash force
        rb.gravityScale = 0f;
        float dashDirection = transform.localScale.x;
        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);
        
        Debug.Log($"Dash aplicado - Direção: {dashDirection}, Nova velocidade: {rb.velocity}");

        // Wait for dash duration
        Debug.Log($"Esperando duração do dash: {dashDuration}s");
        yield return new WaitForSeconds(dashDuration);

        // Reset physics
        rb.gravityScale = originalGravity;
        rb.velocity = originalVelocity;
        Debug.Log("Física resetada para valores originais");

        // Clean up effects
        if (trailRender != null)
        {
            trailRender.emitting = false;
            Debug.Log("TrailRenderer desativado");
        }
        
        pState.SetDashing(false);
        Debug.Log("Estado de dashing desativado");

        // Cooldown period
        Debug.Log($"Iniciando cooldown: {dashCooldown}s");
        yield return new WaitForSeconds(dashCooldown);
        
        dashInCoolDown = false;
        Debug.Log("Cooldown completo - dash disponível");
        
        if (pState.IsGrounded())
        {
            canDash = true;
            Debug.Log("canDash reativado (jogador está no chão)");
        }
    }

    protected override void OnDeactivate()
    {
        Debug.Log("OnDeactivate chamado");
        // Cleanup if dash is interrupted
        if (trailRender != null)
        {
            trailRender.emitting = false;
            Debug.Log("TrailRenderer forçado a desativar");
        }
        
        if (pState != null)
        {
            pState.SetDashing(false);
            Debug.Log("Estado de dashing forçado a desativar");
        }
    }

    protected override bool CanActivateCustom()
    {
        bool result = CanDash();
        Debug.Log($"CanActivateCustom retornando: {result}");
        return result;
    }
}