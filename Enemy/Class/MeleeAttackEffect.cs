using UnityEngine;

public class MeleeAttackEffect : MonoBehaviour
{
    public float expandSpeed = 5f;
    public float fadeSpeed = 2f;
    public float maxScale = 2f;
    
    private SpriteRenderer spriteRenderer;
    private float currentAlpha = 1f;
    private Vector3 targetScale;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // Começa pequeno e expande
        transform.localScale = Vector3.one * 0.1f;
        targetScale = Vector3.one * maxScale;
        
        // Configura a cor inicial
        Color color = spriteRenderer.color;
        color.a = currentAlpha;
        spriteRenderer.color = color;
    }
    
    private void Update()
    {
        // Expande o efeito
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * expandSpeed);
        
        // Faz o efeito desaparecer gradualmente
        currentAlpha = Mathf.Lerp(currentAlpha, 0f, Time.deltaTime * fadeSpeed);
        Color color = spriteRenderer.color;
        color.a = currentAlpha;
        spriteRenderer.color = color;
        
        // Destrói quando ficar invisível
        if (currentAlpha < 0.01f)
        {
            Destroy(gameObject);
        }
    }
} 