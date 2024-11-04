using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Fun��o chamada ao clicar no bot�o "Jogar"
    public void PlayGame()
    {
        // Carrega a pr�xima cena (adicione a cena no Build Settings)
        // Se sua cena de jogo for a pr�xima na lista, pode usar "SceneManager.LoadScene(1);" por exemplo.
        SceneManager.LoadScene(1); // Substitua pelo nome da cena do jogo
    }

    // Fun��o chamada ao clicar no bot�o "Sair"
    public void QuitGame()
    {
        Debug.Log("Sair do jogo!"); // Isso s� aparecer� no editor
        Application.Quit(); // Fecha o jogo
    }
}
