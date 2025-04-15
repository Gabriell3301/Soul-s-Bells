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
            DontDestroyOnLoad(gameObject); // Mant�m o Canvas entre cenas
        }
        else
        {
            Destroy(gameObject); // Garante que n�o existam m�ltiplos Canvas
        }
    }
}
