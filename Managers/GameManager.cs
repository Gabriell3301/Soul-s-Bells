using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gerencia o estado geral do jogo, incluindo pausa, game over e transições de cena.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Instância singleton

    [Header("Configurações de Jogo")]
    [SerializeField] private float gameOverDelay = 2f; // Tempo de espera antes do game over
    [SerializeField] private string mainMenuScene = "MainMenu"; // Nome da cena do menu principal
    [SerializeField] private string gameOverScene = "GameOver"; // Nome da cena de game over

    private bool isPaused = false; // Estado de pausa do jogo
    private bool isGameOver = false; // Estado de game over

    /// <summary>
    /// Inicializa o singleton e configura o jogo
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Inicializa o jogo
    /// </summary>
    private void InitializeGame()
    {
        isPaused = false;
        isGameOver = false;
        Time.timeScale = 1f;

        // Garante que as cenas existam
        if (string.IsNullOrEmpty(mainMenuScene))
        {
            mainMenuScene = "MainMenu";
            Debug.LogWarning("Nome da cena do menu principal não definido. Usando 'MainMenu' como padrão.");
        }

        if (string.IsNullOrEmpty(gameOverScene))
        {
            gameOverScene = "GameOver";
            Debug.LogWarning("Nome da cena de game over não definido. Usando 'GameOver' como padrão.");
        }
    }

    /// <summary>
    /// Pausa ou despausa o jogo
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        // Notifica o sistema de eventos
        if (EventManager.Instance != null)
        {
            if (isPaused)
            {
                EventManager.Instance.TriggerPauseGame();
            }
            else
            {
                EventManager.Instance.TriggerResumeGame();
            }
        }
    }

    /// <summary>
    /// Inicia a sequência de game over
    /// </summary>
    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            StartCoroutine(GameOverSequence());
        }
    }

    /// <summary>
    /// Sequência de game over com delay
    /// </summary>
    private IEnumerator GameOverSequence()
    {
        // Notifica o sistema de eventos
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerGameOver();
        }

        yield return new WaitForSeconds(gameOverDelay);
        LoadGameOverScene();
    }

    /// <summary>
    /// Carrega a cena de game over
    /// </summary>
    public void LoadGameOverScene()
    {
        if (string.IsNullOrEmpty(gameOverScene))
        {
            Debug.LogError("Nome da cena de game over não definido!");
            return;
        }

        SceneManager.LoadScene(gameOverScene);
    }

    /// <summary>
    /// Carrega o menu principal
    /// </summary>
    public void LoadMainMenu()
    {
        if (string.IsNullOrEmpty(mainMenuScene))
        {
            Debug.LogError("Nome da cena do menu principal não definido!");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);

        // Notifica o sistema de eventos
        if (EventManager.Instance != null)
        {
            EventManager.Instance.TriggerMainMenu();
        }
    }

    /// <summary>
    /// Reinicia a cena atual
    /// </summary>
    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Sai do jogo
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 