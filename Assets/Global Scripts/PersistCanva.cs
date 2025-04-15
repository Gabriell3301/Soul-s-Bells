using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistCanva : MonoBehaviour
{
    private static PersistCanva instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantém o Canvas entre cenas
        }
        else
        {
            Destroy(gameObject); // Garante que não existam múltiplos Canvas
        }
    }
}
