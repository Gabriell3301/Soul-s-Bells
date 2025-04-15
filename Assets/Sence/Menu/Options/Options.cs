using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject controller;
    [SerializeField] private GameObject language;
    //Button's para navegar nas op��es
    [SerializeField] private Button buttonController;
    [SerializeField] private Button buttonLanguage;
    private void Awake()
    {
        options.SetActive(true);
        buttonController.onClick.AddListener(Controller);
        buttonLanguage.onClick.AddListener(Language);
    }

    private void Controller()
    {
        Debug.Log("Abrindo Op��es de controle!");
        controller.SetActive(true);
        options.SetActive(false);
    }
    private void Language()
    {
        Debug.Log("Abrindo Op��es de linguagem!");
        language.SetActive(true);
        options.SetActive(false);
    }
}
