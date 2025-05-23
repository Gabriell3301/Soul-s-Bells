using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia o sistema de salvamento e carregamento do jogo.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; } // Instância singleton

    [System.Serializable]
    public class GameData
    {
        public int coins; // Quantidade de moedas
        public int score; // Pontuação
        public float playerHealth; // Vida do jogador
        public float playerMana; // Mana do jogador
        public Vector3 playerPosition; // Posição do jogador
        public List<string> unlockedAbilities; // Habilidades desbloqueadas
        public Dictionary<string, bool> completedLevels; // Níveis completados
    }

    private GameData currentGameData; // Dados atuais do jogo
    private const string SAVE_KEY = "GameSave"; // Chave para salvar os dados

    /// <summary>
    /// Inicializa o singleton e carrega os dados salvos
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Salva os dados atuais do jogo
    /// </summary>
    public void SaveGame()
    {
        string jsonData = JsonUtility.ToJson(currentGameData);
        PlayerPrefs.SetString(SAVE_KEY, jsonData);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Carrega os dados salvos do jogo
    /// </summary>
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string jsonData = PlayerPrefs.GetString(SAVE_KEY);
            currentGameData = JsonUtility.FromJson<GameData>(jsonData);
        }
        else
        {
            currentGameData = new GameData();
            InitializeNewGame();
        }
    }

    /// <summary>
    /// Inicializa um novo jogo com valores padrão
    /// </summary>
    private void InitializeNewGame()
    {
        currentGameData.coins = 0;
        currentGameData.score = 0;
        currentGameData.playerHealth = 100f;
        currentGameData.playerMana = 100f;
        currentGameData.playerPosition = Vector3.zero;
        currentGameData.unlockedAbilities = new List<string>();
        currentGameData.completedLevels = new Dictionary<string, bool>();
    }

    /// <summary>
    /// Atualiza a quantidade de moedas
    /// </summary>
    /// <param name="amount">Nova quantidade de moedas</param>
    public void UpdateCoins(int amount)
    {
        currentGameData.coins = amount;
        SaveGame();
    }

    /// <summary>
    /// Atualiza a pontuação
    /// </summary>
    /// <param name="score">Nova pontuação</param>
    public void UpdateScore(int score)
    {
        currentGameData.score = score;
        SaveGame();
    }

    /// <summary>
    /// Atualiza a vida do jogador
    /// </summary>
    /// <param name="health">Nova vida</param>
    public void UpdatePlayerHealth(float health)
    {
        currentGameData.playerHealth = health;
        SaveGame();
    }

    /// <summary>
    /// Atualiza a mana do jogador
    /// </summary>
    /// <param name="mana">Nova mana</param>
    public void UpdatePlayerMana(float mana)
    {
        currentGameData.playerMana = mana;
        SaveGame();
    }

    /// <summary>
    /// Atualiza a posição do jogador
    /// </summary>
    /// <param name="position">Nova posição</param>
    public void UpdatePlayerPosition(Vector3 position)
    {
        currentGameData.playerPosition = position;
        SaveGame();
    }

    /// <summary>
    /// Desbloqueia uma nova habilidade
    /// </summary>
    /// <param name="abilityName">Nome da habilidade</param>
    public void UnlockAbility(string abilityName)
    {
        if (!currentGameData.unlockedAbilities.Contains(abilityName))
        {
            currentGameData.unlockedAbilities.Add(abilityName);
            SaveGame();
        }
    }

    /// <summary>
    /// Marca um nível como completo
    /// </summary>
    /// <param name="levelName">Nome do nível</param>
    public void CompleteLevel(string levelName)
    {
        if (!currentGameData.completedLevels.ContainsKey(levelName))
        {
            currentGameData.completedLevels.Add(levelName, true);
            SaveGame();
        }
    }

    /// <summary>
    /// Verifica se uma habilidade está desbloqueada
    /// </summary>
    /// <param name="abilityName">Nome da habilidade</param>
    public bool IsAbilityUnlocked(string abilityName)
    {
        return currentGameData.unlockedAbilities.Contains(abilityName);
    }

    /// <summary>
    /// Verifica se um nível está completo
    /// </summary>
    /// <param name="levelName">Nome do nível</param>
    public bool IsLevelCompleted(string levelName)
    {
        return currentGameData.completedLevels.ContainsKey(levelName) && 
               currentGameData.completedLevels[levelName];
    }

    /// <summary>
    /// Deleta todos os dados salvos
    /// </summary>
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        InitializeNewGame();
    }
} 