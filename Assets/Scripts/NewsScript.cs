using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Timeline;
using UnityEngine.InputSystem;

public class NewsScript : MonoBehaviour
{
    [SerializeField] private GameObject newsPrefab;

    private void Awake()
    {
        StartCoroutine(LoadNews());
    }



    IEnumerator LoadNews()
    {
        string apiUrl = $"https://mympk.heosam.ru/api/news";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                List<News> data = JsonConvert.DeserializeObject<List<News>>(webRequest.downloadHandler.text);
                foreach (News item in data)
                {
                    Debug.Log($"Новость '{item.title}' загружена");
                    GameObject news = Instantiate(newsPrefab, gameObject.transform);
                    news.GetComponent<NewsTemplateScript>().LoadData(item);
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, -100000, gameObject.transform.position.z);

                        }


                }
            }
    }

}

[System.Serializable]
public class News
{
    public string title;
    public string description;
    public string link;
    public string image;
    public string date;
}


