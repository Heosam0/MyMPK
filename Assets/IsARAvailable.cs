using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class IsARAvailable : MonoBehaviour
{
    [SerializeField] Button Button;

    private void OnEnable()
    {
     
        StartCoroutine(AllowARScene());
    }

    IEnumerator AllowARScene()
    {
        StartCoroutine(ARSession.CheckAvailability());
        yield return null;

        if(ARSession.state == ARSessionState.Unsupported) 
            Button.interactable = false;
    }

}