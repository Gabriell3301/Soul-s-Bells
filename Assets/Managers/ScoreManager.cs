using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia o sistema de pontuação do jogo.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; } // Instância singleton

    [Header("Configurações de Pontuação")]
    [SerializeField] private int baseScore = 100; // Pontuação base
    [SerializeField] private int comboMultiplier = 10; // Multiplicador de combo
    [SerializeField] private float comboTime = 3f; // Tempo para manter o combo
    [SerializeField] private int maxCombo = 10; // Combo máximo

    private int currentScore; // Pontuação atual
    private int currentCombo; // Combo atual
    private float comboTimer; // Timer do combo
    private int highScore; // Recorde

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

        // Carrega o recorde
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    /// <summary>
    /// Atualiza o combo e a pontuação
    /// </summary>
    private void Update()
    {
        if (currentCombo > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }

    /// <summary>
    /// Adiciona pontos ao jogador
    /// </summary>
    /// <param name="points">Quantidade de pontos</param>
    public void AddScore(int points)
    {
        // Incrementa o combo
        currentCombo = Mathf.Min(currentCombo + 1, maxCombo);
        comboTimer = comboTime;

        // Calcula a pontuação com o combo
        int comboBonus = currentCombo * comboMultiplier;
        int totalPoints = points + comboBonus;

        // Adiciona os pontos
        currentScore += totalPoints;

        // Atualiza o recorde se necessário
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        // Notifica o sistema de eventos
        EventManager.Instance.TriggerPlayerScoreChanged(currentScore);
    }

    /// <summary>
    /// Reseta o combo
    /// </summary>
    public void ResetCombo()
    {
        currentCombo = 0;
        comboTimer = 0;
    }

    /// <summary>
    /// Reseta a pontuação
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        ResetCombo();
        EventManager.Instance.TriggerPlayerScoreChanged(currentScore);
    }

    /// <summary>
    /// Retorna a pontuação atual
    /// </summary>
    public int GetCurrentScore()
    {
        return currentScore;
    }

    /// <summary>
    /// Retorna o recorde
    /// </summary>
    public int GetHighScore()
    {
        return highScore;
    }

    /// <summary>
    /// Retorna o combo atual
    /// </summary>
    public int GetCurrentCombo()
    {
        return currentCombo;
    }

    /// <summary>
    /// Retorna o tempo restante do combo
    /// </summary>
    public float GetComboTimeRemaining()
    {
        return comboTimer;
    }

    /// <summary>
    /// Adiciona pontos por derrotar um inimigo
    /// </summary>
    /// <param name="enemyType">Tipo do inimigo</param>
    public void AddEnemyKillScore(string enemyType)
    {
        int points = baseScore;
        switch (enemyType.ToLower())
        {
            case "boss":
                points *= 10;
                break;
            case "elite":
                points *= 5;
                break;
            case "normal":
                points *= 1;
                break;
        }
        AddScore(points);
    }

    /// <summary>
    /// Adiciona pontos por coletar um item
    /// </summary>
    /// <param name="itemType">Tipo do item</param>
    public void AddItemScore(string itemType)
    {
        int points = baseScore / 2;
        switch (itemType.ToLower())
        {
            case "rare":
                points *= 5;
                break;
            case "common":
                points *= 1;
                break;
        }
        AddScore(points);
    }

    /// <summary>
    /// Adiciona pontos por completar um objetivo
    /// </summary>
    /// <param name="objectiveType">Tipo do objetivo</param>
    public void AddObjectiveScore(string objectiveType)
    {
        int points = baseScore * 2;
        switch (objectiveType.ToLower())
        {
            case "main":
                points *= 5;
                break;
            case "side":
                points *= 2;
                break;
        }
        AddScore(points);
    }
} 