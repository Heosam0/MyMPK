using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockFPS : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 1;

        int refreshRate = Screen.currentResolution.refreshRate;

        Application.targetFrameRate = refreshRate > 0 ? refreshRate : 60;

        Debug.Log($"FPS capped to screen refresh rate: {Application.targetFrameRate} Hz");
    }
}
