using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gerencia eventos do jogo usando o padrão Observer.
/// </summary>
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; } // Instância singleton

    // Eventos do jogador
    public UnityEvent<float> OnPlayerHealthChanged = new UnityEvent<float>(); // Evento de mudança de vida
    public UnityEvent<float> OnPlayerManaChanged = new UnityEvent<float>(); // Evento de mudança de mana
    public UnityEvent<int> OnPlayerCoinsChanged = new UnityEvent<int>(); // Evento de mudança de moedas
    public UnityEvent<int> OnPlayerScoreChanged = new UnityEvent<int>(); // Evento de mudança de pontuação
    public UnityEvent OnPlayerDeath = new UnityEvent(); // Evento de morte do jogador
    public UnityEvent OnPlayerRespawn = new UnityEvent(); // Evento de respawn do jogador

    // Eventos de estados do jogador
    public UnityEvent<string, bool> OnPlayerMovementStateChanged = new UnityEvent<string, bool>(); // Evento de mudança de estado de movimento
    public UnityEvent<string, bool> OnPlayerCombatStateChanged = new UnityEvent<string, bool>(); // Evento de mudança de estado de combate
    public UnityEvent<string, bool> OnPlayerInteractionStateChanged = new UnityEvent<string, bool>(); // Evento de mudança de estado de interação

    // Eventos de combate
    public UnityEvent<float> OnEnemyDamaged = new UnityEvent<float>(); // Evento de dano em inimigo
    public UnityEvent OnEnemyDeath = new UnityEvent(); // Evento de morte de inimigo
    public UnityEvent<float> OnPlayerDamaged = new UnityEvent<float>(); // Evento de dano no jogador
    public UnityEvent OnPlayerAttack = new UnityEvent(); // Evento de ataque do jogador
    public UnityEvent OnPlayerSpecialAttack = new UnityEvent(); // Evento de ataque especial

    // Eventos de progressão
    public UnityEvent<string> OnLevelCompleted = new UnityEvent<string>(); // Evento de nível completo
    public UnityEvent<string> OnAbilityUnlocked = new UnityEvent<string>(); // Evento de habilidade desbloqueada
    public UnityEvent<string> OnCheckpointReached = new UnityEvent<string>(); // Evento de checkpoint alcançado

    // Eventos de UI
    public UnityEvent OnPauseGame = new UnityEvent(); // Evento de pausa
    public UnityEvent OnResumeGame = new UnityEvent(); // Evento de despausa
    public UnityEvent OnGameOver = new UnityEvent(); // Evento de game over
    public UnityEvent OnMainMenu = new UnityEvent(); // Evento de menu principal

    /// <summary>
    /// Inicializa o singleton
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Inicializa todos os eventos
    /// </summary>
    private void InitializeEvents()
    {
        // Eventos do jogador
        OnPlayerHealthChanged = new UnityEvent<float>();
        OnPlayerManaChanged = new UnityEvent<float>();
        OnPlayerCoinsChanged = new UnityEvent<int>();
        OnPlayerScoreChanged = new UnityEvent<int>();
        OnPlayerDeath = new UnityEvent();
        OnPlayerRespawn = new UnityEvent();

        // Eventos de estados do jogador
        OnPlayerMovementStateChanged = new UnityEvent<string, bool>();
        OnPlayerCombatStateChanged = new UnityEvent<string, bool>();
        OnPlayerInteractionStateChanged = new UnityEvent<string, bool>();

        // Eventos de combate
        OnEnemyDamaged = new UnityEvent<float>();
        OnEnemyDeath = new UnityEvent();
        OnPlayerDamaged = new UnityEvent<float>();
        OnPlayerAttack = new UnityEvent();
        OnPlayerSpecialAttack = new UnityEvent();

        // Eventos de progressão
        OnLevelCompleted = new UnityEvent<string>();
        OnAbilityUnlocked = new UnityEvent<string>();
        OnCheckpointReached = new UnityEvent<string>();

        // Eventos de UI
        OnPauseGame = new UnityEvent();
        OnResumeGame = new UnityEvent();
        OnGameOver = new UnityEvent();
        OnMainMenu = new UnityEvent();
    }

    /// <summary>
    /// Dispara o evento de mudança de vida do jogador
    /// </summary>
    /// <param name="health">Nova vida</param>
    public void TriggerPlayerHealthChanged(float health)
    {
        OnPlayerHealthChanged?.Invoke(health);
    }

    /// <summary>
    /// Dispara o evento de mudança de mana do jogador
    /// </summary>
    /// <param name="mana">Nova mana</param>
    public void TriggerPlayerManaChanged(float mana)
    {
        OnPlayerManaChanged?.Invoke(mana);
    }

    /// <summary>
    /// Dispara o evento de mudança de moedas do jogador
    /// </summary>
    /// <param name="coins">Nova quantidade de moedas</param>
    public void TriggerPlayerCoinsChanged(int coins)
    {
        OnPlayerCoinsChanged?.Invoke(coins);
    }

    /// <summary>
    /// Dispara o evento de mudança de pontuação do jogador
    /// </summary>
    /// <param name="score">Nova pontuação</param>
    public void TriggerPlayerScoreChanged(int score)
    {
        OnPlayerScoreChanged?.Invoke(score);
    }

    /// <summary>
    /// Dispara o evento de morte do jogador
    /// </summary>
    public void TriggerPlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }

    /// <summary>
    /// Dispara o evento de respawn do jogador
    /// </summary>
    public void TriggerPlayerRespawn()
    {
        OnPlayerRespawn?.Invoke();
    }

    /// <summary>
    /// Dispara o evento de dano em inimigo
    /// </summary>
    /// <param name="damage">Quantidade de dano</param>
    public void TriggerEnemyDamaged(float damage)
    {
        OnEnemyDamaged?.Invoke(damage);
    }

    /// <summary>
    /// Dispara o evento de morte de inimigo
    /// </summary>
    public void TriggerEnemyDeath()
    {
        OnEnemyDeath?.Invoke();
    }

    /// <summary>
    /// Dispara o evento de dano no jogador
    /// </summary>
    /// <param name="damage">Quantidade de dano</param>
    public void TriggerPlayerDamaged(float damage)
    {
        OnPlayerDamaged?.Invoke(damage);
    }

    /// <summary>
    /// Dispara o evento de ataque do jogador
    /// </summary>
    public void TriggerPlayerAttack()
    {
        OnPlayerAttack?.Invoke();
    }

    /// <summary>
    /// Dispara o evento de ataque especial do jogador
    /// </summary>
    public void TriggerPlayerSpecialAttack()
    {
        OnPlayerSpecialAttack?.Invoke();
    }

    /// <summary>
    /// Dispara o evento de nível completo
    /// </summary>
    /// <param name="levelName">Nome do nível</param>
    public void TriggerLevelCompleted(string levelName)
    {
        OnLevelCompleted?.Invoke(levelName);
    }

    /// <summary>
    /// Dispara o evento de habilidade desbloqueada
    /// </summary>
    /// <param name="abilityName">Nome da habilidade</param>
    public void TriggerAbilityUnlocked(string abilityName)
    {
        OnAbilityUnlocked?.Invoke(abilityName);
    }

    /// <summary>
    /// Dispara o evento de checkpoint alcançado
    /// </summary>
    /// <param name="checkpointName">Nome do checkpoint</param>
    public void TriggerCheckpointReached(string checkpointName)
    {
        OnCheckpointReached?.Invoke(checkpointName);
    }

    /// <summary>
    /// Dispara o evento de pausa
    /// </summary>
    public void TriggerPauseGame()
    {
        OnPauseGame?.Invoke();
    }

    /// <summary>
    /// Dispara o evento de despausa
    /// </summary>
    public void TriggerResumeGame()
    {
        OnResumeGame?.Invoke();
    }

    /// <summary>
    /// Dispara o evento de game over
    /// </summary>
    public void TriggerGameOver()
    {
        OnGameOver?.Invoke();
    }

    /// <summary>
    /// Dispara o evento de menu principal
    /// </summary>
    public void TriggerMainMenu()
    {
        OnMainMenu?.Invoke();
    }

    /// <summary>
    /// Dispara o evento de mudança de estado de movimento do jogador
    /// </summary>
    /// <param name="stateName">Nome do estado</param>
    /// <param name="isActive">Se o estado está ativo</param>
    public void TriggerPlayerMovementStateChanged(string stateName, bool isActive)
    {
        OnPlayerMovementStateChanged?.Invoke(stateName, isActive);
    }

    /// <summary>
    /// Dispara o evento de mudança de estado de combate do jogador
    /// </summary>
    /// <param name="stateName">Nome do estado</param>
    /// <param name="isActive">Se o estado está ativo</param>
    public void TriggerPlayerCombatStateChanged(string stateName, bool isActive)
    {
        OnPlayerCombatStateChanged?.Invoke(stateName, isActive);
    }

    /// <summary>
    /// Dispara o evento de mudança de estado de interação do jogador
    /// </summary>
    /// <param name="stateName">Nome do estado</param>
    /// <param name="isActive">Se o estado está ativo</param>
    public void TriggerPlayerInteractionStateChanged(string stateName, bool isActive)
    {
        OnPlayerInteractionStateChanged?.Invoke(stateName, isActive);
    }
} 