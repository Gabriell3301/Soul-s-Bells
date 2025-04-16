using UnityEngine;

public class PlayerParryFeedback : MonoBehaviour
{
    [Header("Warning Circle")]
    [SerializeField] private SpriteRenderer warningCircle;
    [SerializeField] private float warningCircleSize = 3f;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private float warningAlpha = 0.5f;

    [Header("Parry Circle")]
    [SerializeField] private SpriteRenderer parryCircle;
    [SerializeField] private float parryCircleSize = 2f;
    [SerializeField] private Color parryActiveColor = Color.green;
    [SerializeField] private Color parryFailedColor = Color.red;
    [SerializeField] private Color parrySuccessColor = Color.cyan;
    [SerializeField] private float parryAlpha = 0.7f;

    private void Start()
    {
        // Criar círculo de aviso se não existir
        if (warningCircle == null)
        {
            GameObject warningObj = new GameObject("WarningCircle");
            warningObj.transform.SetParent(transform);
            warningObj.transform.localPosition = Vector3.zero;
            warningCircle = warningObj.AddComponent<SpriteRenderer>();
            warningCircle.sprite = CreateCircleSprite();
            warningCircle.transform.localScale = Vector3.one * warningCircleSize;
            warningCircle.sortingOrder = 1;
        }

        // Criar círculo de parry se não existir
        if (parryCircle == null)
        {
            GameObject parryObj = new GameObject("ParryCircle");
            parryObj.transform.SetParent(transform);
            parryObj.transform.localPosition = Vector3.zero;
            parryCircle = parryObj.AddComponent<SpriteRenderer>();
            parryCircle.sprite = CreateCircleSprite();
            parryCircle.transform.localScale = Vector3.one * parryCircleSize;
            parryCircle.sortingOrder = 2;
        }

        // Inicializar estados
        HideParryWindow();
    }

    private Sprite CreateCircleSprite()
    {
        int resolution = 256;
        Texture2D texture = new Texture2D(resolution, resolution);
        Vector2 center = new Vector2(resolution / 2, resolution / 2);
        float radius = resolution / 2;

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = distance <= radius ? 1f : 0f;
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f));
    }

    public void UpdateWarningFill(float fillAmount)
    {
        if (warningCircle != null)
        {
            Color color = warningColor;
            color.a = fillAmount * warningAlpha;
            warningCircle.color = color;
        }
    }

    public void ShowParryActive()
    {
        if (parryCircle != null)
        {
            parryCircle.gameObject.SetActive(true);
            Color color = parryActiveColor;
            color.a = parryAlpha;
            parryCircle.color = color;
        }
    }

    public void ShowParryFailed()
    {
        if (parryCircle != null)
        {
            parryCircle.gameObject.SetActive(true);
            Color color = parryFailedColor;
            color.a = parryAlpha;
            parryCircle.color = color;
        }
    }

    public void ShowSuccess()
    {
        if (parryCircle != null)
        {
            parryCircle.gameObject.SetActive(true);
            Color color = parrySuccessColor;
            color.a = parryAlpha;
            parryCircle.color = color;
        }
    }

    public void HideParryWindow()
    {
        if (parryCircle != null)
        {
            Color color = parryCircle.color;
            color.a = 0f;
            parryCircle.color = color;
        }
    }
}
