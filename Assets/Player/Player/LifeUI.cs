using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeUI : MonoBehaviour
{
    public GameObject hitPrefab; // Prefab do quadrado da vida
    public Transform container; // Onde os quadrados serão criados
    private List<GameObject> hitIcons = new List<GameObject>(); // Lista dos quadrados

    private int maxHits;

    public void Initialize(int totalHits)
    {
        maxHits = totalHits;
        UpdateUI(totalHits);
    }

    public void UpdateUI(int currentHits)
    {
        // Remove todos os ícones antigos
        foreach (GameObject hit in hitIcons)
        {
            Destroy(hit);
        }
        hitIcons.Clear();

        // Cria novos ícones baseados na vida atual
        for (int i = 0; i < currentHits; i++)
        {
            GameObject newHit = Instantiate(hitPrefab, container);
            hitIcons.Add(newHit);
        }
    }
}
