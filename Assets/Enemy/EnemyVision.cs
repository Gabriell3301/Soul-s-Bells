using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float visionRange = 5f;
    public float visionAngle = 45f;
    private Enemy enemy;
    private EnemysMenager enemyManager;
    private Transform player;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        enemyManager = GetComponentInParent<EnemysMenager>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (PlayerInVision())
        {
            enemyManager.AlertEnemies(player);
        }
    }

    private bool PlayerInVision()
    {
        if (player == null) return false;

        Vector2 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);

        if (angleToPlayer < visionAngle / 2 && directionToPlayer.magnitude <= visionRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, visionRange);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
