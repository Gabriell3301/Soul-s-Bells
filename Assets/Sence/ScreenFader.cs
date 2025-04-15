using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance; // Singleton para acesso global

    public Image fadeImage; // Arraste a Image preta no Inspector
    public float fadeDuration = 1.0f; // Tempo do fade

    private void Awake()
    {
        // Garante que só existe um ScreenFader
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantém ao mudar de cena
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, timer / fadeDuration));
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, timer / fadeDuration));
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }
}
