using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScriptVision : MonoBehaviour
{
    public event Action<Collider2D> OnTriggerEntered;
    public event Action<Collider2D> OnTriggerExited;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Se o jogador entrar na vis�o
        {
            OnTriggerEntered?.Invoke(other); // Dispara o evento para o inimigo
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Se o jogador sair da vis�o
        {
            OnTriggerExited?.Invoke(other); // Dispara o evento de sa�da
        }
    }
}
