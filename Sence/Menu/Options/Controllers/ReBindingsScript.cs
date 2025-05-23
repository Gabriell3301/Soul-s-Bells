using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ReBindingsScript : MonoBehaviour
{
    [Header("Referências")]
    public InputActionReference actionToRebind;
    public int bindingIndex = 0;
    public TextMeshProUGUI displayText;
    public Button rebindButton;
    public Button resetButton;

    [Header("Configurações")]
    public Color normalColor = Color.white;
    public Color conflictColor = Color.red;
    public Color listeningColor = Color.yellow;
    public string listeningText = "Pressione uma tecla...";
    public string conflictText = "Tecla já em uso!";
    public float conflictMessageDuration = 2f;

    [Header("Todos os InputActions")]
    public InputActionAsset inputActions;

    [Header("Teclas Bloqueadas")]
    public List<string> blockedKeys = new List<string> { "<Keyboard>/escape" };

    private string originalBinding;
    private Coroutine displayCoroutine;

    private void Start()
    {
        if (actionToRebind != null)
        {
            originalBinding = actionToRebind.action.bindings[bindingIndex].effectivePath;
        }

        UpdateUI();

        rebindButton.onClick.AddListener(() =>
        {
            StartRebind();
        });

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(() =>
            {
                ResetToDefault();
            });
        }

        // Carrega bindings salvos ao iniciar
        LoadBindings();
    }

    public void UpdateUI()
    {
        if (actionToRebind == null || displayText == null) return;

        if (actionToRebind.action.bindings[bindingIndex].hasOverrides)
        {
            var binding = actionToRebind.action.bindings[bindingIndex];
            displayText.text = binding.ToDisplayString();
        }
        else
        {
            displayText.text = actionToRebind.action.bindings[bindingIndex].ToDisplayString();
        }

        displayText.color = normalColor;
    }

    private void StartRebind()
    {
        if (actionToRebind == null) return;

        // Desabilita a ação durante o rebinding
        actionToRebind.action.Disable();

        // Atualiza UI para modo de "escuta"
        displayText.text = listeningText;
        displayText.color = listeningColor;

        var rebindOperation = actionToRebind.action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => CompleteRebind(operation))
            .OnCancel(operation => CancelRebind(operation))
            .Start();
    }

    private void CompleteRebind(InputActionRebindingExtensions.RebindingOperation operation)
    {
        actionToRebind.action.Enable();
        operation.Dispose();

        var newBinding = actionToRebind.action.bindings[bindingIndex];
        string newPath = newBinding.effectivePath;

        // Verifica se a tecla está na lista de bloqueadas
        if (IsBlockedKey(newPath))
        {
            ShowTemporaryMessage("Tecla não permitida!", conflictColor);
            actionToRebind.action.RemoveBindingOverride(bindingIndex);
            return;
        }

        // Verifica conflitos com outras ações
        if (IsBindingConflict(newPath, actionToRebind.action, bindingIndex))
        {
            ShowTemporaryMessage(conflictText, conflictColor);
            actionToRebind.action.RemoveBindingOverride(bindingIndex);
            return;
        }

        Debug.Log($"✅ Nova tecla atribuída: {newBinding.ToDisplayString()}");
        UpdateUI();

        // Salva os novos bindings
        SaveBindings();
    }

    private void CancelRebind(InputActionRebindingExtensions.RebindingOperation operation)
    {
        actionToRebind.action.Enable();
        operation.Dispose();
        UpdateUI();
        Debug.Log("Rebind cancelado.");
    }

    private bool IsBindingConflict(string newPath, InputAction currentAction, int currentBindingIndex)
    {
        foreach (var map in inputActions.actionMaps)
        {
            foreach (var action in map.actions)
            {
                // Pula a própria ação
                if (action == currentAction) continue;

                for (int i = 0; i < action.bindings.Count; i++)
                {
                    var binding = action.bindings[i];

                    // Verifica se o caminho efetivo é igual ao novo binding
                    // e se é do mesmo tipo (teclado vs gamepad)
                    if (binding.effectivePath == newPath &&
                        binding.effectivePath.Contains(currentAction.bindings[currentBindingIndex].effectivePath.Split('/')[0]))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool IsBlockedKey(string keyPath)
    {
        return blockedKeys.Contains(keyPath);
    }

    private void ShowTemporaryMessage(string message, Color color)
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        displayCoroutine = StartCoroutine(DisplayTemporaryMessage(message, color));
    }

    private IEnumerator DisplayTemporaryMessage(string message, Color color)
    {
        displayText.text = message;
        displayText.color = color;

        yield return new WaitForSeconds(conflictMessageDuration);

        UpdateUI();
        displayCoroutine = null;
    }

    public void ResetToDefault()
    {
        if (actionToRebind == null) return;

        actionToRebind.action.RemoveBindingOverride(bindingIndex);
        UpdateUI();
        SaveBindings();
    }

    private void SaveBindings()
    {
        // Salvando todos os bindings personalizados no PlayerPrefs
        string rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("InputBindings", rebinds);
        PlayerPrefs.Save();
        Debug.Log("Bindings salvos com sucesso!");
    }

    private void LoadBindings()
    {
        if (PlayerPrefs.HasKey("InputBindings"))
        {
            string rebinds = PlayerPrefs.GetString("InputBindings");
            inputActions.LoadBindingOverridesFromJson(rebinds);
            Debug.Log("Bindings carregados com sucesso!");
        }
        UpdateUI();
    }

    public static void UpdateAllRebindUIs()
    {
        foreach (var rebinder in FindObjectsOfType<ReBindingsScript>())
        {
            rebinder.UpdateUI();
        }
    }
}