using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomStremar : MonoBehaviour
{
    public string targetScene; // Nome da cena para carregar (Sala 2)

    private Collider2D playerCollider; // Refer�ncia ao collider do jogador
    private bool isSceneLoaded = false; // Verifica se a cena est� carregada

    private void Start()
    {
        // Localiza o jogador pela tag
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == playerCollider && !isSceneLoaded)
        {
            // Antes de carregar a cena, pr�-carregue a Sala 2
            StartCoroutine(PreloadAndLoadScene());
        }
    }

    private IEnumerator PreloadAndLoadScene()
    {
        isSceneLoaded = true;

        // Inicia o fade-out e espera ele terminar antes de continuar
        yield return StartCoroutine(ScreenFader.Instance.FadeOut());

        // Carrega a cena de forma ass�ncrona (Sala 2)
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
        loadOperation.allowSceneActivation = false; // N�o ativa a cena imediatamente

        // Aguarda at� que a cena esteja quase carregada
        while (loadOperation.progress < 0.9f)
        {
            yield return null; // Espera um frame antes de continuar
        }

        // A cena j� est� carregada, mas ainda N�O ativamos ela
        yield return new WaitForSeconds(0.2f); // Pequena espera extra

        // Teleportamos o jogador para o novo local **antes de ativar a cena**
        StartRoom startRoom = FindObjectOfType<StartRoom>();
        if (startRoom != null)
        {
            yield return StartCoroutine(startRoom.Teleport(SceneManager.GetSceneByName(targetScene)));
        }

        // Agora a cena pode ser ativada (o jogador j� estar� na posi��o correta)
        loadOperation.allowSceneActivation = true;
    }
}
