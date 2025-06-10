using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Timeline;

public class BuildingController : MonoBehaviour, IARTappable
{
    public string buildingCode = "";
    private string apiUrl = "https://mympk.heosam.ru/api/buildings/";

    private Material originalMaterial;
    private Color originalColor;
    private Renderer buildingRenderer;

    [SerializeField] private GameObject buildingInfo;
    private bool isLoading = false; 

    void Start()
    {
        buildingRenderer = GetComponent<Renderer>();
        originalMaterial = buildingRenderer.material;
        originalColor = buildingRenderer.material.color;
    }

    public void OnTap()
    {
        if (!isLoading)
        {
            OpenInfo();
        }
    }

    public void Highlight(Color highlightColor)
    {
        buildingRenderer.material.color = highlightColor;
    }

    public void ResetAppearance()
    {
        buildingRenderer.material = originalMaterial;
        buildingRenderer.material.color = originalColor;
    }

    public void OpenInfo()
    {
       
        if (!buildingInfo.activeSelf)
        {
            buildingInfo.SetActive(true);
        }

       
        var infoComponent = buildingInfo.GetComponent<BuildingInfo>();
        if (infoComponent != null)
        {
            infoComponent.SetLoadingState(true); 
        }

        isLoading = true; 
        StartCoroutine(LoadingData());
    }

    IEnumerator LoadingData()
    {
        string url = apiUrl + buildingCode;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        var infoComponent = buildingInfo.GetComponent<BuildingInfo>();

        if (request.result == UnityWebRequest.Result.Success)
        {
            BuildingInfo.BuildingData building = JsonUtility.FromJson<BuildingInfo.BuildingData>(request.downloadHandler.text);
            infoComponent.LoadData(building); 
        }
        else
        {
            infoComponent.SetErrorState("Ошибка загрузки данных");
            Debug.LogError($"Ошибка: {request.error}");
        }

        isLoading = false;
    }
}

