using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia todos os efeitos sonoros e músicas do jogo.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } // Instância singleton

    [System.Serializable]
    public class Sound
    {
        public string name; // Nome do som
        public AudioClip clip; // Clip de áudio
        [Range(0f, 1f)]
        public float volume = 1f; // Volume do som
        [Range(0.1f, 3f)]
        public float pitch = 1f; // Pitch do som
        public bool loop = false; // Se o som deve repetir
        [HideInInspector]
        public AudioSource source; // Componente de áudio
    }

    [Header("Configurações de Áudio")]
    [SerializeField] private Sound[] sounds; // Array de sons
    [SerializeField] private Sound[] music; // Array de músicas
    [SerializeField] private float fadeSpeed = 1f; // Velocidade do fade

    private AudioSource currentMusic; // Música atual tocando

    /// <summary>
    /// Inicializa o singleton e configura os componentes de áudio
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Configura os componentes de áudio para efeitos sonoros
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        // Configura os componentes de áudio para músicas
        foreach (Sound m in music)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.clip;
            m.source.volume = m.volume;
            m.source.pitch = m.pitch;
            m.source.loop = m.loop;
        }
    }

    /// <summary>
    /// Toca um efeito sonoro pelo nome
    /// </summary>
    /// <param name="name">Nome do som</param>
    public void PlaySound(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Som " + name + " não encontrado!");
            return;
        }
        s.source.Play();
    }

    /// <summary>
    /// Toca uma música pelo nome
    /// </summary>
    /// <param name="name">Nome da música</param>
    public void PlayMusic(string name)
    {
        Sound m = System.Array.Find(music, song => song.name == name);
        if (m == null)
        {
            Debug.LogWarning("Música " + name + " não encontrada!");
            return;
        }

        if (currentMusic != null)
        {
            StartCoroutine(FadeOut(currentMusic));
        }

        currentMusic = m.source;
        m.source.Play();
        StartCoroutine(FadeIn(m.source));
    }

    /// <summary>
    /// Para a música atual
    /// </summary>
    public void StopMusic()
    {
        if (currentMusic != null)
        {
            StartCoroutine(FadeOut(currentMusic));
            currentMusic = null;
        }
    }

    /// <summary>
    /// Aplica fade in em uma fonte de áudio
    /// </summary>
    private IEnumerator FadeIn(AudioSource source)
    {
        float targetVolume = source.volume;
        source.volume = 0;
        source.Play();

        while (source.volume < targetVolume)
        {
            source.volume += fadeSpeed * Time.deltaTime;
            yield return null;
        }

        source.volume = targetVolume;
    }

    /// <summary>
    /// Aplica fade out em uma fonte de áudio
    /// </summary>
    private IEnumerator FadeOut(AudioSource source)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= fadeSpeed * Time.deltaTime;
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }
} 