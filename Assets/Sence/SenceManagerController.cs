using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SenceManagerController : MonoBehaviour
{
    public static SenceManagerController Instance;

    // Nome da cena atual e pr�xima
    private string currentScene;
    private string nextScene;

    // Refer�ncia ao jogador
    private GameObject player;

    private void Awake()
    {
        // Garantir que apenas um SceneManager exista
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // N�o destr�i o SceneManager ao carregar novas cenas
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
    /// Transi��o para outra cena
    /// </summary>
    /// <param name="sceneName">Nome da pr�xima cena</param>
    /// <param name="spawnPosition">Posi��o de spawn do jogador na nova cena</param>
    public void TransitionToScene(string sceneName, Vector3 spawnPosition)
    {
        nextScene = sceneName;
        StartCoroutine(LoadSceneRoutine(sceneName, spawnPosition));
    }

    /// <summary>
    /// Carregar a pr�xima cena e descarregar a atual
    /// </summary>
    private IEnumerator LoadSceneRoutine(string sceneName, Vector3 spawnPosition)
    {
        // Carregar nova cena de forma ass�ncrona
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // Mover o jogador para a posi��o de spawn
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
