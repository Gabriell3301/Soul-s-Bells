using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private float dashSpeed = 50f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.4f;

    private bool canDash = true;
    private TrailRenderer trailRender;
    private Rigidbody2D rb;
    PlayerStateList pState;
    // Start is called before the first frame update
    void Start()
    {
        trailRender = GetComponent<TrailRenderer>();
        pState = GetComponent<PlayerStateList>();
        if (!TryGetComponent<Rigidbody2D>(out rb))
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && canDash)
        {
            StartCoroutine(Dashing());
        }
    }
    private IEnumerator Dashing()
    {
        // Inicia o dash
        canDash = false;
        pState.SetDashing(true);
        trailRender.emitting = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0); // Define a velocidade do dash no eixo horizontal

        yield return new WaitForSeconds(dashDuration); // Aguarda a duração do dash
        // Termina o dash
        trailRender.emitting = false;
        rb.gravityScale = originalGravity;
        pState.SetDashing(false);

        yield return new WaitForSeconds(dashCooldown); // Espera o cooldown antes de poder dashar novamente
        canDash = true;
    }
}
