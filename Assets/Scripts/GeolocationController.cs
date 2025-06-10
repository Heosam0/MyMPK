using System.Collections;
using UnityEngine;
using UnityEngine.Android; // ��� Android

public class GeolocationController : MonoBehaviour
{
    public bool isLocationReady = false;
    public bool emulation = false;
    public float latitude;
    public float longitude;
    public float accuracy;

    IEnumerator Start()
    {
        // �������� ���������� ��� Android
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(1); // ���� ����� �� �����
        }
#endif

        // ��������, �������������� �� ������ ����������
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("���������� ��������� �������������");
            yield break;
        }

        // ��������� ������
        Input.location.Start(10f, 10f); // �������� � ������

        // ���� �������������
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // ���� ����� �����
        if (maxWait < 1)
        {
            Debug.Log("������� ������������� ����������");
            yield break;
        }

        // ���� ���������� �����������
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("�� ������� ���������� ��������������");
            yield break;
        }
        else
        {
            // ������ � ������ �������
            isLocationReady = true;
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            accuracy = Input.location.lastData.horizontalAccuracy;

            Debug.Log($"��������������: {latitude}, {longitude} � {accuracy}�");
        }
    }

    void Update()
    {
        if (isLocationReady && !emulation)
        {
            // ��������� ������
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            accuracy = Input.location.lastData.horizontalAccuracy;
        }
    }

    void OnDestroy()
    {
        // ������������� ������ ��� ����������� �������
        Input.location.Stop();
    }
}