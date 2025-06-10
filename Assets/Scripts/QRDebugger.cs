using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class QRDebugger : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    void Start()
    {
        Debug.Log("Доступные изображения в библиотеке:");
        for(int i = 0; i < trackedImageManager.referenceLibrary.count; i++)
        {
            Debug.Log($"- {trackedImageManager.referenceLibrary[i].name} (Размер: {trackedImageManager.referenceLibrary[i].size}m)");
        }
    }
}
