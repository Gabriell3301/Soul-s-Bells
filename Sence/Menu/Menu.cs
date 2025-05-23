using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject menu;
    [SerializeField] private Button buttonPlay;
    [SerializeField] private Button buttonOptions;
    [SerializeField] private Button buttonExit;

    private void Awake()
    {
        buttonPlay.onClick.AddListener(PlayGame);
        buttonOptions.onClick.AddListener(Options);
        buttonExit.onClick.AddListener(QuitGame);
    }
    private void PlayGame()
    {
        // Carrega a pr�xima cena (adicione a cena no Build Settings)
        // Se sua cena de jogo for a pr�xima na lista, pode usar "SceneManager.LoadScene(1);" por exemplo.
        SceneManager.LoadScene(1); 
    }
    private void Options()
    {
        Debug.Log("Abrindo Op��es!");
        options.SetActive(true); // Ativa o Canvas
        menu.SetActive(false); // Desativa o Canvas do Menu

    }
    // Fun��o chamada ao clicar no bot�o "Sair"
    private void QuitGame()
    {
        Debug.Log("Sair do jogo!"); // Isso s� aparecer� no editor
        Application.Quit(); // Fecha o jogo
    }
}
