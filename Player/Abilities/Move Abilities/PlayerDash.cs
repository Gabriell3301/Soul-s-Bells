using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.DocumentationSortingAttribute;
using static UnityEngine.Mesh;

public class PlayerDash : MonoBehaviour, IAbility
{
    private float dashSpeed;
    private float dashDuration;
    private float dashCooldown;

    private bool hasDashedInAir;
    private bool dashInCoolDown = false;
    private bool canDash = true;
    private float originalGravity;
    private TrailRenderer trailRender;
    private Rigidbody2D rb;
    private PlayerStateList pState;
    private PlayerControls playerControls;

    private DashData dashData;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Dash.performed += PerformedDash;
    }
    public void Initialize(AbilityData data)
    {
        dashData = data as DashData;
        if (dashData == null)
        {
            Debug.LogError("DashData inv�lido ao inicializar PlayerDash.");
            return;
        }
        dashSpeed = dashData.dashSpeed;
        dashDuration = dashData.dashDuration;
        dashCooldown = dashData.dashCooldown;
    }
    void Start()
    {
        trailRender = GetComponent<TrailRenderer>();
        pState = GetComponent<PlayerStateList>();
        if (!TryGetComponent<Rigidbody2D>(out rb))
        {
            Destroy(this);
        }
        originalGravity = rb.gravityScale;
    }

    private void OnEnable() => playerControls.Enable();
    private void OnDisable() => playerControls.Disable();

    void Update()
    {
        if (pState.IsGrounded())
        {
            canDash = true;
            hasDashedInAir = false;
        }
    }

    private void PerformedDash(InputAction.CallbackContext context)
    {
        if (canDash && (!hasDashedInAir || pState.IsGrounded()) && !pState.IsCharging() && !dashInCoolDown)
        {
            StartCoroutine(Dashing());
        }
    }

    private IEnumerator Dashing()
    {
        // Inicia o dash
        canDash = false;
        dashInCoolDown = true;
        // Marca que o jogador usou o dash no ar, se n�o estiver no ch�o
        if (!pState.IsGrounded())
        {
            hasDashedInAir = true;
        }

        pState.SetDashing(true);
        trailRender.emitting = true;

        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0); // Define a velocidade do dash no eixo horizontal

        yield return new WaitForSeconds(dashDuration); // Aguarda a dura��o do dash

        // Termina o dash
        trailRender.emitting = false;
        rb.gravityScale = originalGravity;
        pState.SetDashing(false);

        // Espera o cooldown antes de permitir o pr�ximo dash
        yield return new WaitForSeconds(dashCooldown);

        if (pState.IsGrounded())
        {
            dashInCoolDown = false;
            canDash = true;
        }
        else
        {
            // No ar, permite o pr�ximo dash ap�s o cooldown
            dashInCoolDown = false;
        }
    }
}