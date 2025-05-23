using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gerencia o botão de habilidade na UI.
/// </summary>
public class AbilityButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject lockedOverlay;

    private Ability ability;
    public string AbilityId => ability?.id;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (icon == null) icon = transform.Find("Icon")?.GetComponent<Image>();
        if (nameText == null) nameText = transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
        if (costText == null) costText = transform.Find("Cost")?.GetComponent<TextMeshProUGUI>();
        if (lockedOverlay == null) lockedOverlay = transform.Find("LockedOverlay")?.gameObject;
    }

    public void Initialize(Ability ability)
    {
        this.ability = ability;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (ability == null) return;

        if (icon != null) icon.sprite = ability.icon;
        if (nameText != null) nameText.text = ability.name;
        if (costText != null) costText.text = ability.cost.ToString();
        if (lockedOverlay != null) lockedOverlay.SetActive(!ability.isUnlocked);
        if (button != null) button.interactable = ability.isUnlocked;
    }

    public void UpdateState(bool isUnlocked)
    {
        if (ability != null)
        {
            ability.isUnlocked = isUnlocked;
            UpdateUI();
        }
    }

    public void OnClick()
    {
        if (ability != null && ability.isUnlocked)
        {
            // Trigger ability activation
            AbilityManager.Instance.UseAbility(ability.id);
        }
    }
} 