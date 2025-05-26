using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LockedAbilityUIButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI abilityNameText;
    [SerializeField] private TextMeshProUGUI abilityDescriptionText;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private GameObject lockIcon; // Ícone do cadeado
    [SerializeField] private Image backgroundImage; // Imagem de fundo para deixar acinzentada
    
    private AbilityData abilityData;
    
    public AbilityData AbilityData => abilityData;
    
    private void Awake()
    {
        // Tenta encontrar os componentes automaticamente se não foram definidos
        if (button == null)
            button = GetComponent<Button>();
            
        if (abilityNameText == null)
            abilityNameText = GetComponentInChildren<TextMeshProUGUI>();
            
        if (lockIcon == null)
        {
            // Procura por um child que possa ser o ícone do cadeado
            Transform lockTransform = transform.Find("LockIcon");
            if (lockTransform == null) lockTransform = transform.Find("Lock");
            if (lockTransform == null) lockTransform = transform.Find("Cadeado");
            
            if (lockTransform != null)
                lockIcon = lockTransform.gameObject;
        }
        
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
    }
    
    public void Setup(AbilityData ability)
    {
        abilityData = ability;
        UpdateUI();
        
        // Desabilita o botão para que não seja clicável
        if (button != null)
        {
            button.interactable = false;
        }
    }
    
    private void UpdateUI()
    {
        if (abilityData == null) return;
        
        // Atualiza o nome da habilidade
        if (abilityNameText != null)
        {
            abilityNameText.text = abilityData.abilityName;
            // Deixa o texto mais escuro/acinzentado
            abilityNameText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
            
        // Atualiza a descrição se existe
        if (abilityDescriptionText != null)
        {
            // Pode mostrar uma mensagem de "Habilidade bloqueada" ou a descrição normal
            abilityDescriptionText.text = "Habilidade bloqueada"; // ou abilityData.description
            abilityDescriptionText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
            
        // Atualiza o ícone se existe
        if (abilityIcon != null && abilityData.abilityIcon != null)
        {
            abilityIcon.sprite = abilityData.abilityIcon;
            // Deixa o ícone mais escuro/acinzentado
            abilityIcon.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        
        // Mostra o ícone do cadeado
        if (lockIcon != null)
        {
            lockIcon.SetActive(true);
        }
        
        // Deixa o fundo acinzentado
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        }
    }
    
    // Método opcional para debug
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        
        // Auto-assign components in editor
        if (button == null)
            button = GetComponent<Button>();
            
        if (abilityNameText == null)
            abilityNameText = GetComponentInChildren<TextMeshProUGUI>();
    }
}