using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

public class QRScannerAR : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private GameObject roomInfoGm;
    private RoomInfo roomInfoScript;
    private HashSet<string> processedMarkers = new HashSet<string>();

    void OnEnable() => trackedImageManager.trackedImagesChanged += OnImagesChanged;
    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnImagesChanged;

    private void Awake()
    {
        roomInfoScript = roomInfoGm.GetComponent<RoomInfo>();
    }

    private void OnImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            Debug.Log($"Обнаружен маркер: {trackedImage.referenceImage.name}");
            ProcessMarker(trackedImage);
        }

        foreach (var removedImage in eventArgs.removed)
        {
            Debug.Log($"Removed {removedImage.name}");
        }

    }

    private void ProcessMarker(ARTrackedImage trackedImage)
    {
        string markerId = trackedImage.referenceImage.name;
        processedMarkers.Add(markerId);
     
      
        StartCoroutine(FetchMarkerData(markerId));
    }

    IEnumerator FetchMarkerData(string markerId)
    {
        string cleanId = markerId.Replace("room_", ""); 
        string apiUrl = $"https://mympk.heosam.ru/api/rooms/{cleanId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                RoomData data = JsonUtility.FromJson<RoomData>(webRequest.downloadHandler.text);
                Debug.Log($"Кабинет: {data.room_name}, Этаж: {data.floor}");
                roomInfoGm.SetActive(true);
                roomInfoScript.LoadData(data);
               
            }
        }
    }


    [SerializeField]
    private ARSession arSession;

    public void ResetARSession()
    {
        if (arSession != null)
        {
            arSession.Reset();
        }
    }




}