using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingRaw : MonoBehaviour
{

    private RawImage rawImage;
    [SerializeField] float speed = 1.0f;
    [SerializeField] Vector2 vector;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        rawImage.uvRect = new Rect(rawImage.uvRect.x + vector.x * speed, rawImage.uvRect.x + vector.x * speed, rawImage.uvRect.width, rawImage.uvRect.height); 
    }
}
