using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttack : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab do projétil
    public Transform firePoint; // Ponto onde o projétil será disparado
    public float fireRate = 1.5f; // Tempo entre tiros
    private float nextFireTime = 0f;

    public LayerMask groundLayer;
    public float cliffDetectionDistance = 1f; // Distância para detectar penhasco
    private Enemy enemy;

    private void Start()
    {
        enemy = GetComponent<Enemy>(); // Obtém referência ao script do inimigo
    }

    public void TryFire()
    {
        if (enemy.IsPlayerDetected() && Time.time >= nextFireTime && !IsRetreating())
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private bool IsRetreating()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, enemy.GetPlayerPosition());
        return distanceToPlayer < enemy.retreatDistance;
    }

    private void Shoot()
    {
        Debug.Log("Inimigo disparou!");

        // Instancia o projétil e define a direção dele
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        EnemyAttack attack = projectile.GetComponent<EnemyAttack>();
        if (attack != null)
        {
            attack.Initialize(enemy);
        }
        else
        {
            Debug.Log("Attack not found");
        }
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float direction = transform.localScale.x > 0 ? -1f : 1f; // Determina a direção do tiro com base no lado que o inimigo está virado
            rb.velocity = new Vector2(direction * 10f, 0); // Ajuste a velocidade conforme necessário
        }
    }
}
