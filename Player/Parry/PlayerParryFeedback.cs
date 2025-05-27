using System.Collections;
using UnityEngine;

public class PlayerParryFeedback : MonoBehaviour
{
    [Header("Warning Circle Settings")]
    [SerializeField] private SpriteRenderer warningCircle;
    [SerializeField] private float warningCircleSize = 3f;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private float warningAlpha = 0.5f;
    [SerializeField] private float warningPulseSpeed = 2f;
    [SerializeField] private bool enableWarningPulse = true;

    [Header("Parry Circle Settings")]
    [SerializeField] private SpriteRenderer parryCircle;
    [SerializeField] private float parryCircleSize = 2f;
    [SerializeField] private Color parryActiveColor = Color.green;
    [SerializeField] private Color parryFailedColor = Color.red;
    [SerializeField] private Color parrySuccessColor = Color.cyan;
    [SerializeField] private Color perfectParryColor = Color.magenta;
    [SerializeField] private float parryAlpha = 0.7f;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.1f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float successFlashDuration = 0.5f;
    [SerializeField] private float failedFlashDuration = 0.8f;
    [SerializeField] private float scaleUpAmount = 1.2f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1);

    [Header("Perfect Parry Effects")]
    [SerializeField] private bool enablePerfectParryBurst = true;
    [SerializeField] private int burstParticleCount = 20;
    [SerializeField] private float burstRadius = 1f;
    [SerializeField] private Color burstColor = Color.white;

    [Header("Shake Effects")]
    [SerializeField] private bool enableScreenShake = true;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeDuration = 0.2f;

    // Componentes privados
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private Coroutine currentFeedbackCoroutine;
    private Coroutine warningPulseCoroutine;
    private Coroutine shakeCoroutine;

    // Estado atual
    private FeedbackState currentState = FeedbackState.Hidden;
    private float currentWarningFill = 0f;

    // Enum para estados do feedback
    public enum FeedbackState
    {
        Hidden,
        Warning,
        ParryActive,
        ParrySuccess,
        ParryFailed,
        PerfectParry
    }

    // Constantes
    private const int CIRCLE_TEXTURE_RESOLUTION = 512;
    private const string WARNING_CIRCLE_NAME = "WarningCircle";
    private const string PARRY_CIRCLE_NAME = "ParryCircle";

    #region Unity Lifecycle

    private void Start()
    {
        InitializeComponents();
        CreateFeedbackCircles();
        InitializeState();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        CleanupTextures();
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.localPosition;
        }
    }

    private void CreateFeedbackCircles()
    {
        CreateWarningCircle();
        CreateParryCircle();
    }

    private void CreateWarningCircle()
    {
        if (warningCircle == null)
        {
            GameObject warningObj = new GameObject(WARNING_CIRCLE_NAME);
            warningObj.transform.SetParent(transform);
            warningObj.transform.localPosition = Vector3.zero;
            warningObj.transform.localRotation = Quaternion.identity;
            
            warningCircle = warningObj.AddComponent<SpriteRenderer>();
            warningCircle.sprite = CreateCircleSprite(true);
            warningCircle.transform.localScale = Vector3.one * warningCircleSize;
            warningCircle.sortingOrder = 1;
            warningCircle.sortingLayerName = "UI"; // Ajuste conforme sua configuração
        }
    }

    private void CreateParryCircle()
    {
        if (parryCircle == null)
        {
            GameObject parryObj = new GameObject(PARRY_CIRCLE_NAME);
            parryObj.transform.SetParent(transform);
            parryObj.transform.localPosition = Vector3.zero;
            parryObj.transform.localRotation = Quaternion.identity;
            
            parryCircle = parryObj.AddComponent<SpriteRenderer>();
            parryCircle.sprite = CreateCircleSprite(false);
            parryCircle.transform.localScale = Vector3.one * parryCircleSize;
            parryCircle.sortingOrder = 2;
            parryCircle.sortingLayerName = "UI"; // Ajuste conforme sua configuração
        }
    }

    private void InitializeState()
    {
        HideParryWindow();
        currentState = FeedbackState.Hidden;
    }

    #endregion

    #region Sprite Creation

    private Sprite CreateCircleSprite(bool isOutline)
    {
        Texture2D texture = new Texture2D(CIRCLE_TEXTURE_RESOLUTION, CIRCLE_TEXTURE_RESOLUTION);
        Vector2 center = new Vector2(CIRCLE_TEXTURE_RESOLUTION / 2f, CIRCLE_TEXTURE_RESOLUTION / 2f);
        float outerRadius = CIRCLE_TEXTURE_RESOLUTION / 2f - 2f;
        float innerRadius = isOutline ? outerRadius - 8f : 0f; // Outline de 8 pixels

        // Usar cores com anti-aliasing
        for (int x = 0; x < CIRCLE_TEXTURE_RESOLUTION; x++)
        {
            for (int y = 0; y < CIRCLE_TEXTURE_RESOLUTION; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = CalculatePixelAlpha(distance, innerRadius, outerRadius);
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, 
            new Rect(0, 0, CIRCLE_TEXTURE_RESOLUTION, CIRCLE_TEXTURE_RESOLUTION), 
            new Vector2(0.5f, 0.5f));
    }

    private float CalculatePixelAlpha(float distance, float innerRadius, float outerRadius)
    {
        if (innerRadius == 0f)
        {
            // Círculo sólido com anti-aliasing
            return distance <= outerRadius ? 
                Mathf.Clamp01(outerRadius - distance + 1f) : 0f;
        }
        else
        {
            // Círculo outline com anti-aliasing
            if (distance < innerRadius - 1f || distance > outerRadius + 1f)
                return 0f;
            
            if (distance >= innerRadius + 1f && distance <= outerRadius - 1f)
                return 1f;
            
            // Anti-aliasing nas bordas
            if (distance < innerRadius + 1f)
                return Mathf.Clamp01(distance - innerRadius + 1f);
            else
                return Mathf.Clamp01(outerRadius - distance + 1f);
        }
    }

    #endregion

    #region Public Interface

    public void UpdateWarningFill(float fillAmount)
    {
        currentWarningFill = fillAmount;
        
        if (warningCircle != null)
        {
            bool shouldShow = fillAmount > 0.01f;
            
            if (shouldShow && currentState != FeedbackState.Warning)
            {
                ShowWarningState();
            }
            else if (!shouldShow && currentState == FeedbackState.Warning)
            {
                HideWarningState();
            }

            if (shouldShow)
            {
                UpdateWarningVisuals(fillAmount);
            }
        }
    }

    public void ShowParryActive()
    {
        if (currentState == FeedbackState.ParryActive) return;
        
        StopCurrentFeedback();
        currentState = FeedbackState.ParryActive;
        currentFeedbackCoroutine = StartCoroutine(ShowParryActiveCoroutine());
    }

    public void ShowParryFailed()
    {
        if (currentState == FeedbackState.ParryFailed) return;
        
        StopCurrentFeedback();
        currentState = FeedbackState.ParryFailed;
        currentFeedbackCoroutine = StartCoroutine(ShowParryFailedCoroutine());
    }

    public void ShowSuccess()
    {
        if (currentState == FeedbackState.ParrySuccess) return;
        
        StopCurrentFeedback();
        currentState = FeedbackState.ParrySuccess;
        currentFeedbackCoroutine = StartCoroutine(ShowParrySuccessCoroutine());
        
        if (enableScreenShake)
        {
            StartScreenShake(shakeIntensity * 0.5f, shakeDuration * 0.5f);
        }
    }

    public void ShowPerfectParrySuccess()
    {
        if (currentState == FeedbackState.PerfectParry) return;
        
        StopCurrentFeedback();
        currentState = FeedbackState.PerfectParry;
        currentFeedbackCoroutine = StartCoroutine(ShowPerfectParryCoroutine());
        
        if (enableScreenShake)
        {
            StartScreenShake(shakeIntensity, shakeDuration);
        }
    }

    public void HideParryWindow()
    {
        StopCurrentFeedback();
        currentState = FeedbackState.Hidden;
        
        if (parryCircle != null)
        {
            parryCircle.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Warning System

    private void ShowWarningState()
    {
        currentState = FeedbackState.Warning;
        
        if (warningCircle != null)
        {
            warningCircle.gameObject.SetActive(true);
            
            if (enableWarningPulse && warningPulseCoroutine == null)
            {
                warningPulseCoroutine = StartCoroutine(WarningPulseCoroutine());
            }
        }
    }

    private void HideWarningState()
    {
        if (warningPulseCoroutine != null)
        {
            StopCoroutine(warningPulseCoroutine);
            warningPulseCoroutine = null;
        }

        if (warningCircle != null && warningCircle.gameObject.activeSelf)
        {
            StartCoroutine(FadeOutWarning());
        }
    }

    private void UpdateWarningVisuals(float fillAmount)
    {
        if (warningCircle == null) return;

        Color color = warningColor;
        color.a = fillAmount * warningAlpha;
        
        // Adiciona pulsação se habilitada
        if (enableWarningPulse)
        {
            float pulse = 1f + Mathf.Sin(Time.time * warningPulseSpeed) * 0.2f;
            color.a *= pulse;
        }
        
        warningCircle.color = color;
    }

    private IEnumerator WarningPulseCoroutine()
    {
        while (currentState == FeedbackState.Warning)
        {
            float pulse = 1f + Mathf.Sin(Time.time * warningPulseSpeed) * 0.1f;
            if (warningCircle != null)
            {
                warningCircle.transform.localScale = Vector3.one * warningCircleSize * pulse;
            }
            yield return null;
        }
        
        // Restaura escala original
        if (warningCircle != null)
        {
            warningCircle.transform.localScale = Vector3.one * warningCircleSize;
        }
    }

    private IEnumerator FadeOutWarning()
    {
        if (warningCircle == null) yield break;

        Color startColor = warningCircle.color;
        Color endColor = startColor;
        endColor.a = 0f;

        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            
            if (warningCircle != null)
            {
                warningCircle.color = Color.Lerp(startColor, endColor, t);
            }
            
            yield return null;
        }

        if (warningCircle != null)
        {
            warningCircle.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Parry Feedback Coroutines

    private IEnumerator ShowParryActiveCoroutine()
    {
        if (parryCircle == null) yield break;

        parryCircle.gameObject.SetActive(true);
        
        // Fade in rápido
        yield return StartCoroutine(FadeInParryCircle(parryActiveColor, fadeInDuration));
        
        // Pequena animação de escala
        yield return StartCoroutine(ScaleAnimation(parryCircle.transform, 1f, scaleUpAmount * 0.1f, 0.1f));
    }

    private IEnumerator ShowParryFailedCoroutine()
    {
        if (parryCircle == null) yield break;

        parryCircle.gameObject.SetActive(true);
        
        // Flash vermelho mais intenso
        Color flashColor = parryFailedColor;
        flashColor.a = parryAlpha * 1.5f;
        
        yield return StartCoroutine(FlashEffect(flashColor, failedFlashDuration, 3));
        
        // Fade out
        yield return StartCoroutine(FadeOutParryCircle(fadeOutDuration));
    }

    private IEnumerator ShowParrySuccessCoroutine()
    {
        if (parryCircle == null) yield break;

        parryCircle.gameObject.SetActive(true);
        
        // Flash de sucesso
        yield return StartCoroutine(FlashEffect(parrySuccessColor, successFlashDuration, 2));
        
        // Animação de escala
        yield return StartCoroutine(ScaleAnimation(parryCircle.transform, 1f, scaleUpAmount, 0.3f));
        
        // Fade out
        yield return StartCoroutine(FadeOutParryCircle(fadeOutDuration));
    }

    private IEnumerator ShowPerfectParryCoroutine()
    {
        if (parryCircle == null) yield break;

        parryCircle.gameObject.SetActive(true);
        
        // Flash perfeito mais intenso
        Color perfectColor = perfectParryColor;
        perfectColor.a = parryAlpha * 2f;
        
        yield return StartCoroutine(FlashEffect(perfectColor, successFlashDuration * 0.7f, 4));
        
        // Burst de partículas se habilitado
        if (enablePerfectParryBurst)
        {
            StartCoroutine(PerfectParryBurstEffect());
        }
        
        // Animação de escala maior
        yield return StartCoroutine(ScaleAnimation(parryCircle.transform, 1f, scaleUpAmount * 1.5f, 0.4f));
        
        // Fade out mais lento
        yield return StartCoroutine(FadeOutParryCircle(fadeOutDuration * 1.5f));
    }

    #endregion

    #region Animation Helpers

    private IEnumerator FadeInParryCircle(Color targetColor, float duration)
    {
        if (parryCircle == null) yield break;

        Color startColor = targetColor;
        startColor.a = 0f;
        Color endColor = targetColor;
        endColor.a = parryAlpha;

        parryCircle.color = startColor;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            if (parryCircle != null)
            {
                parryCircle.color = Color.Lerp(startColor, endColor, t);
            }
            
            yield return null;
        }

        if (parryCircle != null)
        {
            parryCircle.color = endColor;
        }
    }

    private IEnumerator FadeOutParryCircle(float duration)
    {
        if (parryCircle == null) yield break;

        Color startColor = parryCircle.color;
        Color endColor = startColor;
        endColor.a = 0f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            if (parryCircle != null)
            {
                parryCircle.color = Color.Lerp(startColor, endColor, t);
            }
            
            yield return null;
        }

        if (parryCircle != null)
        {
            parryCircle.gameObject.SetActive(false);
        }
    }

    private IEnumerator FlashEffect(Color flashColor, float duration, int flashCount)
    {
        if (parryCircle == null) yield break;

        float flashInterval = duration / (flashCount * 2);
        
        for (int i = 0; i < flashCount; i++)
        {
            // Flash on
            parryCircle.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            
            // Flash off
            Color dimColor = flashColor;
            dimColor.a *= 0.3f;
            parryCircle.color = dimColor;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private IEnumerator ScaleAnimation(Transform target, float startScale, float endScale, float duration)
    {
        if (target == null) yield break;

        Vector3 startScaleVector = Vector3.one * startScale * parryCircleSize;
        Vector3 endScaleVector = Vector3.one * endScale * parryCircleSize;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = scaleCurve.Evaluate(t);
            
            if (target != null)
            {
                target.localScale = Vector3.Lerp(startScaleVector, endScaleVector, curveValue);
            }
            
            yield return null;
        }

        // Volta ao tamanho original
        if (target != null)
        {
            target.localScale = Vector3.one * parryCircleSize;
        }
    }

    private IEnumerator PerfectParryBurstEffect()
    {
        // Simula burst de partículas criando círculos temporários
        for (int i = 0; i < burstParticleCount; i++)
        {
            StartCoroutine(CreateBurstParticle());
            yield return new WaitForSeconds(0.02f); // Pequeno delay entre partículas
        }
    }

    private IEnumerator CreateBurstParticle()
    {
        GameObject particle = new GameObject("BurstParticle");
        particle.transform.SetParent(transform);
        
        SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite(false);
        sr.color = burstColor;
        sr.sortingOrder = 3;
        
        // Posição e direção randômica
        Vector2 direction = Random.insideUnitCircle.normalized;
        particle.transform.localPosition = Vector3.zero;
        particle.transform.localScale = Vector3.one * 0.1f;
        
        // Animação da partícula
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            if (particle != null)
            {
                // Movimento
                particle.transform.localPosition = direction * burstRadius * t;
                
                // Escala
                float scale = Mathf.Lerp(0.1f, 0.3f, t) * (1f - t);
                particle.transform.localScale = Vector3.one * scale;
                
                // Alpha
                Color color = sr.color;
                color.a = (1f - t) * 0.8f;
                sr.color = color;
            }
            
            yield return null;
        }
        
        if (particle != null)
        {
            Destroy(particle);
        }
    }

    #endregion

    #region Screen Shake

    private void StartScreenShake(float intensity, float duration)
    {
        if (!enableScreenShake || mainCamera == null) return;
        
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        
        shakeCoroutine = StartCoroutine(ScreenShakeCoroutine(intensity, duration));
    }

    private IEnumerator ScreenShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - (elapsed / duration);
            
            Vector3 randomOffset = Random.insideUnitCircle * intensity * t;
            if (mainCamera != null)
            {
                mainCamera.transform.localPosition = originalCameraPosition + randomOffset;
            }
            
            yield return null;
        }
        
        // Restaura posição original
        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = originalCameraPosition;
        }
        
        shakeCoroutine = null;
    }

    #endregion

    #region Utility Methods

    private void StopCurrentFeedback()
    {
        if (currentFeedbackCoroutine != null)
        {
            StopCoroutine(currentFeedbackCoroutine);
            currentFeedbackCoroutine = null;
        }
    }

    private void CleanupTextures()
    {
        // Apenas limpa as referências, não destrói as texturas
        if (warningCircle?.sprite != null)
        {
            warningCircle.sprite = null;
        }
        
        if (parryCircle?.sprite != null)
        {
            parryCircle.sprite = null;
        }
    }

    #endregion

    #region Public Properties (Para debug/inspeção)

    public FeedbackState CurrentState => currentState;
    public float CurrentWarningFill => currentWarningFill;
    public bool IsShowingFeedback => currentState != FeedbackState.Hidden;

    #endregion
}