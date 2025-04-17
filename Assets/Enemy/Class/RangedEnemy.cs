using UnityEngine;
using System.Collections;

public class RangedEnemy : Enemy
{
    [Header("Ataque Ranged")]
    public float bulletSpeed = 10f;
    public float bulletDamage = 1f;
    
    private FireAttack fireAttack;
    
    protected override void Start()
    {
        base.Start();
        fireAttack = GetComponent<FireAttack>();
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (chasingPlayer && fireAttack != null)
        {
            fireAttack.TryFire();
        }
    }
    
    protected override void ChasePlayer()
    {
        float stopMargin = 0.4f;
        if (player == null) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float directionX = Mathf.Sign(player.position.x - transform.position.x);
        
        if (distanceToPlayer > stoppingDistance + stopMargin)
        {
            // Persegue o jogador até uma certa distância
            safeStop = false;
            rb.velocity = new Vector2(directionX * chaseSpeed, rb.velocity.y);
        }
        else if (distanceToPlayer < retreatDistance - stopMargin)
        {
            // Se estiver muito perto, se afasta
            safeStop = false;
            rb.velocity = new Vector2(-directionX * chaseSpeed, rb.velocity.y);
        }
        else
        {
            // Se estiver dentro do intervalo aceitável, para completamente
            if (!safeStop)
            {
                safeStop = true;
                rb.velocity = Vector2.zero;
            }
        }

        // Verifica se precisa dar flip
        if ((directionX > 0 && !isFacingRight) || (directionX < 0 && isFacingRight))
        {
            Flip();
        }
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }
}