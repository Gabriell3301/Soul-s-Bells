using System.Collections;
using UnityEngine;

public class PlayerParryManage : MonoBehaviour
{
    private EnemyAttack detectedAttack;
    private PlayerParry playerParry;
    private PlayerParryFeedback feedback;
    private PlayerHealth PlayerH;
    private int hits;

    private void Start()
    {
        PlayerH = GetComponentInParent<PlayerHealth>();
        playerParry = GetComponent<PlayerParry>();
        feedback = GetComponentInChildren<PlayerParryFeedback>();

        if (playerParry == null || feedback == null)
        {
            Debug.LogError("PlayerParryManage: Dependências não encontradas!");
        }
    }

    public bool IsEnemyAttackDetected() => detectedAttack != null;

    public void HandleParrySuccess()
    {
        if (detectedAttack != null)
        {
            detectedAttack.Destroythis();
            detectedAttack = null;
            feedback.ShowSuccess(); // Feedback verde
            StartCoroutine(ClearFeedbackAfterTime(0.5f));
        }
    }

    public void HandleParryFailure(int hits)
    {
        if (detectedAttack != null)
        {
            Debug.Log($"O jogador recebeu {hits} dano por falha ou ausência de parry."); // Log de falha
            feedback.ShowFailure(); // Feedback vermelho
            PlayerH.TakeHit(hits);
            StartCoroutine(ClearFeedbackAfterTime(0.3f));
        }
    }
    /// <summary>
    /// Limpa o aviso visual que o player tem
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator ClearFeedbackAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        feedback.HideParryWindow(); // Esconde qualquer feedback após o tempo
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            detectedAttack = collision.GetComponent<EnemyAttack>();
            hits = detectedAttack.Hits;
            if (detectedAttack != null)
            {
                playerParry.OpenParryWindow(hits);
            }
            else
            {
                Debug.Log("O jogador foi atingido por um ataque e não fez parry."); // Log de impacto sem parry
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack") && collision.GetComponent<EnemyAttack>() == detectedAttack)
        {
            if (!playerParry.HasParried())
            {
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
