using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenScene : MonoBehaviour
{
    
    public void LoadScene(string name)
    {
       StartCoroutine(LoadAsync(name));
    }

    private IEnumerator LoadAsync(string name)
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadSceneAsync(name);
    }
}
