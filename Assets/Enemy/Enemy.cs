using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 3;

    public void TakeDamage(int damage)
    {
        Debug.Log("Dano");
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Lógica de morte do inimigo
        Destroy(gameObject); // Destruir o objeto inimigo
    }
}
