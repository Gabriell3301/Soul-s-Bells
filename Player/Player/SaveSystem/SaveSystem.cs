using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SaveSystem
{
    static string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SoulBell");
    static string path = Path.Combine(folder, "save.json");
    public static void SaveGame(string room, int moedas, List<string> unlocked, List<string> equipped)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
            Debug.Log("Diretório criado: " + folder);
        }
        SaveData data = new SaveData
        {
            currentRoom = room,
            moedas = moedas,
            unlockedAbilities = unlocked,
            equippedAbilities = equipped
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Jogo salvo em: " + path);
    }
    public static SaveData LoadGame()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Jogo carregado de: " + path);
            return data;
        }
        else
        {
            Debug.LogWarning("Nenhum save encontrado.");
            return null;
        }
    }
}

