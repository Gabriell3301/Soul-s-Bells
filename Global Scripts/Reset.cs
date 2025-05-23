using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;
    Rigidbody2D rbPlayer;
    Rigidbody2D rbEnemy;
    private Vector2 playerPosition;
    private Vector2 enemyPosition;
    // Start is called before the first frame update
    void Start()
    {
        rbPlayer = Player.GetComponent<Rigidbody2D>();
        rbEnemy = Enemy.GetComponent<Rigidbody2D>();
        playerPosition = Player.transform.position;
        enemyPosition = Enemy.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rbEnemy.velocity = new Vector2(0, 0);
            rbPlayer.velocity = new Vector2(0, 0);
            Player.transform.position = playerPosition;
            Enemy.transform.position = enemyPosition;
        }
    }
}
