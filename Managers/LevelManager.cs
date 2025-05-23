using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gerencia o carregamento e transição entre níveis do jogo.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; } // Instância singleton

    [Header("Configurações de Nível")]
    [SerializeField] private float transitionTime = 1f; // Tempo de transição
    [SerializeField] private Animator transitionAnimator; // Animador da transição
    [SerializeField] private string transitionTrigger = "Start"; // Trigger da animação

    [Header("Níveis")]
    [SerializeField] private string mainMenuScene = "MainMenu"; // Cena do menu principal
    [SerializeField] private string gameOverScene = "GameOver"; // Cena de game over
    [SerializeField] private string[] levelScenes; // Array de cenas de nível

    private int currentLevelIndex = -1; // Índice do nível atual
    private bool isLoading = false; // Estado de carregamento

    /// <summary>
    /// Inicializa o singleton
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
        }
    }

    /// <summary>
    /// Carrega o menu principal
    /// </summary>
    public void LoadMainMenu()
    {
        StartCoroutine(LoadScene(mainMenuScene));
    }

    /// <summary>
    /// Carrega a tela de game over
    /// </summary>
    public void LoadGameOver()
    {
        StartCoroutine(LoadScene(gameOverScene));
    }

    /// <summary>
    /// Carrega o próximo nível
    /// </summary>
    public void LoadNextLevel()
    {
        if (currentLevelIndex < levelScenes.Length - 1)
        {
            currentLevelIndex++;
            StartCoroutine(LoadScene(levelScenes[currentLevelIndex]));
        }
        else
        {
            LoadMainMenu();
        }
    }

    /// <summary>
    /// Reinicia o nível atual
    /// </summary>
    public void RestartCurrentLevel()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levelScenes.Length)
        {
            StartCoroutine(LoadScene(levelScenes[currentLevelIndex]));
        }
    }

    /// <summary>
    /// Carrega um nível específico
    /// </summary>
    /// <param name="levelIndex">Índice do nível</param>
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelScenes.Length)
        {
            currentLevelIndex = levelIndex;
            StartCoroutine(LoadScene(levelScenes[levelIndex]));
        }
    }

    /// <summary>
    /// Carrega uma cena com transição
    /// </summary>
    /// <param name="sceneName">Nome da cena</param>
    private IEnumerator LoadScene(string sceneName)
    {
        if (isLoading) yield break;
        isLoading = true;

        // Inicia a animação de transição
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger(transitionTrigger);
        }

        // Espera a animação terminar
        yield return new WaitForSeconds(transitionTime);

        // Carrega a nova cena
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        isLoading = false;
    }

    /// <summary>
    /// Retorna o índice do nível atual
    /// </summary>
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    /// <summary>
    /// Retorna o nome do nível atual
    /// </summary>
    public string GetCurrentLevelName()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levelScenes.Length)
        {
            return levelScenes[currentLevelIndex];
        }
        return string.Empty;
    }

    /// <summary>
    /// Verifica se é o último nível
    /// </summary>
    public bool IsLastLevel()
    {
        return currentLevelIndex == levelScenes.Length - 1;
    }

    /// <summary>
    /// Verifica se é o primeiro nível
    /// </summary>
    public bool IsFirstLevel()
    {
        return currentLevelIndex == 0;
    }
} 