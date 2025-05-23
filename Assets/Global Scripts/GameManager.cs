using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gerencia o sistema de salvamento e carregamento do jogo.
/// </summary>
public class SaveGameManager : MonoBehaviour
{
    public string CurrentRoomName { get; private set; }
    public int Moedas { get; private set; }

    public static SaveGameManager Instance;
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
    public void SetCurrentRoom(string roomName)
    {
        CurrentRoomName = roomName;
    }

    public void AddMoeda(int quantidade)
    {
        Moedas = quantidade;
    }
    public void SaveGame(string room, int moedas, PlayerAbilitySystem playerAbilities)
    {
        var unlocked = new List<string>();
        foreach (var a in playerAbilities.GetUnlockedAbilities())
            unlocked.Add(a.abilityName);

        var equipped = new List<string>();
        foreach (var a in playerAbilities.GetEquippedAbilities())
            equipped.Add(a.abilityName);

        SaveSystem.SaveGame(room, moedas, unlocked, equipped);
    }
    public void LoadGame()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data == null) return;

        // Recuperar a referência ao player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var abilitySystem = player.GetComponent<PlayerAbilitySystem>();
        if (abilitySystem == null) return;

        abilitySystem.ClearAbilities(); // Limpar habilidades anteriores

        // Restaurar as habilidades desbloqueadas e adicionar ao AbilityDatabase
        foreach (string abilityName in data.unlockedAbilities)
        {
            AbilityData ability = AbilityDataBase.Instance.GetAbilityByName(abilityName);
            Debug.Log(ability);
            if (ability != null)
            {
                abilitySystem.UnlockAbility(ability);
                AbilityDataBase.Instance.AddAbility(ability); // Adiciona a habilidade ao banco
                Debug.Log($"Habilidade {ability.abilityName} carregada com sucesso");
            }
        }

        // Restaurar as habilidades equipadas
        foreach (string abilityName in data.equippedAbilities)
        {
            AbilityData ability = AbilityDataBase.Instance.GetAbilityByName(abilityName);
            if (ability != null)
            {
                abilitySystem.EquipAbility(ability);
            }
        }
        CoinsManager.Instance.SetCoins(data.moedas); // Atualizar o número de moedas
        // Carregar a cena correta
        SceneManager.LoadScene(data.currentRoom);

        Debug.Log("✅ Jogo carregado com sucesso!");
    }
}
