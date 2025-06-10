using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanoramaCameraController : MonoBehaviour
{
    [Header("Camera Rotation")]
    public float rotationSpeed = 2.0f;
    public bool enableMouseControl = true;
    private Vector2 currentRotation;

    [Header("Zoom (FOV) Settings")]
    public float minFOV = 30f;
    public float maxFOV = 100f;
    public float defaultFOV = 60f;
    public float zoomSensitivity = 0.5f;
    private Camera cam;

    [Header("Excurse")]
    public List<Excurse> excurses = new List<Excurse>();
    [SerializeField] Text exName;
    [SerializeField] Text exDescription;



    private float initialDistance;
    private float initialFOV;     


    public Material[] panoramaMaterials; 
    public MeshRenderer sphereRenderer;  

    [Header("Animation References")]
    public Animator transitionAnimator;
    public string fadeOutTrigger = "FadeOut";
    public string fadeInTrigger = "FadeIn";

    private int currentIndex = 0;
    private bool isTransitioning = false;

    public void NextPanorama()
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionPanorama(1));
    }

    public void PreviousPanorama()
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionPanorama(-1));
    }

    private IEnumerator TransitionPanorama(int direction)
    {
        isTransitioning = true;

        if (transitionAnimator != null)
        {
            transitionAnimator.Play("fade_in");
            yield return new WaitForSeconds(GetAnimationLength("fade_in"));
        }

        currentIndex = (currentIndex + direction + panoramaMaterials.Length) % panoramaMaterials.Length;
        Material newMaterial = panoramaMaterials[currentIndex];
        exName.text = excurses[currentIndex].Name;
        exDescription.text = excurses[currentIndex].Description;
        sphereRenderer.material = newMaterial;

        if (transitionAnimator != null)
        {
            transitionAnimator.Play("fade_out");
            yield return new WaitForSeconds(GetAnimationLength("fade_out"));
        }

        isTransitioning = false;
    }

    private float GetAnimationLength(string clipName)
    {
        AnimationClip[] clips = transitionAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Contains(clipName))
            {
                return clip.length;
            }
        }
        return 0f;
    }


    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;

        cam.fieldOfView = defaultFOV;
    }

    void Update()
    {
        HandleCameraRotation();
        HandleCameraRotationByMouse();
        HandleZoom();
    }

    private void HandleCameraRotation()
    {
      

        if (Input.touchCount == 0 || EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Moved)
        {
            Vector2 delta = touch.deltaPosition;
            currentRotation.x += delta.x * rotationSpeed * 0.01f; 
            currentRotation.y -= delta.y * rotationSpeed * 0.01f;
            currentRotation.y = Mathf.Clamp(currentRotation.y, -90f, 90f);
        }

        transform.rotation = Quaternion.Lerp(
    transform.rotation,
    Quaternion.Euler(currentRotation.y, currentRotation.x, 0),
    Time.deltaTime * 5f
);
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * zoomSensitivity * 10, minFOV, maxFOV);
        }

        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touch1.position, touch2.position);
                initialFOV = cam.fieldOfView;
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                float pinchRatio = initialDistance / currentDistance;
                cam.fieldOfView = Mathf.Clamp(initialFOV * pinchRatio, minFOV, maxFOV);
            }
        }
    }
    private void HandleCameraRotationByMouse()
    {
        if (!enableMouseControl) return;

        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButton(0))
        {
            currentRotation.x += Input.GetAxis("Mouse X") * rotationSpeed;
            currentRotation.y -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentRotation.y = Mathf.Clamp(currentRotation.y, -90f, 90f);
        }

        transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
    }
}

[Serializable]
public struct Excurse
{
    public string Name;
    [TextArea(3, 10)]
    public string Description;
}