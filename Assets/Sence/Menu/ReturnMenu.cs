using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private bool isPaused;
    private GlobalStates gStates;
    // Update is called once per frame
    private void Awake()
    {
        gStates = FindObjectOfType<GlobalStates>();
        if (gStates != null)
        {
        Debug.Log(gStates);
            gStates.SetPaused(false);
        }
        pauseMenu.SetActive(false);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gStates.isPaused)
            {
                pauseMenu.SetActive(true);
                gStates.SetPaused(true);
                Time.timeScale = 0;
            }
            else
            {
                Resume();
            }
        }
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        gStates.SetPaused(false);
        Time.timeScale = 1;
    }
    public void Restart()
    {
        pauseMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reinicia a cena atual
        Time.timeScale = 1;
    }
    public void ReturneMainMenu()
    {
        pauseMenu.SetActive(false);
        SceneManager.LoadScene(0); // Substitua pelo nome da sua cena de menu
        Time.timeScale = 1f; // Garante que o jogo n�o esteja pausado
    }
}
