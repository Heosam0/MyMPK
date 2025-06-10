using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewsTemplateScript : MonoBehaviour
{
    [SerializeField] private Text Title;
    [SerializeField] private Text Description;
    [SerializeField] private Text Date;
    [SerializeField] private string Link;
    [SerializeField] private Image Image;

                
     public async void LoadData(News data)
    {
        Title.text = data.title;
        Description.text = data.description;
       // Date.text = Description.text;
        Link = data.link;

        Texture2D texture = await RoomInfo.GetRemoteTexture(data.image);
        if (texture != null)
        {
            Sprite image = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            Image.sprite = image;
           float originalWidth = image.textureRect.width;
           float originalHeight = image.textureRect.height;
            
            if (originalWidth > 600)
           {
               float scale = 600f / originalWidth;
    
               float newWidth = 600f;
               float newHeight = originalHeight * scale;
    
               Image.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
           }
           else
           {
                           Image.rectTransform.sizeDelta = new Vector2(originalWidth, originalHeight);
           }
        }
        else
        {
            Image.gameObject.SetActive(false);
        }
    }

    public void OpenLink()
    {
        Application.OpenURL(Link);
    }

 
}
