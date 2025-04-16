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
        for (int i = 0; i < quantidade; i++)
        {
            Debug.Log("Spawn de moedas na posição: " + position + " com quantidade: " + quantidade);
            Instantiate(moedaPrefab, position, Quaternion.identity);
        }
    }
}
