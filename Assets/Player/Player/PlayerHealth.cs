using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHits = 5; // Quantidade de hits que o jogador pode tomar
    private int currentHits;

    public float invulnerabilityTime = 1.5f; // Tempo de invulnerabilidade ap�s ser atingido
    public LifeUI LifeUI;

    private PlayerDeathManager playerDeath;
    private SpriteRenderer spriteRenderer;
    private PlayerStateList pState;

    private void Start()
    {
        playerDeath = FindAnyObjectByType<PlayerDeathManager>();
        pState = GetComponent<PlayerStateList>();
        currentHits = maxHits;
        spriteRenderer = GetComponent<SpriteRenderer>();
        LifeUI.Initialize(currentHits);
    }

    public void TakeHit(int hits)
    {
        if (!pState.isInvulnerable)
        {
            currentHits -= hits;
            LifeUI.UpdateUI(currentHits);

            if (currentHits <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(InvulnerabilityRoutine());
            }
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        pState.SetInvulnerable(true);
        float blinkInterval = 0.05f; // Tempo entre os piscados
        int blinkCount = Mathf.RoundToInt(invulnerabilityTime / (blinkInterval * 2)); // Quantidade de piscadas

        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(blinkInterval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
        }

        pState.SetInvulnerable(false);
    }

    private void Die()
    {
        Debug.Log("O jogador morreu!");
        playerDeath.PlayerDie();
        // Aqui voc� pode adicionar anima��o de morte, respawn, etc.
    }
}
