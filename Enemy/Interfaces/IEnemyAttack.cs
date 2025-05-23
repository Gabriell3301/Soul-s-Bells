using UnityEngine;

public interface IEnemyAttack
{
    void OnParried();
    void OnReflected(Enemy target);
} 