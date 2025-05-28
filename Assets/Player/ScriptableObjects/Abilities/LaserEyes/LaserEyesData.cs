using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLaserEyesAbility", menuName = "Abilities/LaserEyesAbility")]
public class LaserEyesData : AbilityData
{
    [Header("Prefab")]
    public GameObject laserPrefab;
    public GameObject impactEffectPrefab;

    [Header("Laser Settings")]
    public float laserRange = 10f;
    public float laserWidth = 0.3f;
    public float laserHeight = 0.3f;
    public float maxDuration = 1f;

    [Header("Visual Effects")]
    public Color laserColor = Color.red;
    public float laserIntensity = 1f;
    public float impactEffectDuration = 0.5f;
}
    