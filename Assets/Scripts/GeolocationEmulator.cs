using UnityEngine;

public class GeolocationEmulator : MonoBehaviour
{
    public bool useSimulator = true;
    public float simulatedLatitude = 55.123456f;
    public float simulatedLongitude = 37.123456f;
    public float simulatedAccuracy = 5f;

    public GeolocationController geolocationController;

    void Update()
    {
        if (useSimulator && Application.isEditor)
        {
            geolocationController.isLocationReady = true;
            geolocationController.latitude = simulatedLatitude;
            geolocationController.longitude = simulatedLongitude;
            geolocationController.accuracy = simulatedAccuracy;
        }
    }
}