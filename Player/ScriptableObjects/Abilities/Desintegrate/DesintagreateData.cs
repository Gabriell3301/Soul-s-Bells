using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDesintagreateAbility", menuName = "Abilities/DesintagreateAbility")]
public class DesintagreateData : AbilityData
{
    [Header("Animador/Sprite")]
    public Animator animator;
    
    [Header("Prefab")]
    public GameObject prefabRay;
    
    [Header("Configurações do Raio")]
    [Tooltip("Tempo mínimo para ativar modo targeting (morte instantânea)")]
    public float holdThreshold = 0.5f;
    
    [Tooltip("Alcance para encontrar inimigos no modo segurar")]
    public float targetingRange = 15f;
    
    [Header("Configurações Box Collider")]
    [Tooltip("Tamanho do box para raios que matam (X, Y)")]
    public Vector2 instantKillBoxSize = new Vector2(3f, 3f);
    
    [Tooltip("Tamanho do box para raio de dano (X, Y)")]
    public Vector2 damageBoxSize = new Vector2(2f, 8f);
    
    [Header("Configurações de Dano")]
    [Tooltip("Dano causado pelo raio rápido")]
    public float quickRayDamage = 2f;
    
    [Tooltip("Distância do raio rápido à frente do player")]
    public float forwardDistance = 5f;
}