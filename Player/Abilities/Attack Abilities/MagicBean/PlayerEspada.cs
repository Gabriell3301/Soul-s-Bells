using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDesintegrate : BaseAbility
{
    [Header("Referências")]
    public GameObject rayPrefab;
    public Transform firePoint; // Para modo clique rápido
    private List<GameObject> activeRays = new List<GameObject>();
    
    [Header("Estado")]
    private float holdTime;
    private bool isHolding;
    private bool showTargetingGizmo;
    private List<Transform> targetEnemies = new List<Transform>();
    private bool quickReleaseRegistered; // Nova flag para detectar releases rápidos
    
    [Header("Configurações")]
    public float quickReleaseThreshold = 0.15f; // Tempo máximo para considerar um quick release
    public float minHoldTimeForTargeting = 0.3f; // Tempo mínimo antes de começar targeting
    
    [Header("Visual Feedback")]
    public GameObject chargingEffect;
    private GameObject currentChargingEffect;   

    protected override void OnInitialize()
    {
        if (abilityData == null)
        {
            Debug.LogError("[Desintegrate] AbilityData é nulo!");
            enabled = false;
            return;
        }

        if (abilityData is DesintagreateData data)
        {
            rayPrefab = data.prefabRay;
            
            if (rayPrefab == null)
            {
                Debug.LogError("[Desintegrate] Prefab do raio não atribuído nos dados!");
            }
            
            // Se não tiver firePoint definido, criar um na posição do player
            if (firePoint == null)
            {
                GameObject firePointObj = new GameObject("FirePoint");
                firePointObj.transform.SetParent(transform);
                firePointObj.transform.localPosition = Vector2.right * 1f; // Posição à direita do player
                firePoint = firePointObj.transform;
            }
        }
        else
        {
            Debug.LogError("[Desintegrate] AbilityData não é do tipo DesintagreateData!");
            enabled = false;
        }
    }
    
    protected override void OnActivate()
    {
        isHolding = true;
        holdTime = 0f;
        showTargetingGizmo = true;
        targetEnemies.Clear();
        quickReleaseRegistered = false;
        
        StartCoroutine(HoldDetectionRoutine());
        StartChargingVisualEffect();
    }

    protected override void OnDeactivate()
    {
        isHolding = false;
        showTargetingGizmo = false;
        
        StopChargingVisualEffect();
        
        DesintagreateData data = (abilityData as DesintagreateData);
        
        // Se foi um quick release e ainda não registramos
        if (holdTime < data.holdThreshold && !quickReleaseRegistered)
        {
            quickReleaseRegistered = true;
            CreateForwardDamageRay();
        }
        else if (holdTime >= data.holdThreshold)
        {
            CreateInstantKillRays();
        }
    }

    private IEnumerator HoldDetectionRoutine()
    {
        DesintagreateData data = (abilityData as DesintagreateData);
        bool targetingStarted = false;
        
        while (isHolding)
        {
            holdTime += Time.deltaTime;
            
            // Atualizar efeito visual baseado no progresso
            float progress = Mathf.Clamp01(holdTime / data.holdThreshold);
            UpdateChargingEffect(progress);
            
            // Começar targeting apenas após um tempo mínimo
            if (holdTime >= minHoldTimeForTargeting && !targetingStarted)
            {
                targetingStarted = true;
            }
            
            // Buscar inimigos continuamente durante o carregamento (após tempo mínimo)
            if (targetingStarted && holdTime >= data.holdThreshold)
            {
                targetEnemies = FindEnemiesInRange();
            }
            
            yield return null;
        }
    }

    private void CreateInstantKillRays()
    {
        ClearRays();
        
        List<Transform> enemies = FindEnemiesInRange();
        
        if (enemies.Count == 0)
        {
            Debug.LogWarning("[Desintegrate] Nenhum inimigo encontrado - criando raio para frente");
            CreateForwardDamageRay();
            return;
        }

        int maxRays = GetMaxRayCount();
        int rayCount = Mathf.Min(enemies.Count, maxRays);

        for (int i = 0; i < rayCount; i++)
        {
            Transform enemy = enemies[i];
            
            GameObject rayObj = CreateInstantKillRayAt(enemy.position);
            if (rayObj != null)
            {
                activeRays.Add(rayObj);
            }
        }
    }

    private void CreateForwardDamageRay()
    {
        ClearRays();
        
        // Usar direção da câmera ou do firePoint
        Vector2 shootDirection = Camera.main != null ? 
            Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position : 
            firePoint.right;
        shootDirection.Normalize();
        
        // Posição à frente do player
        Vector2 rayPosition = (Vector2)firePoint.position + shootDirection * 2f;
        
        GameObject rayObj = CreateDamageRayAt(rayPosition, shootDirection);
        if (rayObj != null)
        {
            activeRays.Add(rayObj);
        }
    }

    private GameObject CreateInstantKillRayAt(Vector2 position)
    {
        if (rayPrefab == null)
        {
            Debug.LogError("[Desintegrate] Não é possível instanciar - rayPrefab é nulo!");
            return null;
        }

        // Instancia com a rotação original do prefab
        GameObject rayObj = Instantiate(rayPrefab, position, rayPrefab.transform.rotation);
        
        if (rayObj == null)
        {
            Debug.LogError("[Desintegrate] Falha ao instanciar raio!");
            return null;
        }

        // Rotaciona apenas o sprite em -45 graus
        SpriteRenderer spriteRenderer = rayObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, -45f);
        }

        // Usa o BoxCollider2D que já existe no prefab
        BoxCollider2D trigger = rayObj.GetComponent<BoxCollider2D>();
        if (trigger != null)
        {
            trigger.isTrigger = true;
            trigger.size = (abilityData as DesintagreateData).instantKillBoxSize;
        }
        
        // Adicionar componente que mata instantaneamente
        InstantKillTrigger killTrigger = rayObj.GetComponent<InstantKillTrigger>();
        if (killTrigger == null)
        {
            killTrigger = rayObj.AddComponent<InstantKillTrigger>();
        }
        
        killTrigger.Initialize();
        
        return rayObj;
    }

    private GameObject CreateDamageRayAt(Vector2 position, Vector2 direction)
    {
        if (rayPrefab == null)
        {
            Debug.LogError("[Desintegrate] Não é possível instanciar - rayPrefab é nulo!");
            return null;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject rayObj = Instantiate(rayPrefab, position, Quaternion.Euler(0, 0, angle));
        
        if (rayObj == null)
        {
            Debug.LogError("[Desintegrate] Falha ao instanciar raio!");
            return null;
        }

        // Escala menor para raio rápido
        rayObj.transform.localScale = Vector2.one * 0.5f;

        // Configurar trigger para causar dano
        BoxCollider2D trigger = rayObj.GetComponent<BoxCollider2D>();
        if (trigger == null)
        {
            trigger = rayObj.AddComponent<BoxCollider2D>();
        }
        
        trigger.isTrigger = true;
        trigger.size = (abilityData as DesintagreateData).damageBoxSize;
        
        // Adicionar componente que causa dano
        DamageTrigger damageTrigger = rayObj.GetComponent<DamageTrigger>();
        if (damageTrigger == null)
        {
            damageTrigger = rayObj.AddComponent<DamageTrigger>();
        }
        
        damageTrigger.Initialize(2); // 2 de dano
        
        // Destruir após alguns segundos
        Destroy(rayObj, 2f);
        
        return rayObj;
    }

    private List<Transform> FindEnemiesInRange()
    {
        List<Transform> enemies = new List<Transform>();
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        DesintagreateData data = (abilityData as DesintagreateData);

        foreach (GameObject enemy in allEnemies)
        {
            if (enemy == null) continue;
            
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            
            if (distance <= data.targetingRange)
            {
                enemies.Add(enemy.transform);
            }
        }

        // Ordenar por distância (mais próximos primeiro)
        enemies.Sort((a, b) => 
        {
            float distA = Vector2.Distance(transform.position, a.position);
            float distB = Vector2.Distance(transform.position, b.position);
            return distA.CompareTo(distB);
        });

        Debug.Log($"[Desintegrate] Encontrados {enemies.Count} inimigos no alcance");
        return enemies;
    }

    private int GetMaxRayCount()
    {
        int count = Mathf.Min(abilityData.level, 5); // Máximo 5 raios
        Debug.Log($"[Desintegrate] Número máximo de raios (nível {abilityData.level}): {count}");
        return count;
    }

    private void ClearRays()
    {
        Debug.Log($"[Desintegrate] Limpando {activeRays.Count} raios ativos");
        foreach (GameObject ray in activeRays)
        {
            if (ray != null) 
            {
                Destroy(ray);
            }
        }
        activeRays.Clear();
    }

    private void StartChargingVisualEffect()
    {
        if (chargingEffect != null && currentChargingEffect == null)
        {
            currentChargingEffect = Instantiate(chargingEffect, transform.position, Quaternion.identity);
            currentChargingEffect.transform.SetParent(transform);
        }
    }

    private void UpdateChargingEffect(float progress)
    {
        if (currentChargingEffect != null)
        {
            float scale = 0.5f + (progress * 1.5f);
            currentChargingEffect.transform.localScale = Vector2.one * scale;
            
            // Mudar cor baseado no progresso
            SpriteRenderer renderer = currentChargingEffect.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = Color.Lerp(Color.white, Color.red, progress);
                renderer.color = color;
            }
        }
    }

    private void StopChargingVisualEffect()
    {
        if (currentChargingEffect != null)
        {
            Destroy(currentChargingEffect);
            currentChargingEffect = null;
        }
    }

    protected override bool CanActivateCustom()
    {
        return !isHolding && activeRays.Count == 0;
    }

    // Teste - remover depois
    private void Update()
    {
        // Sistema de input melhorado
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            Activate();
        }
        
        if (Keyboard.current.kKey.wasReleasedThisFrame)
        {
            // Se foi um release rápido, marcar como quick release
            if (holdTime < quickReleaseThreshold)
            {
                quickReleaseRegistered = true;
            }
            Deactivate();
        }
    }

    private void OnDrawGizmos()
    {
        if (!showTargetingGizmo || abilityData == null) return;
        
        DesintagreateData data = (abilityData as DesintagreateData);
        if (data == null) return;
        
        // Raio de targeting
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.targetingRange);
        
        // Inimigos alvo
        foreach (Transform enemy in targetEnemies)
        {
            if (enemy != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, enemy.position);
                Gizmos.DrawWireCube(enemy.position, new Vector2(3f, 3f));
            }
        }
        
        // FirePoint e direção para raio rápido
        if (firePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(firePoint.position, 0.2f);
            
            Vector2 direction = Camera.main != null ? Camera.main.transform.right : firePoint.right;
            Vector2 rayPos = (Vector2)firePoint.position + direction * 5f;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(firePoint.position, direction * 5f);
            Gizmos.DrawWireCube(rayPos, new Vector2(2f, 8f));
        }
        
        // Indicador de progresso do carregamento
        if (isHolding)
        {
            DesintagreateData gizmoData = (abilityData as DesintagreateData);
            float progress = Mathf.Clamp01(holdTime / gizmoData.holdThreshold);
            Gizmos.color = Color.Lerp(Color.white, Color.red, progress);
            Gizmos.DrawWireSphere((Vector2)transform.position + Vector2.up * 1f, 0.5f + progress * 1f);
        }
    }    
}

// Componente para matar instantaneamente
public class InstantKillTrigger : MonoBehaviour
{
    private bool hasTriggered = false;
    private float animationDuration = 0.5f; // Duração da animação em segundos
    
    public void Initialize()
    {
        hasTriggered = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        
        if (other.CompareTag("Enemy"))
        {
            Enemy Enemy = other.GetComponent<Enemy>();
            if (Enemy != null)
            {
                Enemy.TakeDamage(99999);
            }
            else
            {
                Destroy(other.gameObject);
            }
            
            hasTriggered = true;
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(animationDuration);
        Destroy(gameObject);
    }
}

// Componente para causar dano
public class DamageTrigger : MonoBehaviour
{
    private int damage;
    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    private float animationDuration = 0.3f; // Duração da animação em segundos
    
    public void Initialize(int damageAmount)
    {
        damage = damageAmount;
        damagedEnemies.Clear();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !damagedEnemies.Contains(other.gameObject))
        {
            Enemy Enemy = other.GetComponent<Enemy>();
            if (Enemy != null)
            {
                Enemy.TakeDamage(damage);
            }
            
            damagedEnemies.Add(other.gameObject);
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(animationDuration);
        Destroy(gameObject);
    }
}