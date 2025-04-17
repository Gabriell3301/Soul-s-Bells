using UnityEngine;

public class MeleeEnemy : Enemy
{
    [Header("Ataque Melee")]
    public int attackDamage = 1;
    public float attackCooldown = 2f;
    public float attackDuration = 0.5f;
    public float attackRange = 2f;
    public float attackRadius = 0.5f; // Raio do alcance de ataque
    
    [Header("References")]
    public GameObject attackEffectPrefab;
    
    private bool canAttack = true;
    private bool isAttacking = false;
    
    private MeleeAttack meleeAttack;
    
    protected override void Start()
    {
        base.Start();
        meleeAttack = GetComponent<MeleeAttack>();
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (chasingPlayer && meleeAttack != null)
        {
            meleeAttack.TryAttack();
        }
    }
    
    private void StartAttack()
    {
        if (!canAttack || isAttacking) return;
        
        isAttacking = true;
        canAttack = false;
        
        // Define a posição do centro do ataque
        Vector2 attackCenter = transform.position + Vector3.right * (isFacingRight ? attackRange : -attackRange);
        
        // Spawn attack effect
        if (attackEffectPrefab != null)
        {
            Instantiate(attackEffectPrefab, attackCenter, Quaternion.identity);
        }
        
        // Check for hits
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackCenter, attackRadius, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeHit(attackDamage);
            }
        }
        
        // Reset attack state after duration
        Invoke(nameof(EndAttack), attackDuration);
        // Reset cooldown
        Invoke(nameof(ResetAttackCooldown), attackCooldown);
    }
    
    private void EndAttack()
    {
        isAttacking = false;
    }
    
    private void ResetAttackCooldown()
    {
        canAttack = true;
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        // Desenha o alcance do ataque
        Vector2 attackCenter = transform.position + Vector3.right * (isFacingRight ? attackRange : -attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCenter, attackRadius);
    }
} 