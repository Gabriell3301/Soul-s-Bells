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
    private bool grounded = false;
    public float amplitude = 0.5f; // altura da flutuação
    public float speed = 2f;

    private Vector3 startPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 4f));
        startPos = transform.localPosition;
    }

    void Update()
    {
        if (grounded)
        { 
            Floating();
            return;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        if (hit.collider != null && rb.velocity.y <= 0)
        {
            grounded = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            Debug.Log("Moeda sem gravidade.");
            //rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
    }
    private void Floating()
    {
        float offset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = startPos + new Vector3(0f, offset, 0f);
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
