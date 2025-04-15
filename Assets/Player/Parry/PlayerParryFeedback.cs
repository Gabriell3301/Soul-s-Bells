using UnityEngine;

public class PlayerParryFeedback : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("PlayerParryFeedback: SpriteRenderer n�o encontrado!");
        }

        spriteRenderer.color = Color.clear; // Inicia invis�vel
    }

    public void HideParryWindow() => spriteRenderer.color = Color.clear;

    public void ShowParryWindow() => spriteRenderer.color = Color.yellow;

    public void ShowSuccess() => spriteRenderer.color = Color.green;

    public void ShowFailure() => spriteRenderer.color = Color.red;
}
