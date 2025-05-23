using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SenceTrasitionManange : MonoBehaviour
{
    public string nextSceneName;

    public void SceneTransition(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Carrega a nova cena
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // Ajusta a posição do jogador para a nova cena
        PositionPlayerInNewScene();

        // Descarrega a cena antiga
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentScene);

        // Define a nova cena como ativa
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }

    private void PositionPlayerInNewScene()
    {
        // Altere a posição do jogador com base no spawn point da nova cena
        Transform spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = spawnPoint.position;
    }
}
