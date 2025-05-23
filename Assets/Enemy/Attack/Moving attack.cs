using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia o ataque móvel dos inimigos.
/// </summary>
public class MovingAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 1;
    private PlayerHealth playerH;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        if (other.CompareTag("Player"))
        {
            playerH = other.GetComponent<PlayerHealth>();
            if (playerH != null)
            {
                playerH.TakeDamage(attackDamage);
            }
            else
            {
                Debug.LogWarning("PlayerHealth não encontrado no objeto do jogador!");
            }
        }
    }
}
