using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemysMenager : MonoBehaviour
{
    private List<Enemy> enemies = new List<Enemy>();
    private bool isAlerted = false;
    private Transform player;

    void Start()
    {
        foreach (Transform child in transform)
        {
            Enemy enemy = child.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemies.Add(enemy);
                //enemy.SetManager(this);
            }
        }
    }

    public void AlertEnemies(Transform target)
    {
        if (!isAlerted)
        {
            isAlerted = true;
            player = target;
            foreach (Enemy enemy in enemies)
            {
                //enemy.StartChase(target);
            }
        }
    }

    public void ResetAlert()
    {
        isAlerted = false;
        foreach (Enemy enemy in enemies)
        {
           // enemy.StopChase();
        }
    }
}
