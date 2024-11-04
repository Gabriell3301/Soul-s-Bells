using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // Para referenciar a câmera principal
    [SerializeField] private float followSpeed;

    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            // Pega a posição do mouse na tela
            Vector3 mousePosition = Input.mousePosition;

            // Converte a posição do mouse para coordenadas do mundo
            mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

            // Mantém o valor de Z fixo para 2D (0 geralmente para jogos 2D)
            mousePosition.z = 0f;

            // Aplica um Lerp para fazer a bolinha se mover suavemente em direção ao mouse
            transform.position = Vector2.Lerp(transform.position, mousePosition, followSpeed * Time.deltaTime);
        }
    }
}
