using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    protected int hits;
    public int Hits  // Getter público
    {
        get { return hits; }
    }
    public void Initialize(Enemy enemy)
    {
        hits = enemy.hits;
    }
    public void Destroythis()
    {
        Destroy(gameObject);
    }
}
