using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry Settings")]
    [SerializeField] private float parryWindow = 0.1f; // Tempo da janela de parry (em segundos)
    [SerializeField] private float parryCooldown = 0.4f; // Tempo de recarga do parry (em segundos)
    private float parryStartTime = 0f; // Momento em que o parry foi iniciado
    private bool parryOnCooldown = false; // Indica se o parry está em cooldown
    private bool parryAttempted = false; // Indica se o parry foi tentado
    private int hitsToTake;

    private PlayerParryFeedback feedback; // Referência para feedback visual/sonoro
    private PlayerParryManage manager; // Gerenciador de eventos de parry
    private PlayerControls playerControls; // Controles do jogador

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Parry.performed += OnParryPerformed;
    }

    private void Start()
    {
        feedback = GetComponentInChildren<PlayerParryFeedback>();
        manager = GetComponent<PlayerParryManage>();
        if (feedback == null || manager == null)
        {
            Debug.LogError("PlayerParry: Feedback ou Manager não encontrado!");
        }
    }

    private void OnEnable() => playerControls.Enable();

    private void OnDisable() => playerControls.Disable();
    public bool HasParried()
    {
        if (parryAttempted && Time.time <= parryStartTime + parryWindow && manager.IsEnemyAttackDetected())
        {
            parryAttempted = false; // Reseta o estado após a verificação
            return true;
        }
        return false;
    }
    private void OnParryPerformed(InputAction.CallbackContext context)
    {
        if (!parryOnCooldown)
        {
            TryParry();
        }
    }

    /// <summary>
    /// Abre a janela de parry e exibe o feedback amarelo.
    /// </summary>
    public void OpenParryWindow(int hits)
    {
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
    /// <returns></returns>
    private IEnumerator StartCooldown()
    {
        parryOnCooldown = true;
        yield return new WaitForSeconds(parryCooldown);
        parryOnCooldown = false;
    }
}
