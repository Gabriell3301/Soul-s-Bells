using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SaveBidingSystem : MonoBehaviour
{
    [Header("Botões Principais")]
    [SerializeField] private Button buttonApply;
    [SerializeField] private Button buttonCancel;
    [SerializeField] private Button buttonReset;

    [Header("Referências")]
    [SerializeField] private ExitConfirmationDialog confirmationDialog;
    [SerializeField] private GameObject keybindingPanel;
    [SerializeField] private GameObject OptionPanel;
    public InputActionAsset inputActions;

    // Caminhos para salvar os bindings
    static string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SoulBell");
    static string path = Path.Combine(folder, "saveBinding.json");

    // Backup dos bindings para caso o usuário cancele
    private string backupBindings;
    private bool hasChanges = false;

    private void OnEnable()
    {
        // Salva os bindings atuais antes do jogador editar
        backupBindings = inputActions.SaveBindingOverridesAsJson();
        hasChanges = false;
    }

    private void Awake()
    {
        // Cria o diretório se não existir
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        // Carrega bindings salvos
        LoadBindings();

        // Configura listeners para os botões principais
        buttonApply.onClick.AddListener(() => {
            SaveBindings();
            hasChanges = false;
        });

        buttonCancel.onClick.AddListener(CheckAndShowConfirmation);
        buttonReset.onClick.AddListener(ClearBindings);

        // Configura os handlers para o diálogo de confirmação
        if (confirmationDialog != null)
        {
            confirmationDialog.OnSaveAndExit += SaveAndExit;
            confirmationDialog.OnExitWithoutSaving += ExitWithoutSaving;
            confirmationDialog.OnCancelExit += () => { /* Nada precisa ser feito, o diálogo já se fecha */ };
        }
        else
        {
            Debug.LogError("ExitConfirmationDialog não foi atribuído no Inspector!");
        }

        // Adiciona listener para detectar mudanças nos bindings
        AddChangeDetection();
    }

    private void OnDestroy()
    {
        // Remove os handlers ao destruir o objeto para evitar memory leaks
        if (confirmationDialog != null)
        {
            confirmationDialog.OnSaveAndExit -= SaveAndExit;
            confirmationDialog.OnExitWithoutSaving -= ExitWithoutSaving;
        }
    }

    // Método para detectar mudanças nos bindings
    private void AddChangeDetection()
    {
        ReBindingsScript[] rebinders = FindObjectsOfType<ReBindingsScript>();
        foreach (var rebinder in rebinders)
        {
            if (rebinder.rebindButton != null)
            {
                rebinder.rebindButton.onClick.AddListener(() => {
                    hasChanges = true;
                });
            }
        }
    }

    // Verifica se há mudanças e mostra confirmação se necessário
    private void CheckAndShowConfirmation()
    {
        if (hasChanges && confirmationDialog != null)
        {
            confirmationDialog.ShowPanel("Existem alterações não salvas. O que deseja fazer?");
        }
        else
        {
            // Se não há alterações ou o diálogo não existe, simplesmente fecha o painel
            ExitWithoutSaving();
        }
    }

    // Salva os bindings e fecha o painel
    private void SaveAndExit()
    {
        SaveBindings();
        CloseKeyBindingPanel();
    }

    // Reverte para o backup e fecha o painel sem salvar
    private void ExitWithoutSaving()
    {
        RevertToBackup();
        CloseKeyBindingPanel();
    }

    // Fecha o painel de keybindings
    private void CloseKeyBindingPanel()
    {
        if (keybindingPanel != null)
        {
            OptionPanel.SetActive(true);
            keybindingPanel.SetActive(false);
        }   
        else
            Debug.LogWarning("Keybinding panel reference is missing!");
    }

    // Salva os bindings em arquivo
    private void SaveBindings()
    {
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string json = inputActions.SaveBindingOverridesAsJson();
        File.WriteAllText(path, json);
        Debug.Log("🎮 Keybinds salvos em: " + path);

        // Atualiza o backup após salvar
        backupBindings = json;
        hasChanges = false;

        // Atualiza os textos nos botões
        ReBindingsScript.UpdateAllRebindUIs();
    }

    // Carrega os bindings salvos
    private void LoadBindings()
    {
        if (!File.Exists(path))
        {
            Debug.Log("Nenhum keybind salvo encontrado.");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            inputActions.LoadBindingOverridesFromJson(json);
            Debug.Log("✅ Keybinds carregados de: " + path);

            // Atualiza o backup após carregar
            backupBindings = json;

            // Atualiza os textos nos botões
            ReBindingsScript.UpdateAllRebindUIs();
        }
        catch (Exception e)
        {
            Debug.LogError("Erro ao carregar keybinds: " + e.Message);
        }
    }

    // Reverte para o backup salvo anteriormente
    private void RevertToBackup()
    {
        if (!string.IsNullOrEmpty(backupBindings))
        {
            inputActions.LoadBindingOverridesFromJson(backupBindings);
            Debug.Log("↩️ Alterações canceladas e bindings restaurados.");
            hasChanges = false;

            // Atualiza os textos nos botões
            ReBindingsScript.UpdateAllRebindUIs();
        }
        else
        {
            Debug.Log("⚠️ Nenhum backup encontrado. Cancelamento ignorado.");
        }
    }

    // Limpa todos os bindings customizados
    // Limpa todos os bindings customizados
    private void ClearBindings()
    {
        // Verifica se há algum binding personalizado antes de resetar
        bool hasCustomBindings = false;

        // Checa cada action map
        foreach (var actionMap in inputActions.actionMaps)
        {
            foreach (var action in actionMap.actions)
            {
                // Verifica se algum binding tem override
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    if (action.bindings[i].hasOverrides)
                    {
                        hasCustomBindings = true;
                        break;
                    }
                }

                if (hasCustomBindings)
                    break;
            }

            if (hasCustomBindings)
                break;
        }

        // Limpa bindings e seta hasChanges apenas se realmente havia algum binding customizado
        if (hasCustomBindings)
        {
            inputActions.RemoveAllBindingOverrides();
            Debug.Log("⛔ Keybinds resetados.");
            hasChanges = true;
        }
        else
        {
            Debug.Log("ℹ️ Nenhum keybind personalizado para resetar.");
        }

        // Atualiza os textos nos botões
        ReBindingsScript.UpdateAllRebindUIs();
    }
}