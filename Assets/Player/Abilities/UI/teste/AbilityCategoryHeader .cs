using UnityEngine;
using TMPro;

// Script opcional para criar cabeçalhos de categoria na UI
public class AbilityCategoryHeader : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI categoryNameText;
    [SerializeField] private GameObject separatorLine;
    
    public void Setup(string categoryName)
    {
        if (categoryNameText != null)
        {
            categoryNameText.text = categoryName.ToUpper();
        }
        
        if (separatorLine != null)
        {
            separatorLine.SetActive(true);
        }
    }
}