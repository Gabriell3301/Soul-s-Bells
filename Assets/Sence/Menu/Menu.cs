using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Função chamada ao clicar no botão "Jogar"
    public void PlayGame()
    {
        // Carrega a próxima cena (adicione a cena no Build Settings)
        // Se sua cena de jogo for a próxima na lista, pode usar "SceneManager.LoadScene(1);" por exemplo.
        SceneManager.LoadScene(1); // Substitua pelo nome da cena do jogo
    }

    // Função chamada ao clicar no botão "Sair"
    public void QuitGame()
    {
        Debug.Log("Sair do jogo!"); // Isso só aparecerá no editor
        Application.Quit(); // Fecha o jogo
    }
}
