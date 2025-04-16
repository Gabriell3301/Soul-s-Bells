using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry Settings")]
    [SerializeField] private float parryWindow = 0.1f; // Tempo da janela de parry (em segundos)
    [SerializeField] private float parryCooldown = 0.4f; // Tempo de recarga do parry (em segundos)

    [Header("Parry Collider")]
    [SerializeField] private Vector2 parryColliderSize = new Vector2(1.2f, 2f);
    [SerializeField] private Vector2 parryColliderOffset = new Vector2(1f, 0f);

    private float parryStartTime = 0f; // Momento em que o parry foi iniciado
    private bool parryOnCooldown = false; // Indica se o parry está em cooldown
    private bool parryAttempted = false; // Indica se o parry foi tentado
    private int hitsToTake;
    private BoxCollider2D parryCollider;

    private PlayerParryFeedback feedback; // Referência para feedback visual/sonoro
    private PlayerParryManage manager; // Gerenciador de eventos de parry
    private PlayerControls playerControls; // Controles do jogador
    private PlayerStateList playerState; // Referência para estados do jogador

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Parry.performed += OnParryPerformed;
        SetupParryCollider();
    }

    private void Start()
    {
        feedback = GetComponentInChildren<PlayerParryFeedback>();
        manager = GetComponent<PlayerParryManage>();
        playerState = GetComponentInParent<PlayerStateList>();

        if (feedback == null || manager == null)
        {
            Debug.LogError("PlayerParry: Feedback ou Manager não encontrado!");
        }

        if (playerState == null)
        {
            Debug.LogError("PlayerParry: PlayerStateList não encontrado!");
        }
    }

    private void SetupParryCollider()
    {
        // Adicionar ou configurar o BoxCollider2D como trigger
        parryCollider = GetComponent<BoxCollider2D>();
        if (parryCollider == null)
        {
            parryCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        parryCollider.isTrigger = true;
        parryCollider.size = parryColliderSize;
        parryCollider.offset = parryColliderOffset;
        parryCollider.enabled = true;
    }

    private void OnEnable() => playerControls.Enable();

    private void OnDisable() => playerControls.Disable();

    /// <summary>
    /// Verifica se o jogador realizou um parry bem-sucedido
    /// </summary>
    public bool HasParried()
    {
        if (parryAttempted && Time.time <= parryStartTime + parryWindow && manager.IsEnemyAttackDetected())
        {
            parryAttempted = false; // Reseta o estado após a verificação
            return true;
        }
        return false;
    }

    /// <summary>
    /// Chamado quando o botão de parry é pressionado
    /// </summary>
    private void OnParryPerformed(InputAction.CallbackContext context)
    {
        if (!parryOnCooldown && !playerState.isInvulnerable)
        {
            TryParry();
        }
    }

    /// <summary>
    /// Abre a janela de parry e exibe o feedback amarelo.
    /// </summary>
    public void OpenParryWindow(int hits)
    {
        // Se o jogador estiver invulnerável, não abrir a janela de parry
        if (playerState.isInvulnerable)
            return;

        hitsToTake = hits;
        parryStartTime = Time.time;
        feedback.ShowParryWindow(); // Exibe o aviso amarelo de parry
        StartCoroutine(CloseParryWindow());
    }

    /// <summary>
    /// Fecha a janela de parry após o tempo definido.
    /// </summary>
    private IEnumerator CloseParryWindow()
    {
        yield return new WaitForSeconds(parryWindow);

        if (!parryAttempted && manager.IsEnemyAttackDetected())
        {
            manager.HandleParryFailure(hitsToTake); // Marca falha do parry se o jogador não tentou
        }

        feedback.HideParryWindow(); // Esconde o aviso amarelo após a janela expirar
    }

    /// <summary>
    /// Tenta executar o parry, mostrando sucesso ou falha
    /// </summary>
    private void TryParry()
    {
        if (!manager.IsEnemyAttackDetected())
        {
            Debug.Log("Nenhum ataque detectado para parry.");
            return;
        }

        parryAttempted = true;

        // Verifica se estamos dentro da janela de parry
        if (Time.time <= parryStartTime + parryWindow)
        {
            manager.HandleParrySuccess();
            Debug.Log("Acertou o parry.");
        }
        else
        {
            manager.HandleParryFailure(hitsToTake);
            Debug.Log("Errou o parry ou não fez parry a tempo.");
        }

        StartCoroutine(StartCooldown());
    }

    /// <summary>
    /// Começa o tempo de espera para usar o Parry novamente
    /// </summary>
    private IEnumerator StartCooldown()
    {
        parryOnCooldown = true;
        yield return new WaitForSeconds(parryCooldown);
        parryOnCooldown = false;
    }

    // Opcional: Visualização do collider de parry
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (parryCollider != null)
        {
            Gizmos.DrawWireCube(
                transform.position + new Vector3(parryCollider.offset.x, parryCollider.offset.y, 0),
                new Vector3(parryCollider.size.x, parryCollider.size.y, 0.1f)
            );
        }
        else
        {
            Gizmos.DrawWireCube(
                transform.position + new Vector3(parryColliderOffset.x, parryColliderOffset.y, 0),
                new Vector3(parryColliderSize.x, parryColliderSize.y, 0.1f)
            );
        }
    }
}