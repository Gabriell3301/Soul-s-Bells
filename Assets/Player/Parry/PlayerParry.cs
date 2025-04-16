using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry Settings")]
    [SerializeField] private float parryActiveFrames = 0.1f;    
    [SerializeField] private float parryStartupFrames = 0.05f;  
    [SerializeField] private float parryCooldown = 0.5f;        // Aumentado para 0.5s
    [SerializeField] private float invincibilityDuration = 0.5f;
    [SerializeField] private float failedParryCooldown = 1f;    // Cooldown maior se errar o parry
    [SerializeField] private float perfectParryWindow = 0.2f;   // Janela de tempo para parry perfeito

    [Header("Circle Settings")]
    [SerializeField] private float detectionRadius = 2f;        
    [SerializeField] private float parryRadius = 1.5f;         
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color parryColor = Color.green;   
    [SerializeField] private Color failedParryColor = Color.red;

    [Header("Components")]
    [SerializeField] private PlayerParryFeedback feedback;
    [SerializeField] private PlayerStateList playerState;
    [SerializeField] private CircleCollider2D detectionCollider;
    [SerializeField] private CircleCollider2D parryCollider;

    [Header("Parry State")]
    private bool isInStartup = false;      
    private bool isWarning = false;        
    private Transform currentThreat;       
    private bool canParry = true;         
    private float lastParryTime = -999f;   // Controle do timing do último parry
    private bool parrySuccess = false;     // Se o parry atual foi bem sucedido

    // Componentes
    private PlayerControls controls;        

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Parry.performed += OnParryInput;
        SetupColliders();
    }

    private void SetupColliders()
    {
        if (detectionCollider == null)
        {
            detectionCollider = gameObject.AddComponent<CircleCollider2D>();
            detectionCollider.isTrigger = true;
            detectionCollider.radius = detectionRadius;
        }

        if (parryCollider == null)
        {
            parryCollider = gameObject.AddComponent<CircleCollider2D>();
            parryCollider.isTrigger = true;
            parryCollider.radius = parryRadius;
            parryCollider.enabled = false;
        }
    }

    private void Start()
    {
        if (feedback == null)
        {
            feedback = GetComponent<PlayerParryFeedback>();
        }

        if (playerState == null)
        {
            playerState = GetComponentInParent<PlayerStateList>();
        }

        if (playerState == null || feedback == null)
        {
            Debug.LogError("PlayerParry: Componentes necessários não encontrados!");
        }
    }

    private void Update()
    {
        UpdateParryState();
        UpdateWarningVisibility();
    }

    private void UpdateWarningVisibility()
    {
        if (isWarning && currentThreat != null)
        {
            float distance = Vector2.Distance(transform.position, currentThreat.position);
            bool isInParryRange = distance <= parryRadius;

            if (isInParryRange)
            {
                feedback.UpdateWarningFill(1f);
            }
            else
            {
                feedback.UpdateWarningFill(0f);
            }
        }
        else
        {
            feedback.UpdateWarningFill(0f);
            currentThreat = null;
        }
    }

    private void UpdateParryState()
    {
        if (isInStartup)
        {
            CancelInvoke();
            Invoke(nameof(ActivateParry), parryStartupFrames);
            Invoke(nameof(DeactivateParry), parryStartupFrames + parryActiveFrames);
            
            // Se não acertar o parry, aplica o cooldown maior
            if (!parrySuccess)
            {
                Invoke(nameof(ResetParry), parryStartupFrames + parryActiveFrames + failedParryCooldown);
                feedback.ShowParryFailed(); // Feedback visual do erro
            }
            else
            {
                Invoke(nameof(ResetParry), parryStartupFrames + parryActiveFrames + parryCooldown);
            }
            
            isInStartup = false;
        }
    }

    private void OnParryInput(InputAction.CallbackContext context)
    {
        // Verifica se pode usar o parry e se passou o cooldown
        if (canParry && !playerState.parring && !playerState.isInvulnerable && currentThreat != null)
        {
            float timeSinceLastParry = Time.time - lastParryTime;
            if (timeSinceLastParry >= (parrySuccess ? parryCooldown : failedParryCooldown))
            {
                StartParry();
                lastParryTime = Time.time;
            }
        }
    }

    private void StartParry()
    {
        isInStartup = true;
        canParry = false;
        parrySuccess = false; // Reseta o status do parry
        playerState.SetParring(true);
        feedback.ShowParryActive();
    }

    private void ActivateParry()
    {
        parryCollider.enabled = true;
        playerState.SetInvulnerable(true);
        feedback.ShowParryActive();
    }

    private void DeactivateParry()
    {
        parryCollider.enabled = false;
        playerState.SetInvulnerable(false);
        playerState.SetParring(false);
        feedback.HideParryWindow();
    }

    private void ResetParry()
    {
        canParry = true;
        playerState.SetParring(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            if (other.IsTouching(detectionCollider))
            {
                isWarning = true;
                currentThreat = other.transform;
            }

            if (playerState.parring && other.IsTouching(parryCollider))
            {
                HandleSuccessfulParry(other);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAttack") && other.IsTouching(detectionCollider))
        {
            isWarning = false;
            currentThreat = null;
        }
    }

    private void HandleSuccessfulParry(Collider2D attackCollider)
    {
        parrySuccess = true;
        feedback.ShowSuccess();
        StartCoroutine(InvincibilityRoutine());
        Destroy(attackCollider.gameObject);
    }

    private IEnumerator InvincibilityRoutine()
    {
        playerState.SetInvulnerable(true);
        yield return new WaitForSeconds(invincibilityDuration);
        playerState.SetInvulnerable(false);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void OnDrawGizmos()
    {
        // Desenha área de detecção
        Gizmos.color = Color.yellow * 0.5f;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Desenha área de parry com cor baseada no estado
        Color parryAreaColor = playerState != null ? 
            (playerState.parring ? 
                (parrySuccess ? parryColor : failedParryColor) 
                : Color.blue) 
            : Color.blue;
        
        Gizmos.color = parryAreaColor;
        Gizmos.DrawWireSphere(transform.position, parryRadius);
    }
}