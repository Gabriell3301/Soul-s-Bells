using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia os estados do jogador, controlando flags booleanas que indicam diferentes condições e ações.
/// </summary>
public class PlayerStateList : MonoBehaviour
{
    public static PlayerStateList Instance { get; private set; } // Instância singleton

    // Estados de movimento
    [SerializeField] private bool isWalking; // Indica se o jogador está andando
    [SerializeField] private bool isJumping; // Indica se o jogador está pulando
    [SerializeField] private bool isFalling; // Indica se o jogador está caindo
    [SerializeField] private bool isDashing; // Indica se o jogador está dando dash
    [SerializeField] private bool isDoubleJumping; // Indica se o jogador está realizando um double jump
    [SerializeField] private bool isWallSliding; // Indica se o jogador está deslizando na parede
    [SerializeField] private bool isWallJumping; // Indica se o jogador está pulando na parede

    // Estados de combate
    [SerializeField] private bool isAttacking; // Indica se o jogador está atacando
    [SerializeField] private bool isCharging; // Indica se o jogador está carregando um ataque
    [SerializeField] private bool isBlocking; // Indica se o jogador está bloqueando
    [SerializeField] private bool isParrying; // Indica se o jogador está realizando um parry
    [SerializeField] private bool isHit; // Indica se o jogador foi atingido
    [SerializeField] private bool isInvincible; // Indica se o jogador está invulnerável
    [SerializeField] private bool isDead; // Indica se o jogador está morto

    // Estados de interação
    [SerializeField] private bool isInteracting; // Indica se o jogador está interagindo com algo
    [SerializeField] private bool isClimbing; // Indica se o jogador está escalando
    [SerializeField] private bool isSwimming; // Indica se o jogador está nadando
    [SerializeField] private bool isGrounded; // Indica se o jogador está no chão

    // Direção do jogador
    private Vector2 directionLook; // Direção para onde o jogador está olhando

    /// <summary>
    /// Inicializa o singleton e os estados
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStates();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Inicializa todos os estados como false
    /// </summary>
    private void InitializeStates()
    {
        // Estados de movimento
        isWalking = false;
        isJumping = false;
        isFalling = false;
        isDashing = false;
        isDoubleJumping = false;
        isWallSliding = false;
        isWallJumping = false;

        // Estados de combate
        isAttacking = false;
        isCharging = false;
        isBlocking = false;
        isParrying = false;
        isHit = false;
        isInvincible = false;
        isDead = false;

        // Estados de interação
        isInteracting = false;
        isClimbing = false;
        isSwimming = false;
        isGrounded = false;
    }

    /// <summary>
    /// Atualiza o estado de andar do jogador
    /// </summary>
    public void SetWalking(bool value)
    {
        isWalking = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerMovementStateChanged("walking", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de pulo do jogador
    /// </summary>
    public void SetJumping(bool value)
    {
        isJumping = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerMovementStateChanged("jumping", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de queda do jogador
    /// </summary>
    public void SetFalling(bool value)
    {
        isFalling = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerMovementStateChanged("falling", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de dash do jogador
    /// </summary>
    public void SetDashing(bool value)
    {
        isDashing = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerMovementStateChanged("dashing", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de double jump do jogador
    /// </summary>
    public void SetDoubleJumping(bool value)
    {
        isDoubleJumping = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerMovementStateChanged("doubleJumping", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de wall slide do jogador
    /// </summary>
    public void SetWallSliding(bool value)
    {
        isWallSliding = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerMovementStateChanged("wallSliding", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de wall jump do jogador
    /// </summary>
    public void SetWallJumping(bool value)
    {
        isWallJumping = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerMovementStateChanged("wallJumping", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de ataque do jogador
    /// </summary>
    public void SetAttacking(bool value)
    {
        isAttacking = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerCombatStateChanged("attacking", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de carregamento do jogador
    /// </summary>
    public void SetCharging(bool value)
    {
        isCharging = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerCombatStateChanged("charging", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de bloqueio do jogador
    /// </summary>
    public void SetBlocking(bool value)
    {
        isBlocking = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerCombatStateChanged("blocking", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de parry do jogador
    /// </summary>
    public void SetParrying(bool value)
    {
        isParrying = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerCombatStateChanged("parrying", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de hit do jogador
    /// </summary>
    public void SetHit(bool value)
    {
        isHit = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerCombatStateChanged("hit", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de invulnerabilidade do jogador
    /// </summary>
    public void SetInvincible(bool value)
    {
        isInvincible = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerCombatStateChanged("invincible", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de morte do jogador
    /// </summary>
    public void SetDead(bool value)
    {
        isDead = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerCombatStateChanged("dead", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de interação do jogador
    /// </summary>
    public void SetInteracting(bool value)
    {
        isInteracting = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerInteractionStateChanged("interacting", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de escalada do jogador
    /// </summary>
    public void SetClimbing(bool value)
    {
        isClimbing = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerInteractionStateChanged("climbing", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de natação do jogador
    /// </summary>
    public void SetSwimming(bool value)
    {
        isSwimming = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerInteractionStateChanged("swimming", true);
            }
        }
    }

    /// <summary>
    /// Atualiza o estado de chão do jogador
    /// </summary>
    public void SetGrounded(bool value)
    {
        isGrounded = value;
        if (EventManager.Instance != null)
        {
            if (value)
            {
                EventManager.Instance.TriggerPlayerInteractionStateChanged("grounded", true);
            }
        }
    }

    /// <summary>
    /// Atualiza a direção para onde o jogador está olhando
    /// </summary>
    public void SetDirectionLook(Vector2 direction)
    {
        directionLook = direction;
    }

    // Getters para os estados
    public bool IsWalking() => isWalking;
    public bool IsJumping() => isJumping;
    public bool IsFalling() => isFalling;
    public bool IsDashing() => isDashing;
    public bool IsDoubleJumping() => isDoubleJumping;
    public bool IsWallSliding() => isWallSliding;
    public bool IsWallJumping() => isWallJumping;
    public bool IsAttacking() => isAttacking;
    public bool IsCharging() => isCharging;
    public bool IsBlocking() => isBlocking;
    public bool IsParrying() => isParrying;
    public bool IsHit() => isHit;
    public bool IsInvincible() => isInvincible;
    public bool IsDead() => isDead;
    public bool IsInteracting() => isInteracting;
    public bool IsClimbing() => isClimbing;
    public bool IsSwimming() => isSwimming;
    public bool IsGrounded() => isGrounded;
    public Vector2 GetDirectionLook() => directionLook;
} 