using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // Para referenciar a c�mera principal
    [SerializeField] private float followSpeed;

    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            // Pega a posi��o do mouse na tela
            Vector3 mousePosition = Input.mousePosition;

            // Converte a posi��o do mouse para coordenadas do mundo
            mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

            // Mant�m o valor de Z fixo para 2D (0 geralmente para jogos 2D)
            mousePosition.z = 0f;

            // Aplica um Lerp para fazer a bolinha se mover suavemente em dire��o ao mouse
            transform.position = Vector2.Lerp(transform.position, mousePosition, followSpeed * Time.deltaTime);
        }
    }
}
