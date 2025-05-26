using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityUIButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI abilityNameText;
    [SerializeField] private TextMeshProUGUI abilityDescriptionText;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private GameObject equippedIndicator; // A imagem verde que indica se está equipado
    [SerializeField] private Image equippedIndicatorImage; // A imagem do indicador (para mudar cor se necessário)
    
    private AbilityData abilityData;
    private bool isEquipped;
    private AbilityUIManager uiManager;
    
    public AbilityData AbilityData => abilityData;
    
    private void Awake()
    {
        // Tenta encontrar os componentes automaticamente se não foram definidos
        if (button == null)
            button = GetComponent<Button>();
            
        if (abilityNameText == null)
            abilityNameText = GetComponentInChildren<TextMeshProUGUI>();
            
        if (equippedIndicator == null)
        {
            // Procura por um child que possa ser o indicador
            Transform indicator = transform.Find("EquippedIndicator");
            if (indicator == null) indicator = transform.Find("Equipped");
            if (indicator == null) indicator = transform.Find("GreenIndicator");
            
            if (indicator != null)
            {
                equippedIndicator = indicator.gameObject;
                equippedIndicatorImage = indicator.GetComponent<Image>();
            }
        }
    }
    
    public void Setup(AbilityData ability, bool equipped, AbilityUIManager manager)
    {
        abilityData = ability;
        isEquipped = equipped;
        uiManager = manager;
        
        UpdateUI();
        
        // Configura o evento do botão
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
        }
    }
    
    private void UpdateUI()
    {
        if (abilityData == null) return;
        
        // Atualiza o nome da habilidade
        if (abilityNameText != null)
            abilityNameText.text = abilityData.abilityName;
            
        // Atualiza a descrição se existe
        if (abilityDescriptionText != null)
            abilityDescriptionText.text = abilityData.description;
            
        // Atualiza o ícone se existe
        if (abilityIcon != null && abilityData.abilityIcon != null)
            abilityIcon.sprite = abilityData.abilityIcon;
            
        // Atualiza o estado do indicador
        UpdateEquippedState(isEquipped);
    }
    
    public void UpdateEquippedState(bool equipped)
    {
        isEquipped = equipped;
        
        if (equippedIndicator != null)
        {
            equippedIndicator.SetActive(isEquipped);
        }
        
        // Opcional: Mudar a cor do indicador
        if (equippedIndicatorImage != null)
        {
            equippedIndicatorImage.color = isEquipped ? Color.green : Color.gray;
        }
        
        // Opcional: Mudar a aparência do botão
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = isEquipped ? new Color(0.8f, 1f, 0.8f) : Color.white;
            button.colors = colors;
        }
    }
    
    private void OnButtonClick()
    {
        if (uiManager != null && abilityData != null)
        {
            uiManager.OnAbilityButtonClicked(abilityData, isEquipped);
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