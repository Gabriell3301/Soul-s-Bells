using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateList : MonoBehaviour
{

    public bool walking;
    public bool jumping;
    public bool isCharging;
    public bool attacking;
    public bool parring;
    public bool Grounded;
    public bool isDashing;
    public bool isInvulnerable;
    public Vector2 directionLook;

    private PlayerControls playerContrlos;

    private void Awake()
    {
        playerContrlos = new PlayerControls();
    }
    public void SetInvulnerable(bool isInvulnarable)
    {
        this.isInvulnerable = isInvulnarable;
    }
    public void SetCharging(bool ischarging)
    {
        isCharging = ischarging;
    }
    public void SetDashing(bool isDash)
    {
        isDashing = isDash;
    }
    public void SetWalking(bool isWalking)
    {
        walking = isWalking;
    }
    public void SetGrounded(bool isGrounded)
    {
        Grounded = isGrounded;
    }

    public void SetAttacking(bool isAttacking)
    {
        attacking = isAttacking;
    }

    public void SetParring(bool isParring)
    {
        parring = isParring;
    }
    public void SetJumping(bool isJumping)
    {
        jumping = isJumping;
    }
    private void OnEnable()
    {
        playerContrlos.Player.Enable();
        playerContrlos.Player.Move.performed += OnMove;
        playerContrlos.Player.Move.canceled += OnMove; // Atualiza quando o movimento é cancelado
    }

    private void OnDisable()
    {
        playerContrlos.Player.Move.performed -= OnMove;
        playerContrlos.Player.Move.canceled -= OnMove;
        playerContrlos.Player.Disable();
    }
    public void OnMove(InputAction.CallbackContext context)
    { 
        directionLook = context.ReadValue<Vector2>();
    }
}