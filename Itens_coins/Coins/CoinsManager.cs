using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinsManager : MonoBehaviour
{
    // Inst�ncia Singleton
    public static CoinsManager Instance { get; private set; }

    public int moedas = 0; // Quantidade de moedas do jogador
    public TextMeshProUGUI moedasText; // Refer�ncia ao Text da UI para exibir moedas

    // Inicializa��o do Singleton
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Garantir que s� existe uma inst�ncia do CoinsManager
        }
        else
        {
            Instance = this;
        }
    }
    public void SetCoins(int quantidade)
    {
        moedas = quantidade;
        AtualizarInterface();
    }
    // M�todo para adicionar moedas
    public void AdicionarMoeda(int valor)
    {
        moedas += valor;
        SaveGameManager.Instance.AddMoeda(moedas);
        AtualizarInterface();
    }

    // Atualizar o UI com a quantidade de moedas
    void AtualizarInterface()
    {
        if (moedasText != null)
        {
            moedasText.text = moedas.ToString();
        }
    }
}
