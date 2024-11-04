using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStates : MonoBehaviour
{
    public bool isPaused;

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }
}
