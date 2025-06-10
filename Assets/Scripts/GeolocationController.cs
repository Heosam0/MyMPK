using System.Collections;
using UnityEngine;
using UnityEngine.Android; // Для Android

public class GeolocationController : MonoBehaviour
{
    public bool isLocationReady = false;
    public bool emulation = false;
    public float latitude;
    public float longitude;
    public float accuracy;

    IEnumerator Start()
    {
        // Проверка разрешений для Android
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(1); // Даем время на ответ
        }
#endif

        // Проверка, поддерживается ли сервис геолокации
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Геолокация отключена пользователем");
            yield break;
        }

        // Запускаем сервис
        Input.location.Start(10f, 10f); // Точность в метрах

        // Ждем инициализации
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Если время вышло
        if (maxWait < 1)
        {
            Debug.Log("Таймаут инициализации геолокации");
            yield break;
        }

        // Если соединение провалилось
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Не удалось определить местоположение");
            yield break;
        }
        else
        {
            // Доступ к данным получен
            isLocationReady = true;
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            accuracy = Input.location.lastData.horizontalAccuracy;

            Debug.Log($"Местоположение: {latitude}, {longitude} ± {accuracy}м");
        }
    }

    void Update()
    {
        if (isLocationReady && !emulation)
        {
            // Обновляем данные
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            accuracy = Input.location.lastData.horizontalAccuracy;
        }
    }

    void OnDestroy()
    {
        // Останавливаем сервис при уничтожении объекта
        Input.location.Stop();
    }
}