using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia os checkpoints do jogo e o respawn do jogador.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; } // Instância singleton

    [System.Serializable]
    public class Checkpoint
    {
        public string id; // Identificador único do checkpoint
        public Transform spawnPoint; // Ponto de spawn
        public bool isActive; // Se o checkpoint está ativo
    }

    [Header("Configurações")]
    [SerializeField] private List<Checkpoint> checkpoints; // Lista de checkpoints
    [SerializeField] private float respawnDelay = 1f; // Delay do respawn
    [SerializeField] private GameObject respawnEffect; // Efeito de respawn

    private Checkpoint currentCheckpoint; // Checkpoint atual
    private GameObject player; // Referência ao jogador

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
    /// Inicializa os checkpoints
    /// </summary>
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (checkpoints.Count > 0)
        {
            currentCheckpoint = checkpoints[0];
        }
    }

    /// <summary>
    /// Ativa um checkpoint específico
    /// </summary>
    /// <param name="checkpointId">ID do checkpoint</param>
    public void ActivateCheckpoint(string checkpointId)
    {
        Checkpoint checkpoint = checkpoints.Find(cp => cp.id == checkpointId);
        if (checkpoint != null)
        {
            currentCheckpoint = checkpoint;
            checkpoint.isActive = true;

            // Notifica o sistema de eventos
            EventManager.Instance.TriggerCheckpointReached(checkpointId);
        }
    }

    /// <summary>
    /// Respawna o jogador no checkpoint atual
    /// </summary>
    public void RespawnPlayer()
    {
        if (currentCheckpoint != null && player != null)
        {
            StartCoroutine(RespawnSequence());
        }
    }

    /// <summary>
    /// Sequência de respawn com delay e efeitos
    /// </summary>
    private IEnumerator RespawnSequence()
    {
        // Desativa o jogador
        player.SetActive(false);

        // Espera o delay
        yield return new WaitForSeconds(respawnDelay);

        // Move o jogador para o checkpoint
        player.transform.position = currentCheckpoint.spawnPoint.position;
        player.transform.rotation = currentCheckpoint.spawnPoint.rotation;

        // Ativa o efeito de respawn
        if (respawnEffect != null)
        {
            Instantiate(respawnEffect, currentCheckpoint.spawnPoint.position, Quaternion.identity);
        }

        // Reativa o jogador
        player.SetActive(true);

        // Notifica o sistema de eventos
        EventManager.Instance.TriggerPlayerRespawn();
    }

    /// <summary>
    /// Retorna o checkpoint atual
    /// </summary>
    public Checkpoint GetCurrentCheckpoint()
    {
        return currentCheckpoint;
    }

    /// <summary>
    /// Verifica se um checkpoint está ativo
    /// </summary>
    /// <param name="checkpointId">ID do checkpoint</param>
    public bool IsCheckpointActive(string checkpointId)
    {
        Checkpoint checkpoint = checkpoints.Find(cp => cp.id == checkpointId);
        return checkpoint != null && checkpoint.isActive;
    }

    /// <summary>
    /// Reseta todos os checkpoints
    /// </summary>
    public void ResetAllCheckpoints()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.isActive = false;
        }
        if (checkpoints.Count > 0)
        {
            currentCheckpoint = checkpoints[0];
        }
    }

    /// <summary>
    /// Adiciona um novo checkpoint
    /// </summary>
    /// <param name="checkpoint">Novo checkpoint</param>
    public void AddCheckpoint(Checkpoint checkpoint)
    {
        if (!checkpoints.Exists(cp => cp.id == checkpoint.id))
        {
            checkpoints.Add(checkpoint);
        }
    }

    /// <summary>
    /// Remove um checkpoint
    /// </summary>
    /// <param name="checkpointId">ID do checkpoint</param>
    public void RemoveCheckpoint(string checkpointId)
    {
        checkpoints.RemoveAll(cp => cp.id == checkpointId);
        if (currentCheckpoint != null && currentCheckpoint.id == checkpointId)
        {
            currentCheckpoint = checkpoints.Count > 0 ? checkpoints[0] : null;
        }
    }
} 