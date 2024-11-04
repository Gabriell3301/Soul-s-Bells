using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;       // A refer�ncia ao jogador
    public float smoothSpeed = 0.125f;  // A suavidade da transi��o
    public Vector3 offset;         // O deslocamento da c�mera em rela��o ao jogador

    private Vector3 targetPosition;

    void FixedUpdate()
    {
        if (player != null)
        {
            // Define a posi��o desejada da c�mera com base na posi��o do jogador + deslocamento
            targetPosition = player.position + offset;

            // Aplica a suaviza��o do movimento com Lerp
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            // Atualiza a posi��o da c�mera
            transform.position = smoothedPosition;
        }
    }
}
