using System.Collections;
using UnityEngine;

public class PlayerParryManage : MonoBehaviour
{
    [Header("Parry Effects")]
    [SerializeField] private float successFeedbackDuration = 0.5f;
    [SerializeField] private float failureFeedbackDuration = 0.3f;
    // Removido o parrySuccessEffect pois não será mais usado

    private EnemyAttack detectedAttack;
    private PlayerParry playerParry;
    private PlayerParryFeedback feedback;
    private PlayerHealth playerHealth;
    private int hits;
    private float detectionTime;

    private void Start()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
        playerParry = GetComponent<PlayerParry>();
        feedback = GetComponentInChildren<PlayerParryFeedback>();

        if (playerParry == null || feedback == null || playerHealth == null)
        {
            Debug.LogError("PlayerParryManage: Dependências não encontradas!");
        }
    }

    /// <summary>
    /// Verifica se um ataque inimigo foi detectado
    /// </summary>
    public bool IsEnemyAttackDetected() => detectedAttack != null;

    /// <summary>
    /// Lida com um parry bem-sucedido
    /// </summary>
    public void HandleParrySuccess()
    {
        if (detectedAttack != null)
        {
            // Efeito visual de sucesso (verde)
            feedback.ShowSuccess();

            // Destroy o ataque inimigo
            detectedAttack.Destroythis();
            detectedAttack = null;

            StartCoroutine(ClearFeedbackAfterTime(successFeedbackDuration));
        }
    }

    /// <summary>
    /// Lida com uma falha de parry
    /// </summary>
    public void HandleParryFailure(int hits)
    {
        if (detectedAttack != null)
        {
            Debug.Log($"O jogador recebeu {hits} hit(s) por falha ou ausência de parry.");

            // Efeito visual de falha (vermelho)
            feedback.ShowFailure();

            // Aplica dano ao jogador
            playerHealth.TakeHit(hits);

            // Limpa o feedback após um tempo
            StartCoroutine(ClearFeedbackAfterTime(failureFeedbackDuration));

            // Opcional: destruir o ataque após a falha
            detectedAttack.Destroythis();
            detectedAttack = null;
        }
    }

    /// <summary>
    /// Limpa o aviso visual do player após um tempo
    /// </summary>
    private IEnumerator ClearFeedbackAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        feedback.HideParryWindow(); // Esconde qualquer feedback após o tempo
    }

    /// <summary>
    /// Detecta quando um ataque inimigo entra na zona de parry
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            EnemyAttack attackComponent = collision.GetComponent<EnemyAttack>();
            if (attackComponent != null)
            {
                detectedAttack = attackComponent;
                hits = detectedAttack.Hits;
                detectionTime = Time.time;

                // Mostra o aviso de ataque próximo (amarelo)
                feedback.ShowParryWindow();

                // Abre a janela de parry
                playerParry.OpenParryWindow(hits);
            }
        }
    }

    /// <summary>
    /// Detecta quando um ataque inimigo sai da zona de parry
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack") && collision.GetComponent<EnemyAttack>() == detectedAttack)
        {
            if (!playerParry.HasParried())
            {
                // Se não houve parry bem-sucedido, verificar se já aplicamos o dano
                // Se não aplicamos, aplicar agora
                HandleParryFailure(hits);
                Debug.Log("O jogador não fez parry e recebeu dano.");
            }
            else
            {
                Debug.Log("O jogador fez parry com sucesso.");
            }

            detectedAttack = null;
            feedback.HideParryWindow();
        }
    }
}