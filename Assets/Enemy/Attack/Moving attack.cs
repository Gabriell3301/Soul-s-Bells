using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movingattack : EnemyAttack
{
    Rigidbody2D rb;
    public float velocityProject = 10f;
    private Transform playerPosition;
    // Start is called before the first frame update
    void Start()
    {
        playerPosition = FindObjectOfType<PlayerMovement>().transform;
        rb = GetComponent<Rigidbody2D>();

        // Calcula a direção para o jogador
        Vector2 direction = (playerPosition.position - transform.position).normalized;

        // Define a velocidade da bola para seguir o jogador
        rb.velocity = direction * velocityProject;
        StartCoroutine(StartCounter());
    }
    private IEnumerator StartCounter()
    {
        yield return new WaitForSeconds(3);
        Destroythis();
    }
}
