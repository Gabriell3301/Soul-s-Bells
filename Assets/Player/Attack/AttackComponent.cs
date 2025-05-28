using UnityEngine;

/// <summary>
/// Componente base para ataques do jogador.
/// </summary>
public abstract class AttackComponent : MonoBehaviour
{
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float knockbackForce = 5f;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected int hits = 0;

    public int Hits => hits;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            if (other.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
                hits++;
            }
        }
    }
} 