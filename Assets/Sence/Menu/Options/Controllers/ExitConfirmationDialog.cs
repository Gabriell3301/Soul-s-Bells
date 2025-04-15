using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ExitConfirmationDialog : MonoBehaviour
{
    [Header("Referências UI")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private Button saveAndExitButton;
    [SerializeField] private Button exitWithoutSavingButton;
    [SerializeField] private Button cancelExitButton;

    // Eventos para comunicação com outros scripts
    public event Action OnSaveAndExit;
    public event Action OnExitWithoutSaving;
    public event Action OnCancelExit;

    // Texto padrão caso nenhum seja especificado
    private string defaultMessage = "Existem alterações não salvas. O que deseja fazer?";

    private void Awake()
    {
        // Verifica se todos os componentes estão referenciados
        if (confirmationPanel == null)
            confirmationPanel = this.gameObject;

        // Adiciona listeners aos botões
        if (saveAndExitButton != null)
            saveAndExitButton.onClick.AddListener(HandleSaveAndExit);

        if (exitWithoutSavingButton != null)
            exitWithoutSavingButton.onClick.AddListener(HandleExitWithoutSaving);

        if (cancelExitButton != null)
            cancelExitButton.onClick.AddListener(HandleCancelExit);

        // Esconde o painel inicialmente
        HidePanel();
    }

    /// <summary>
    /// Mostra o painel com uma mensagem personalizada
    /// </summary>
    /// <param name="message">Mensagem a ser exibida</param>
    public void ShowPanel(string message = null)
    {
        if (confirmationText != null)
        {
            confirmationText.text = !string.IsNullOrEmpty(message) ? message : defaultMessage;
        }

        confirmationPanel.SetActive(true);
    }

    /// <summary>
    /// Esconde o painel de confirmação
    /// </summary>
    public void HidePanel()
    {
        confirmationPanel.SetActive(false);
    }

    // Manipuladores para cada botão
    private void HandleSaveAndExit()
    {
        OnSaveAndExit?.Invoke();
        HidePanel();
    }

    private void HandleExitWithoutSaving()
    {
        OnExitWithoutSaving?.Invoke();
        HidePanel();
    }

    private void HandleCancelExit()
    {
        OnCancelExit?.Invoke();
        HidePanel();
    }
}
