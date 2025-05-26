using UnityEngine;

[CreateAssetMenu(fileName = "NewEspadaAbility", menuName = "Abilities/EspadaAbility")]
public class EspadaData : AbilityData
{
    [Header("Prefab")]
    public GameObject espadaPrefab;
    public GameObject impactEffectPrefab;

    [Header("Modo Segurar")]
    public float holdThreshold = 0.7f;
    public float targetingRange = 20f;
    public float spawnHeight = 10f;
    public float trackingSpeed = 5f;
    public float fallSpeed = 15f;

    [Header("Modo Projétil")]
    public float projectileSpeed = 25f;

    [Header("Dano")]
    public float damageRadius = 2f;

    [Header("Multi Espada")]
    public float angleBetweenSwords = 30f;
    public Vector3[] espadaOffsets = {
        new Vector3(0, 0, 0),          // Centro
        new Vector3(-0.5f, 0, 0.5f),   // Esquerda
        new Vector3(0.5f, 0, 0.5f)     // Direita
    };
}