using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Impede duplicação ao voltar para a cena inicial
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Preserva o jogador ao trocar de cena
    }
}
