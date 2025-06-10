using UnityEngine;

public class GPSMapPositioner : MonoBehaviour
{
    public GeolocationController geolocation;

    public float mapRealWorldTopLatitude, mapRealWorldLeftLongitude, mapRealWorldBottomLatitude, mapRealWorldRightLongitude;

    public float mapModelWidth = 100f;
    public float mapModelLength = 100f;

    public GameObject locationMarkerPrefab;
    private GameObject currentMarker;

    void Start()
    {
        currentMarker = Instantiate(locationMarkerPrefab, transform);
        currentMarker.SetActive(false);
    }

    void Update()
    {
        if (geolocation.isLocationReady)
        {
            UpdateLocationMarker();
        }
    }

    void UpdateLocationMarker()
    {
        Vector2 mapPosition = ConvertGPSToMapPosition(
            geolocation.latitude,
            geolocation.longitude);

        currentMarker.transform.localPosition = new Vector3(
            mapPosition.x,
            0.5f, 
            mapPosition.y);

        currentMarker.SetActive(true);
    }

    Vector2 ConvertGPSToMapPosition(float latitude, float longitude)
    {
      
        float xNormalized = Mathf.InverseLerp(
            mapRealWorldLeftLongitude,
            mapRealWorldRightLongitude,
            longitude);

        float zNormalized = Mathf.InverseLerp(
            mapRealWorldBottomLatitude,
            mapRealWorldTopLatitude,
            latitude);

        
        float xPosition = Mathf.Lerp(
            -mapModelWidth / 2f,
            mapModelWidth / 2f,
            xNormalized);

        float zPosition = Mathf.Lerp(
            -mapModelLength / 2f,
            mapModelLength / 2f,
            zNormalized);

        return new Vector2(xPosition, zPosition);
    }
}