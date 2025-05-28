using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    public Vector3 offset = new Vector3(0, 1, -10);

    [Header("Movement Settings")]
    [Range(0.1f, 20f)]
    public float smoothSpeed = 0.125f;
    public float verticalSmoothSpeed = 0.1f;
    public float horizontalSmoothSpeed = 0.15f;

    [Header("Look Ahead")]
    public bool useLookAhead = true;
    public float lookAheadDistance = 2f;
    public float lookAheadSpeed = 0.5f;

    private Vector3 targetPosition;
    private Vector3 currentVelocity;
    private Vector3 lookAheadOffset;
    private Camera mainCamera;

    public static CameraFollow instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        mainCamera = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Calcula a posição base da câmera
        targetPosition = player.position + offset;
        
        // Aplica look ahead baseado na direção do movimento do jogador
        if (useLookAhead)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            lookAheadOffset = Vector3.Lerp(lookAheadOffset, 
                new Vector3(horizontalInput * lookAheadDistance, 0, 0), 
                lookAheadSpeed * Time.fixedDeltaTime);
            targetPosition += lookAheadOffset;
        }

        // Movimento suave com velocidades diferentes para cada eixo
        Vector3 smoothedPosition = transform.position;
        smoothedPosition.x = Mathf.Lerp(transform.position.x, targetPosition.x, horizontalSmoothSpeed);
        smoothedPosition.y = Mathf.Lerp(transform.position.y, targetPosition.y, verticalSmoothSpeed);
        smoothedPosition.z = targetPosition.z;

        // Aplica a posição final
        transform.position = smoothedPosition;
    }
}