using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;       // A referência ao jogador
    public float smoothSpeed = 0.125f;  // A suavidade da transição
    public Vector3 offset;         // O deslocamento da câmera em relação ao jogador

    private Vector3 targetPosition;

    void FixedUpdate()
    {
        if (player != null)
        {
            // Define a posição desejada da câmera com base na posição do jogador + deslocamento
            targetPosition = player.position + offset;

            // Aplica a suavização do movimento com Lerp
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            // Atualiza a posição da câmera
            transform.position = smoothedPosition;
        }
    }
}
