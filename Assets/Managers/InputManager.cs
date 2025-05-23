using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gerencia todas as entradas do jogador e eventos relacionados.
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; } // Instância singleton

    [Header("Eventos de Movimento")]
    public UnityEvent<float> OnHorizontalMove; // Evento de movimento horizontal
    public UnityEvent OnJumpPressed; // Evento de pulo
    public UnityEvent OnJumpReleased; // Evento de soltar pulo
    public UnityEvent OnDashPressed; // Evento de dash

    [Header("Eventos de Ataque")]
    public UnityEvent OnAttackPressed; // Evento de ataque
    public UnityEvent OnAttackReleased; // Evento de soltar ataque
    public UnityEvent OnSpecialAttackPressed; // Evento de ataque especial

    [Header("Eventos de Interação")]
    public UnityEvent OnInteractPressed; // Evento de interação
    public UnityEvent OnPausePressed; // Evento de pausa

    [Header("Configurações")]
    [SerializeField] private float deadzone = 0.1f; // Zona morta do input
    [SerializeField] private bool useNewInputSystem = true; // Usar novo sistema de input

    private float horizontalInput; // Input horizontal atual
    private bool isJumpPressed; // Estado do pulo
    private bool isAttackPressed; // Estado do ataque

    /// <summary>
    /// Inicializa o singleton
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Atualiza os inputs a cada frame
    /// </summary>
    private void Update()
    {
        if (useNewInputSystem)
        {
            HandleNewInputSystem();
        }
        else
        {
            HandleOldInputSystem();
        }
    }

    /// <summary>
    /// Processa inputs usando o novo sistema de input
    /// </summary>
    private void HandleNewInputSystem()
    {
        // Movimento horizontal
        float newHorizontalInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(newHorizontalInput) > deadzone)
        {
            horizontalInput = newHorizontalInput;
            OnHorizontalMove?.Invoke(horizontalInput);
        }
        else
        {
            horizontalInput = 0f;
            OnHorizontalMove?.Invoke(0f);
        }

        // Pulo
        if (Input.GetButtonDown("Jump"))
        {
            isJumpPressed = true;
            OnJumpPressed?.Invoke();
        }
        if (Input.GetButtonUp("Jump"))
        {
            isJumpPressed = false;
            OnJumpReleased?.Invoke();
        }

        // Dash
        if (Input.GetButtonDown("Dash"))
        {
            OnDashPressed?.Invoke();
        }

        // Ataque
        if (Input.GetButtonDown("Fire1"))
        {
            isAttackPressed = true;
            OnAttackPressed?.Invoke();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            isAttackPressed = false;
            OnAttackReleased?.Invoke();
        }

        // Ataque especial
        if (Input.GetButtonDown("Fire2"))
        {
            OnSpecialAttackPressed?.Invoke();
        }

        // Interação
        if (Input.GetButtonDown("Interact"))
        {
            OnInteractPressed?.Invoke();
        }

        // Pausa
        if (Input.GetButtonDown("Pause"))
        {
            OnPausePressed?.Invoke();
        }
    }

    /// <summary>
    /// Processa inputs usando o sistema de input antigo
    /// </summary>
    private void HandleOldInputSystem()
    {
        // Movimento horizontal
        float newHorizontalInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(newHorizontalInput) > deadzone)
        {
            horizontalInput = newHorizontalInput;
            OnHorizontalMove?.Invoke(horizontalInput);
        }
        else
        {
            horizontalInput = 0f;
            OnHorizontalMove?.Invoke(0f);
        }

        // Pulo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpPressed = true;
            OnJumpPressed?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumpPressed = false;
            OnJumpReleased?.Invoke();
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnDashPressed?.Invoke();
        }

        // Ataque
        if (Input.GetMouseButtonDown(0))
        {
            isAttackPressed = true;
            OnAttackPressed?.Invoke();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isAttackPressed = false;
            OnAttackReleased?.Invoke();
        }

        // Ataque especial
        if (Input.GetMouseButtonDown(1))
        {
            OnSpecialAttackPressed?.Invoke();
        }

        // Interação
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteractPressed?.Invoke();
        }

        // Pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPausePressed?.Invoke();
        }
    }

    /// <summary>
    /// Retorna o input horizontal atual
    /// </summary>
    public float GetHorizontalInput()
    {
        return horizontalInput;
    }

    /// <summary>
    /// Verifica se o pulo está pressionado
    /// </summary>
    public bool IsJumpPressed()
    {
        return isJumpPressed;
    }

    /// <summary>
    /// Verifica se o ataque está pressionado
    /// </summary>
    public bool IsAttackPressed()
    {
        return isAttackPressed;
    }
} 