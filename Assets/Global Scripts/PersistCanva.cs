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
            // Garante que o objeto seja root antes de chamar DontDestroyOnLoad
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject); // Mantém o Canvas entre cenas
        }
        else
        {
            Destroy(gameObject); // Garante que não existam múltiplos Canvas
        }
    }
}
