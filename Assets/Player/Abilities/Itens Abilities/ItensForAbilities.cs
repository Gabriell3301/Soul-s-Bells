using System;
using UnityEngine;

public class ItensForAbilities : MonoBehaviour
{
    [SerializeField] private string abilityForAdd;
    GameObject player;
    Type abilityType;
    private void Start()
    {
        abilityType = Type.GetType(abilityForAdd);
        if (abilityType == null)
        {
            Debug.Log("Habilidade não encontrada");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (abilityType != null)
        {
            if (collision.CompareTag("Player"))
            {
                player = collision.gameObject;
                if (player.GetComponent(abilityType) == null)
                {
                    // Adiciona a habilidade ao jogador
                    player.AddComponent(abilityType);
                    Debug.Log("Player Pegou Dash");
                }
                Destroy(gameObject);
            }
        }
    }
}
