using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoomInfo : MonoBehaviour
{
    [SerializeField] Text roomName;
    [SerializeField] Text roomDescription;
    [SerializeField] Text roomLocation;
    [SerializeField] Text roomTeacher;
    [SerializeField] Image roomImage;
    [SerializeField] Texture2D defaultTexture;
    [SerializeField] Animator anim;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject contentPanel;

    private bool isDataLoaded = false;

    public async void LoadData(RoomData roomData)
    {
        ResetUI();
        ShowLoading(true);

        try
        {
            SetTextData(roomData);

            await LoadRoomImage(roomData.image);

            ShowContent();
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка загрузки: {e.Message}");
            GetDefaultSprite();
            ShowContent();
        }
    }

    private void ResetUI()
    {
        isDataLoaded = false;
        roomImage.sprite = null;
        roomName.text = "";
        roomDescription.text = "";
        roomTeacher.text = "";
        roomLocation.text = "";
    }

    private void ShowLoading(bool show)
    {
        loadingPanel.SetActive(show);
        contentPanel.SetActive(!show);
    }

    private void SetTextData(RoomData roomData)
    {
        roomName.text = roomData.room_name;
        roomDescription.text = roomData.description;
        roomTeacher.text = roomData.teachers != "" ? roomData.teachers : "Не указан";
        roomLocation.text = $"{roomData.building} здание, {roomData.floor} этаж";
    }

    private async Task LoadRoomImage(string imageUrl)
    {
        try
        {
            Texture2D texture = await GetRemoteTexture(imageUrl);
            roomImage.sprite = texture != null
                ? Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100)
                : GetDefaultSprite();
        }
        catch
        {
            roomImage.sprite = GetDefaultSprite();
        }
    }

    private void ShowContent()
    {
        isDataLoaded = true;
        ShowLoading(false);
        anim.Play("OpenInfoRoom");
    }

    private Sprite GetDefaultSprite()
    {
        return defaultTexture != null
            ? Sprite.Create(defaultTexture, new Rect(0, 0, defaultTexture.width, defaultTexture.height), Vector2.one * 0.5f, 100)
            : null;
    }

    public static async Task<Texture2D> GetRemoteTexture(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            www.timeout = 10;
            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Ошибка загрузки изображения: {www.error}");
                return null;
            }

            return DownloadHandlerTexture.GetContent(www);
        }
    }
}