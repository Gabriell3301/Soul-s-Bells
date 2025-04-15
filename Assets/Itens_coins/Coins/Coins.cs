using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public int valor = 1;
    private bool canTake = true;
    private Rigidbody2D rb;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 4f));
    }

    void Update()
    {
        // Raycast para verificar se está perto do chão
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        if (hit.collider != null)
        {
            // Parar a física da moeda
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canTake && other.CompareTag("Player"))
        {
            canTake = false;
            CoinsManager.Instance.AdicionarMoeda(valor);
            Destroy(gameObject);
        }
    }
}
