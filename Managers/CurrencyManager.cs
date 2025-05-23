using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia o sistema de moedas e outras moedas do jogo.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; } // Instância singleton

    [System.Serializable]
    public class Currency
    {
        public string id; // Identificador único da moeda
        public string name; // Nome da moeda
        public Sprite icon; // Ícone da moeda
        public int amount; // Quantidade atual
    }

    [Header("Configurações")]
    [SerializeField] private List<Currency> currencies; // Lista de moedas
    [SerializeField] private int startingCoins = 0; // Moedas iniciais

    private Dictionary<string, Currency> currencyDictionary; // Dicionário de moedas

    /// <summary>
    /// Inicializa o singleton
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCurrencies();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Inicializa as moedas
    /// </summary>
    private void InitializeCurrencies()
    {
        if (currencies == null)
        {
            currencies = new List<Currency>();
        }

        currencyDictionary = new Dictionary<string, Currency>();

        // Inicializa o dicionário de moedas
        foreach (Currency currency in currencies)
        {
            if (currency != null && !string.IsNullOrEmpty(currency.id))
            {
                currencyDictionary[currency.id] = currency;
            }
        }

        // Carrega as moedas salvas
        LoadCurrencies();
    }

    /// <summary>
    /// Carrega as moedas salvas
    /// </summary>
    private void LoadCurrencies()
    {
        foreach (Currency currency in currencies)
        {
            if (currency != null)
            {
                currency.amount = PlayerPrefs.GetInt("Currency_" + currency.id, startingCoins);
            }
        }
    }

    /// <summary>
    /// Salva as moedas atuais
    /// </summary>
    private void SaveCurrencies()
    {
        foreach (Currency currency in currencies)
        {
            if (currency != null)
            {
                PlayerPrefs.SetInt("Currency_" + currency.id, currency.amount);
            }
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Adiciona moedas
    /// </summary>
    /// <param name="currencyId">ID da moeda</param>
    /// <param name="amount">Quantidade</param>
    public void AddCurrency(string currencyId, int amount)
    {
        if (string.IsNullOrEmpty(currencyId) || amount <= 0) return;

        if (currencyDictionary.TryGetValue(currencyId, out Currency currency))
        {
            currency.amount += amount;
            SaveCurrencies();

            // Notifica o sistema de eventos
            if (currencyId == "coins" && EventManager.Instance != null)
            {
                EventManager.Instance.TriggerPlayerCoinsChanged(currency.amount);
            }
        }
    }

    /// <summary>
    /// Remove moedas
    /// </summary>
    /// <param name="currencyId">ID da moeda</param>
    /// <param name="amount">Quantidade</param>
    /// <returns>Se a remoção foi bem sucedida</returns>
    public bool RemoveCurrency(string currencyId, int amount)
    {
        if (string.IsNullOrEmpty(currencyId) || amount <= 0) return false;

        if (currencyDictionary.TryGetValue(currencyId, out Currency currency))
        {
            if (currency.amount >= amount)
            {
                currency.amount -= amount;
                SaveCurrencies();

                // Notifica o sistema de eventos
                if (currencyId == "coins" && EventManager.Instance != null)
                {
                    EventManager.Instance.TriggerPlayerCoinsChanged(currency.amount);
                }

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Verifica se tem moedas suficientes
    /// </summary>
    /// <param name="currencyId">ID da moeda</param>
    /// <param name="amount">Quantidade</param>
    public bool HasEnoughCurrency(string currencyId, int amount)
    {
        if (string.IsNullOrEmpty(currencyId) || amount <= 0) return false;

        return currencyDictionary.TryGetValue(currencyId, out Currency currency) && 
               currency.amount >= amount;
    }

    /// <summary>
    /// Retorna a quantidade de uma moeda
    /// </summary>
    /// <param name="currencyId">ID da moeda</param>
    public int GetCurrencyAmount(string currencyId)
    {
        if (string.IsNullOrEmpty(currencyId)) return 0;

        return currencyDictionary.TryGetValue(currencyId, out Currency currency) ? 
               currency.amount : 0;
    }

    /// <summary>
    /// Retorna todas as moedas
    /// </summary>
    public List<Currency> GetAllCurrencies()
    {
        return new List<Currency>(currencies);
    }

    /// <summary>
    /// Reseta todas as moedas
    /// </summary>
    public void ResetAllCurrencies()
    {
        foreach (Currency currency in currencies)
        {
            if (currency != null)
            {
                currency.amount = startingCoins;
            }
        }
        SaveCurrencies();
    }

    /// <summary>
    /// Adiciona uma nova moeda
    /// </summary>
    /// <param name="currency">Nova moeda</param>
    public void AddCurrencyType(Currency currency)
    {
        if (currency == null || string.IsNullOrEmpty(currency.id)) return;

        if (!currencyDictionary.ContainsKey(currency.id))
        {
            currencies.Add(currency);
            currencyDictionary[currency.id] = currency;
            SaveCurrencies();
        }
    }

    /// <summary>
    /// Remove um tipo de moeda
    /// </summary>
    /// <param name="currencyId">ID da moeda</param>
    public void RemoveCurrencyType(string currencyId)
    {
        if (string.IsNullOrEmpty(currencyId)) return;

        Currency currency = currencies.Find(c => c != null && c.id == currencyId);
        if (currency != null)
        {
            currencies.Remove(currency);
            currencyDictionary.Remove(currencyId);
            SaveCurrencies();
        }
    }
} 