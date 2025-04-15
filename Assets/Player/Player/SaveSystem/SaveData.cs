using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string currentRoom;
    public int moedas;

    public List<string> unlockedAbilities;
    public List<string> equippedAbilities;
}