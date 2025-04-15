using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDeathManager : MonoBehaviour
{
    public CanvasGroup deathCanvas; // Arraste o Canvas Group da tela de morte aqui
    public float fadeDuration = 2f; // Tempo do fade-in
    public static PlayerDeathManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Se já existe uma instância, destrua o novo objeto
        }
        else
        {
            Instance = this;  // Define esta instância como a instância global
        }
    }
    void Start()
    {
        deathCanvas.alpha = 0; // Começa invisível
        deathCanvas.gameObject.SetActive(false);
    }
    public void PlayerDie()
    {
        ShowDeathScreen();

    }
    private void ShowDeathScreen()
    {
        deathCanvas.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            deathCanvas.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            yield return null;
        }

        deathCanvas.alpha = 1; // Garante que fique totalmente visível
    }
}
