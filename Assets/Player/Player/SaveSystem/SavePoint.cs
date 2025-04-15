using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private bool playerInRange;
    private bool canSave;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.UpArrow) && !canSave)
        {
            canSave = true;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var abilitySystem = player.GetComponent<PlayerAbilitySystem>();
                if (abilitySystem != null)
                {
                    //Debug.Log($"Numero de moedas para salvar: {GameManager.Instance.Moedas}");
                    //string currentRoom = GameManager.Instance.CurrentRoomName; // <-- você precisa expor isso no GameManager
                    //int moedas = GameManager.Instance.Moedas;

                    //GameManager.Instance.SaveGame(currentRoom, moedas, abilitySystem);

                    //Debug.Log("💾 Jogo salvo com sucesso!");

                    //// ⚠️ Para teste: carrega logo após salvar
                    GameManager.Instance.LoadGame();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Pressione ↑ para salvar.");
            // Aqui você pode ativar uma UI na tela
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            canSave = false;
            // Aqui você pode ocultar a UI
        }
    }
}
