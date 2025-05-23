using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gerencia todos os elementos da interface do usuário do jogo.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // Instância singleton

    [Header("Painéis de UI")]
    [SerializeField] private GameObject pausePanel; // Painel de pausa
    [SerializeField] private GameObject gameOverPanel; // Painel de game over
    [SerializeField] private GameObject mainMenuPanel; // Painel do menu principal
    [SerializeField] private GameObject hudPanel; // Painel do HUD

    [Header("Elementos do HUD")]
    [SerializeField] private LifeUI lifeUI; // Sistema de hits (corações)
    [SerializeField] private Slider manaBar; // Barra de mana
    [SerializeField] private TextMeshProUGUI coinText; // Texto de moedas
    [SerializeField] private TextMeshProUGUI scoreText; // Texto de pontuação

    [Header("Configurações")]
    [SerializeField] private float fadeSpeed = 1f; // Velocidade do fade
    [SerializeField] private Color fadeColor = Color.black; // Cor do fade

    private CanvasGroup currentPanel; // Painel atual ativo

    /// <summary>
    /// Inicializa o singleton e configura a UI inicial
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
            return;
        }

        // Inicializa todos os painéis como inativos
        if (pausePanel) pausePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (hudPanel) hudPanel.SetActive(false);
    }

    /// <summary>
    /// Atualiza a UI de vida usando o sistema de hits
    /// </summary>
    /// <param name="currentHits">Número atual de hits</param>
    /// <param name="maxHits">Número máximo de hits</param>
    public void UpdateHealthUI(int currentHits, int maxHits)
    {
        if (lifeUI != null)
        {
            if (currentHits == maxHits)
            {
                lifeUI.Initialize(maxHits);
            }
            lifeUI.UpdateUI(currentHits);
        }
    }

    /// <summary>
    /// Atualiza a barra de mana
    /// </summary>
    /// <param name="currentMana">Mana atual</param>
    /// <param name="maxMana">Mana máxima</param>
    public void UpdateManaBar(float currentMana, float maxMana)
    {
        if (manaBar != null)
        {
            manaBar.value = currentMana / maxMana;
        }
    }

    /// <summary>
    /// Atualiza o texto de moedas
    /// </summary>
    /// <param name="amount">Quantidade de moedas</param>
    public void UpdateCoinText(int amount)
    {
        if (coinText != null)
        {
            coinText.text = amount.ToString();
        }
    }

    /// <summary>
    /// Atualiza o texto de pontuação
    /// </summary>
    /// <param name="score">Pontuação atual</param>
    public void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    /// <summary>
    /// Mostra o painel de pausa
    /// </summary>
    public void ShowPausePanel()
    {
        ShowPanel(pausePanel);
    }

    /// <summary>
    /// Mostra o painel de game over
    /// </summary>
    public void ShowGameOverPanel()
    {
        ShowPanel(gameOverPanel);
    }

    /// <summary>
    /// Mostra o painel do menu principal
    /// </summary>
    public void ShowMainMenuPanel()
    {
        ShowPanel(mainMenuPanel);
    }

    /// <summary>
    /// Mostra o painel do HUD
    /// </summary>
    public void ShowHUDPanel()
    {
        ShowPanel(hudPanel);
    }

    /// <summary>
    /// Mostra um painel específico com fade
    /// </summary>
    private void ShowPanel(GameObject panel)
    {
        if (panel == null) return;

        // Desativa o painel atual se houver
        if (currentPanel != null)
        {
            StartCoroutine(FadeOut(currentPanel));
        }

        // Ativa e configura o novo painel
        panel.SetActive(true);
        currentPanel = panel.GetComponent<CanvasGroup>();
        if (currentPanel != null)
        {
            StartCoroutine(FadeIn(currentPanel));
        }
    }

    /// <summary>
    /// Aplica fade in em um CanvasGroup
    /// </summary>
    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += fadeSpeed * Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    /// <summary>
    /// Aplica fade out em um CanvasGroup
    /// </summary>
    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        canvasGroup.gameObject.SetActive(false);
    }
} 