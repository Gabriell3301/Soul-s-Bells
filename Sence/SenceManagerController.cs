using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SenceManagerController : MonoBehaviour
{
    public static SenceManagerController Instance;

    // Nome da cena atual e próxima
    private string currentScene;
    private string nextScene;

    // Referência ao jogador
    private GameObject player;

    private void Awake()
    {
        // Garantir que apenas um SceneManager exista
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Não destrói o SceneManager ao carregar novas cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Identificar a cena inicial
        currentScene = SceneManager.GetActiveScene().name;

        // Encontrar o jogador na cena
        player = GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// Transição para outra cena
    /// </summary>
    /// <param name="sceneName">Nome da próxima cena</param>
    /// <param name="spawnPosition">Posição de spawn do jogador na nova cena</param>
    public void TransitionToScene(string sceneName, Vector3 spawnPosition)
    {
        nextScene = sceneName;
        StartCoroutine(LoadSceneRoutine(sceneName, spawnPosition));
    }

    /// <summary>
    /// Carregar a próxima cena e descarregar a atual
    /// </summary>
    private IEnumerator LoadSceneRoutine(string sceneName, Vector3 spawnPosition)
    {
        // Carregar nova cena de forma assíncrona
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // Mover o jogador para a posição de spawn
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        player.transform.position = spawnPosition;

        // Descarregar a cena atual
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
        while (!unloadOperation.isDone)
        {
            yield return null;
        }

        // Atualizar a cena atual
        currentScene = sceneName;

        // Definir a nova cena como ativa
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

    }
}
