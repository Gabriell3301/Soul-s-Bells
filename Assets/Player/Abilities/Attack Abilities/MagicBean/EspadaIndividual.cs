using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EspadaIndividual : MonoBehaviour
{
    [Header("Configurações de Dano")]
    public int damage = 10;
    public float damageRadius = 2f;
    public float knockbackForce = 5f;
    public DamageType damageType = DamageType.Slashing;

    [Header("Configurações de Movimento")]
    public float fallSpeed = 15f;
    public float trackingSpeed = 5f;

    [Header("Efeitos")]
    public GameObject impactEffect;
    public AudioClip impactSound;
    public Color gizmoColor = Color.red;

    [Header("Filtro de Inimigos")]
    [Tooltip("Layer dos inimigos")]
    public LayerMask enemyLayer;
    
    [Header("Estado")]
    [SerializeField] private bool isTracking = false;
    [SerializeField] private bool isFalling = false;
    [SerializeField] private bool damageDealt = false;

    private Transform target;
    private Vector3 originalPosition;
    private Collider2D swordCollider;
    private AudioSource audioSource;

    private void Awake()
    {
        swordCollider = GetComponent<Collider2D>();
        if (swordCollider == null)
        {
            swordCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        swordCollider.isTrigger = true;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        originalPosition = transform.position;
    }

    public void Initialize(int dmg, float radius, float fSpeed, float tSpeed, LayerMask layer)
    {
        damage = dmg;
        damageRadius = radius;
        fallSpeed = fSpeed;
        trackingSpeed = tSpeed;
        enemyLayer = layer;
        swordCollider.enabled = true;
    }

    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
        isTracking = true;
        isFalling = false;
    }

    public void StartFalling()
    {
        isTracking = false;
        isFalling = true;
    }

    private void Update()
    {
        if (isTracking && target != null)
        {
            TrackTarget();
        }
        else if (isFalling)
        {
            FallToGround();
        }
    }

    private void TrackTarget()
    {
        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y + (originalPosition.y - transform.position.y),
            transform.position.z
        );
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, trackingSpeed * Time.deltaTime);
        
        if (Vector2.Distance(transform.position, target.position) < 0.5f && !damageDealt)
        {
            DealDamage(target.position);
        }
    }

    private void FallToGround()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
        
        if (transform.position.y <= 0.5f && !damageDealt)
        {
            DealDamage(transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damageDealt) return;
        
        if (IsInLayerMask(other.gameObject, enemyLayer))
        {
            DealDamage(other.ClosestPoint(transform.position));
            Destroy(gameObject);
        }
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return (layerMask.value & (1 << obj.layer)) != 0;
    }

    private void DealDamage(Vector2 impactPoint)
    {
        if (damageDealt) return;
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(impactPoint, damageRadius, enemyLayer);
        bool hitConfirmed = false;

        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                enemyComponent.TakeDamage(damage, damageType, transform.position, knockbackForce);
                hitConfirmed = true;
            }
        }

        if (hitConfirmed)
        {
            PlayImpactEffects(impactPoint);
            damageDealt = true;
            swordCollider.enabled = false;
            Destroy(gameObject, 0.5f);
        }
    }

    private void PlayImpactEffects(Vector2 position)
    {
        if (impactEffect != null)
        {
            Instantiate(impactEffect, position, Quaternion.identity);
        }

        if (impactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(impactSound);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
        
        if (isTracking && target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}