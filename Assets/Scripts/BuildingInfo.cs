using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BuildingInfo : MonoBehaviour
{
    [SerializeField] Text buildingName;
    [SerializeField] Text buildingDescription;
    [SerializeField] Text buildingLocation;
    [SerializeField] Image buildingImage;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] Texture2D defaultTexture;
    [SerializeField] private GameObject contentPanel;
    [SerializeField] Animator anim;

    private bool isImageLoaded = false;

    public void SetLoadingState(bool isLoading)
    {
        loadingIndicator.SetActive(isLoading);
        contentPanel.SetActive(!isLoading);
        if (!isLoading && isImageLoaded)
        {
            anim.Play("OpenInfoRoom");
        }
    }

    public async void LoadData(BuildingData buildingData)
    {
        isImageLoaded = false;

        buildingName.text = buildingData.name;
        buildingDescription.text = buildingData.description;
        buildingLocation.text = buildingData.address;

        // Показываем состояние загрузки
        SetLoadingState(true);

        try
        {
            Texture2D texture = await GetRemoteTexture(buildingData.image);

            Sprite image = texture != null
                ? Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f)
                : GetDefaultSprite();

            buildingImage.sprite = image;
            isImageLoaded = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка обработки изображения: {e.Message}");
            buildingImage.sprite = GetDefaultSprite();
            isImageLoaded = true; 
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private Sprite GetDefaultSprite()
    {
        if (defaultTexture != null)
        {
            return Sprite.Create(defaultTexture,
                new Rect(0.0f, 0.0f, defaultTexture.width, defaultTexture.height),
                new Vector2(0.5f, 0.5f), 100.0f);
        }
        return null;
    }

    public void SetErrorState(string errorMessage)
    {
        loadingIndicator.SetActive(false);
        contentPanel.SetActive(true);
        buildingDescription.text = errorMessage;
    }

    public static async Task<Texture2D> GetRemoteTexture(string url)
    {
        if (string.IsNullOrEmpty(url))
            return null;

        try
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                www.timeout = 10;
                var asyncOp = www.SendWebRequest();

                while (!asyncOp.isDone)
                {
                    await Task.Delay(100);
                }

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"{www.error}, URL:{www.url}");
                    return null;
                }

                return DownloadHandlerTexture.GetContent(www);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка загрузки изображения: {e.Message}");
            return null;
        }
    }

    [System.Serializable]
    public class FloorData
    {
        public string building;
        public int floor;
        public int rooms_count;
    }

    [System.Serializable]
    public class BuildingData
    {
        public int id;
        public string name;
        public string address;
        public string description;
        public string image;
    }
}