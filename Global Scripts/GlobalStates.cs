using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStates : MonoBehaviour
{
    public bool isPaused;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }
}
