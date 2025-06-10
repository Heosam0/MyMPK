using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

public class ARRoomLoader : MonoBehaviour
{
    public string apiUrl = "https://mympk.heosam.ru/api/rooms/";
    [SerializeField] private RoomInfo roomInfoUI;
    [SerializeField] private GameObject loadingIndicator;

    private void Awake()
    {
        LoadRoomData("testing");
    }

    public void LoadRoomData(string markerId)
    {
        StartCoroutine(GetRoomData(markerId));
    }

    IEnumerator GetRoomData(string markerId)
    {
        loadingIndicator.SetActive(true);

        string url = apiUrl + markerId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                RoomData room = JsonUtility.FromJson<RoomData>(request.downloadHandler.text);
                roomInfoUI.LoadData(room);
                Debug.Log($"Загружено: {room.room_name}");
            }
            else
            {
                Debug.LogError($"Ошибка: {request.error}");
            }
        }

        loadingIndicator.SetActive(false);
    }
}

[System.Serializable]
public class RoomData
{
    public string room_number;
    public string room_name;
    public string description;
    public string building;
    public int floor;
    public string image;
    public string teachers;
}