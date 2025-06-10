using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARQRScanner : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private Text debug_name;
    private string apiUrl = "https://mympk.heosam.ru/api/ar/marker/";

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            string markerData = $"room:{trackedImage.referenceImage.name.Replace("_", ":")}";
            StartCoroutine(GetAPIData(markerData));
        }

        Debug.Log($"Найдено изображений: {eventArgs.added.Count}");
        foreach (var trackedImage in eventArgs.added)
        {
            Debug.Log($"Распознано: {trackedImage.referenceImage.name}");
        }
    }

    IEnumerator GetAPIData(string markerData)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + markerData);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            RoomData data = JsonUtility.FromJson<RoomData>(request.downloadHandler.text);
            Debug.Log($"Распознан кабинет: {data.room_name}");

            debug_name.text = data.room_name;
        }
    }

   /* void CreateARLabel(RoomData data, Transform parent) 
    {
        GameObject label = new GameObject("AR_Label");
        label.transform.SetParent(parent);
        label.transform.localPosition = Vector3.up * 0.2f; 

        TextMesh text = label.AddComponent<TextMesh>();
        text.text = $"{data.room_name}\nЭтаж: {data.floor}";
        text.fontSize = 10;
        text.anchor = TextAnchor.MiddleCenter;
    }  говно, переделать */
}