using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUri : MonoBehaviour
{
    public void OpenURIButton(string Uri)
    {
        Application.OpenURL(Uri);
    }
}
