using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public static DropManager Instance;

    [SerializeField] private GameObject moedaPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnMoeda(Vector2 position, int quantidade = 1)
    {
        if (moedaPrefab == null)
        {
            Debug.LogError("Moeda prefab não está configurado no DropManager!");
            return;
        }

        for (int i = 0; i < quantidade; i++)
        {
            try
            {
                Instantiate(moedaPrefab, position, Quaternion.identity);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao spawnar moeda: {e.Message}");
            }
        }
    }
} 