using UnityEngine;
using UnityEngine.InputSystem;


public interface IAbility
{
    void Initialize(AbilityData abilityData);
    void Activate();
    void Deactivate();
    bool CanActivate();
    float GetCooldownRemaining();
    void OnLevelUp();
}

// Classe base abstrata para habilidades (opcional, mas recomendada)
public abstract class BaseAbility : MonoBehaviour, IAbility
{
    [Header("Ability Base")]
    protected AbilityData abilityData;
    protected float lastActivationTime;
    protected bool isActive;
    
    public virtual void Initialize(AbilityData data)
    {
        abilityData = data;
        OnInitialize();
    }
    
    protected virtual void OnInitialize() { }
    
    public virtual void Activate()
    {
        Debug.Log("Antes do Return");
        if (!CanActivate()) return;

        Debug.Log("OIIIIIIIIIIIII");
        lastActivationTime = Time.time;
        isActive = true;
        
        // Toca som de ativação
        if (abilityData.activationSound != null)
        {
            AudioSource.PlayClipAtPoint(abilityData.activationSound, transform.position);
        }
        
        OnActivate();
    }
    
    protected abstract void OnActivate();
    
    public virtual void Deactivate()
    {
        isActive = false;
        OnDeactivate();
    }
    
    protected virtual void OnDeactivate() { }
    
    public virtual bool CanActivate()
    {
        Debug.Log("Entrou no CanActive");
        if (abilityData == null) return false;
        Debug.Log("Tem abilityData");
        if (!abilityData.equipped) return false;
        Debug.Log("Está equipada");
        if (GetCooldownRemaining() > 0) return false;
        
        return CanActivateCustom();
    }
    
    protected virtual bool CanActivateCustom() { return true; }
    
    public virtual float GetCooldownRemaining()
    {
        if (abilityData == null) return 0f;
        
        float timeSinceLastActivation = Time.time - lastActivationTime;
        return Mathf.Max(0f, abilityData.TotalCooldown - timeSinceLastActivation);
    }
    
    public virtual void OnLevelUp()
    {
        Debug.Log($"Habilidade {abilityData.abilityName} subiu para o level {abilityData.level}!");
        OnLevelUpCustom();
    }
    
    protected virtual void OnLevelUpCustom() { }
    
    // Método de utilidade para verificar se a habilidade é do tipo passiva
    public bool IsPassive => abilityData?.abilityType == AbilityType.Passive;
    
    // Método de utilidade para obter informações da habilidade
    public string GetAbilityInfo()
    {
        if (abilityData == null) return "Nenhuma habilidade";
        
        return $"{abilityData.abilityName} (Lv.{abilityData.level}) - Cooldown: {GetCooldownRemaining():F1}s";
    }
}