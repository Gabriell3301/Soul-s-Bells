using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry Timing")]
    [SerializeField] private float parryActiveFrames = 0.1f;    
    [SerializeField] private float parryStartupFrames = 0.05f;  
    [SerializeField] private float parryCooldown = 0.5f;        
    [SerializeField] private float invincibilityDuration = 0.5f;
    [SerializeField] private float failedParryCooldown = 1f;    
    [SerializeField] private float perfectParryWindow = 0.2f;   

    [Header("Detection & Range")]
    [SerializeField] private float detectionRadius = 2f;        
    [SerializeField] private float parryRadius = 1.5f;
    [SerializeField] private float reflectionRange = 10f; // Alcance para encontrar inimigo para reflexão
    [SerializeField] private LayerMask enemyAttackLayer = -1;   // Layer dos ataques inimigos
    [SerializeField] private LayerMask enemyLayer; // Layer dos inimigos

    [Header("Visual Feedback Colors")]
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color parryColor = Color.green;   
    [SerializeField] private Color failedParryColor = Color.red;
    [SerializeField] private Color perfectParryColor = Color.cyan;

    [Header("Components")]
    [SerializeField] private PlayerParryFeedback feedback;
    [SerializeField] private PlayerStateList playerState;
    [SerializeField] private CircleCollider2D detectionCollider;
    [SerializeField] private CircleCollider2D parryCollider;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip parrySuccessSound;
    [SerializeField] private AudioClip parryFailSound;
    [SerializeField] private AudioClip perfectParrySound;

    // Estado do Parry
    private ParryState currentParryState = ParryState.Ready;
    private bool isWarning = false;        
    private Transform currentThreat;       
    private float lastParryTime = -999f;   
    private bool parrySuccess = false;     
    private bool isPerfectParry = false;
    private float threatDetectionTime = 0f; // Para calcular parry perfeito

    // Componentes e Controles
    private PlayerControls controls;
    private List<Collider2D> detectedThreats = new List<Collider2D>(); // Lista de ameaças detectadas
    private Coroutine parrySequenceCoroutine;

    // Enum para estados do parry
    private enum ParryState
    {
        Ready,      // Pronto para usar
        Startup,    // Frames de startup
        Active,     // Janela ativa do parry
        Recovery,   // Recuperação após parry
        Cooldown    // Cooldown antes de poder usar novamente
    }

    // Constantes
    private const string ENEMY_ATTACK_TAG = "EnemyAttack";
    private const float GIZMO_ALPHA = 0.3f;

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeInputSystem();
        SetupColliders();
    }

    private void Start()
    {
        InitializeComponents();
        ValidateSetup();
    }

    private void Update()
    {
        UpdateThreatDetection();
        UpdateWarningSystem();
        UpdateParryVisuals();
    }

    private void OnEnable()
    {
        controls?.Enable();
    }

    private void OnDisable()
    {
        controls?.Disable();
        StopAllCoroutines();
    }

    #endregion

    #region Initialization

    private void InitializeInputSystem()
    {
        controls = new PlayerControls();
        controls.Player.Parry.performed += OnParryInput;
    }

    private void SetupColliders()
    {
        // Collider de detecção
        if (detectionCollider == null)
        {
            detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;

        // Collider do parry
        if (parryCollider == null)
        {
            parryCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        parryCollider.isTrigger = true;
        parryCollider.radius = parryRadius;
        parryCollider.enabled = false;
    }

    private void InitializeComponents()
    {
        if (feedback == null)
            feedback = GetComponent<PlayerParryFeedback>();

        if (playerState == null)
            playerState = GetComponentInParent<PlayerStateList>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void ValidateSetup()
    {
        if (playerState == null)
        {
            Debug.LogError($"PlayerParry em {gameObject.name}: PlayerStateList não encontrado!");
        }

        if (feedback == null)
        {
            Debug.LogWarning($"PlayerParry em {gameObject.name}: PlayerParryFeedback não encontrado. Feedback visual será limitado.");
        }

        if (parryRadius > detectionRadius)
        {
            Debug.LogWarning($"PlayerParry em {gameObject.name}: Raio do parry é maior que o de detecção. Isso pode causar comportamentos inesperados.");
        }
    }

    #endregion

    #region Input Handling

    private void OnParryInput(InputAction.CallbackContext context)
    {
        if (CanExecuteParry())
        {
            StartParrySequence();
        }
    }

    private bool CanExecuteParry()
    {
        // Verifica todas as condições necessárias para executar o parry
        bool hasValidThreat = detectedThreats.Count > 0;
        bool stateAllows = !playerState.IsParrying() && !playerState.IsInvincible();
        bool cooldownReady = Time.time - lastParryTime >= GetCurrentCooldown();
        bool parryStateReady = currentParryState == ParryState.Ready;

        return hasValidThreat && stateAllows && cooldownReady && parryStateReady;
    }

    private float GetCurrentCooldown()
    {
        return parrySuccess ? parryCooldown : failedParryCooldown;
    }

    #endregion

    #region Parry Execution

    private void StartParrySequence()
    {
        // Para qualquer sequência anterior
        if (parrySequenceCoroutine != null)
        {
            StopCoroutine(parrySequenceCoroutine);
        }

        parrySequenceCoroutine = StartCoroutine(ExecuteParrySequence());
        lastParryTime = Time.time;
    }

    private IEnumerator ExecuteParrySequence()
    {
        // Reset de variáveis
        parrySuccess = false;
        isPerfectParry = false;
        
        // Startup Phase
        currentParryState = ParryState.Startup;
        playerState?.SetParrying(true);
        feedback?.ShowParryActive();
        
        yield return new WaitForSeconds(parryStartupFrames);

        // Active Phase
        currentParryState = ParryState.Active;
        parryCollider.enabled = true;
        playerState?.SetInvincible(true);
        
        yield return new WaitForSeconds(parryActiveFrames);

        // Recovery Phase
        currentParryState = ParryState.Recovery;
        parryCollider.enabled = false;
        playerState?.SetInvincible(false);
        playerState?.SetParrying(false);
        feedback?.HideParryWindow();

        // Determina o cooldown baseado no sucesso
        float cooldownTime = parrySuccess ? parryCooldown : failedParryCooldown;
        
        if (!parrySuccess)
        {
            feedback?.ShowParryFailed();
            PlayAudioClip(parryFailSound);
        }

        // Cooldown Phase
        currentParryState = ParryState.Cooldown;
        yield return new WaitForSeconds(cooldownTime);

        // Ready novamente
        currentParryState = ParryState.Ready;
        parrySequenceCoroutine = null;
    }

    #endregion

    #region Threat Detection & Warning System

    private void UpdateThreatDetection()
    {
        // Remove ameaças que não existem mais
        detectedThreats.RemoveAll(threat => threat == null);
        
        // Atualiza a ameaça atual
        currentThreat = GetClosestThreat();
        isWarning = currentThreat != null;

        // Atualiza o tempo de detecção para parry perfeito
        if (isWarning)
        {
            threatDetectionTime += Time.deltaTime;
        }
        else
        {
            threatDetectionTime = 0f;
        }
    }

    private Transform GetClosestThreat()
    {
        if (detectedThreats.Count == 0) return null;

        Transform closest = null;
        float closestDistance = float.MaxValue;

        foreach (var threat in detectedThreats)
        {
            if (threat == null) continue;

            float distance = Vector2.Distance(transform.position, threat.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = threat.transform;
            }
        }

        return closest;
    }

    private void UpdateWarningSystem()
    {
        if (isWarning && currentThreat != null)
        {
            float distance = Vector2.Distance(transform.position, currentThreat.position);
            bool isInParryRange = distance <= parryRadius;
            
            // Atualiza o fill baseado na proximidade
            float fillAmount = isInParryRange ? 1f : Mathf.InverseLerp(detectionRadius, parryRadius, distance);
            feedback?.UpdateWarningFill(fillAmount);
        }
        else
        {
            feedback?.UpdateWarningFill(0f);
        }
    }

    private void UpdateParryVisuals()
    {
        // Atualiza visuais baseado no estado atual
        switch (currentParryState)
        {
            case ParryState.Active:
                feedback?.ShowParryActive();
                break;
            case ParryState.Recovery when !parrySuccess:
                feedback?.ShowParryFailed();
                break;
        }
    }

    #endregion

    #region Collision Detection

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsValidEnemyAttack(other)) return;

        // Detecção de ameaça
        if (other.IsTouching(detectionCollider))
        {
            HandleThreatDetection(other);
        }

        // Parry bem-sucedido
        if (currentParryState == ParryState.Active && other.IsTouching(parryCollider))
        {
            HandleSuccessfulParry(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsValidEnemyAttack(other)) return;

        // Remove da lista de ameaças
        if (detectedThreats.Contains(other))
        {
            detectedThreats.Remove(other);
        }
    }

    private bool IsValidEnemyAttack(Collider2D other)
    {
        // Verifica se é um ataque inimigo válido
        bool hasCorrectTag = other.CompareTag(ENEMY_ATTACK_TAG);
        bool isInCorrectLayer = (enemyAttackLayer.value & (1 << other.gameObject.layer)) > 0;
        
        return hasCorrectTag && (enemyAttackLayer.value == -1 || isInCorrectLayer);
    }

    private void HandleThreatDetection(Collider2D threat)
    {
        if (!detectedThreats.Contains(threat))
        {
            detectedThreats.Add(threat);
            Debug.Log($"PlayerParry: Ameaça detectada - {threat.name}");
        }
    }

    private void HandleSuccessfulParry(Collider2D attackCollider)
    {
        parrySuccess = true;
        
        // Verifica se é um parry perfeito
        CheckPerfectParry();
        
        // Feedback visual e sonoro
        if (isPerfectParry)
        {
            feedback?.ShowPerfectParrySuccess();
            PlayAudioClip(perfectParrySound);
            Debug.Log("PlayerParry: PARRY PERFEITO!");
        }
        else
        {
            feedback?.ShowSuccess();
            PlayAudioClip(parrySuccessSound);
            Debug.Log("PlayerParry: Parry bem-sucedido!");
        }

        // Inicia invencibilidade
        StartCoroutine(InvincibilityRoutine());

        // CORREÇÃO DO BUG: Desabilita apenas o ataque específico, não destrói o objeto todo
        HandleAttackDisabling(attackCollider);

        // Remove da lista de ameaças
        if (detectedThreats.Contains(attackCollider))
        {
            detectedThreats.Remove(attackCollider);
        }
    }

    private void CheckPerfectParry()
    {
        // Parry perfeito se foi executado dentro da janela ideal
        isPerfectParry = threatDetectionTime <= perfectParryWindow;
    }

    private void HandleAttackDisabling(Collider2D attackCollider)
    {
        // Tenta encontrar um componente de ataque para refletir
        var attackComponent = attackCollider.GetComponent<IEnemyAttack>();
        if (attackComponent != null)
        {
            // Encontra o inimigo mais próximo para refletir o ataque
            Enemy nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                attackComponent.OnReflected(nearestEnemy);
                Debug.Log($"PlayerParry: Ataque refletido para {nearestEnemy.name}");
            }
            else
            {
                attackComponent.OnParried();
                Debug.Log("PlayerParry: Nenhum inimigo encontrado para reflexão");
            }
            return;
        }

        // Se for um projétil (Bullet), destrói ele
        var bullet = attackCollider.GetComponent<Bullet>();
        if (bullet != null)
        {
            Destroy(attackCollider.gameObject);
            Debug.Log($"PlayerParry: Projétil destruído - {attackCollider.name}");
            return;
        }

        // Se não tem interface, tenta desabilitar apenas o collider
        var attackScript = attackCollider.GetComponent<MonoBehaviour>();
        if (attackScript != null && attackScript.GetType().Name.Contains("Attack"))
        {
            attackCollider.enabled = false;
            Debug.Log($"PlayerParry: Desabilitado ataque {attackCollider.name}");
            return;
        }

        // Último recurso: desabilita o collider
        attackCollider.enabled = false;
        Debug.Log($"PlayerParry: Collider de ataque desabilitado - {attackCollider.name}");
    }

    private Enemy FindNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, reflectionRange, enemyLayer);
        Enemy nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var enemyCollider in enemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null && !enemy.die)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        return nearestEnemy;
    }

    #endregion

    #region Utility Methods

    private IEnumerator InvincibilityRoutine()
    {
        playerState?.SetInvincible(true);
        yield return new WaitForSeconds(invincibilityDuration);
        playerState?.SetInvincible(false);
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    #endregion

    #region Public Methods (Para acesso externo)

    public bool IsParryActive() => currentParryState == ParryState.Active;
    public bool IsParryReady() => currentParryState == ParryState.Ready;
    public bool HasDetectedThreats() => detectedThreats.Count > 0;
    public float GetParryProgress() => currentParryState == ParryState.Cooldown ? 
        (Time.time - lastParryTime) / GetCurrentCooldown() : 1f;

    #endregion

    #region Debug & Gizmos

    private void OnDrawGizmos()
    {
        // Área de detecção
        Gizmos.color = new Color(warningColor.r, warningColor.g, warningColor.b, GIZMO_ALPHA);
        Gizmos.DrawSphere(transform.position, detectionRadius);
        
        Gizmos.color = warningColor;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Área de parry
        Color parryGizmoColor = GetParryGizmoColor();
        Gizmos.color = new Color(parryGizmoColor.r, parryGizmoColor.g, parryGizmoColor.b, GIZMO_ALPHA);
        Gizmos.DrawSphere(transform.position, parryRadius);
        
        Gizmos.color = parryGizmoColor;
        Gizmos.DrawWireSphere(transform.position, parryRadius);

        // Área de reflexão
        Gizmos.color = new Color(1f, 0.5f, 0f, GIZMO_ALPHA); // Laranja
        Gizmos.DrawWireSphere(transform.position, reflectionRange);

        // Linha para a ameaça atual
        if (currentThreat != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentThreat.position);
        }
    }

    private Color GetParryGizmoColor()
    {
        switch (currentParryState)
        {
            case ParryState.Active:
                return isPerfectParry ? perfectParryColor : parryColor;
            case ParryState.Recovery when !parrySuccess:
                return failedParryColor;
            case ParryState.Cooldown:
                return Color.gray;
            default:
                return Color.blue;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Informações de debug
        if (Application.isPlaying)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, 
                $"Estado: {currentParryState}\n" +
                $"Ameaças: {detectedThreats.Count}\n" +
                $"Cooldown: {(GetCurrentCooldown() - (Time.time - lastParryTime)):F1}s");
        }
    }

    #endregion
}