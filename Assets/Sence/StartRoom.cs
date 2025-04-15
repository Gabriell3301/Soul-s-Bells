using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartRoom : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(DelayedFadeIn());
    }
    private IEnumerator DelayedFadeIn()
    {
        yield return new WaitForSeconds(1f); // Pequeno delay para evitar cortes bruscos
        yield return StartCoroutine(ScreenFader.Instance.FadeIn());
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameObject.scene.name) // Certifica que está rodando na cena correta
        {
            GameManager.Instance.SetCurrentRoom(scene.name);

            StartCoroutine(WaitAndTeleport(scene));
            SceneManager.sceneLoaded -= OnSceneLoaded; // Remove o evento após o uso
        }
    }
    private IEnumerator WaitAndTeleport(Scene scene)
    {
        // Aguarda um frame extra após a cena carregar completamente
        yield return null;
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(Teleport(scene));
    }
    public IEnumerator Teleport(Scene scene)
    {
        yield return null;
        // Encontrar o jogador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Jogador não encontrado!");
            yield break;
        }

        // Procurar o SpawnPoint **apenas dentro da cena carregada**
        GameObject spawnPoint = FindSpawnPointInScene(scene);
        if (spawnPoint == null)
        {
            // This is a scam, this goes in and goes through this if (go to line: 53)
            yield break;
        }

        // Teleportar o jogador para o spawn point da nova cena
        // But even with the spawnpoint being null, the teleport works, I don't know until when
        player.transform.position = spawnPoint.transform.position;
        Debug.Log("Jogador teleportado para: " + spawnPoint.transform.position);
        // Espera um pequeno tempo para evitar transições abruptas
        yield return new WaitForSeconds(0.1f);

    }
    /// <summary>
    /// Find a game object with the tag "SpawnPoint" in the given scene.
    /// </summary>
    /// <param name="scene">Name of Scene</param>
    /// <returns>Game object with the tag "SpawnPoint"</returns>
    private GameObject FindSpawnPointInScene(Scene scene)
    {
        foreach (GameObject rootObj in scene.GetRootGameObjects())
        {
            Transform[] children = rootObj.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.CompareTag("SpawnPoint"))
                {
                    return child.gameObject;
                }
            }
        }
        return null;
    }
}
