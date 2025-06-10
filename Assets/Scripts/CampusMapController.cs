using UnityEngine;
using UnityEngine.EventSystems;

public class CampusMapController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mapCamera;
    public float zoomSpeed = 0.5f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float moveSpeed = 0.01f;
    public float rotationSpeed = 0.5f;

    [Header("Input Settings")]
    [SerializeField] private bool enableMouseInput = true; 
    [SerializeField] private float mouseDragSensitivity = 0.5f;
    [SerializeField] private float mouseZoomSensitivity = 0.1f;

    [Header("Map Boundaries")]
    public Vector2 mapBoundsX = new Vector2(-50, 50);
    public Vector2 mapBoundsZ = new Vector2(-50, 50);
    public float maxTiltAngle = 80f;
    public float minTiltAngle = 20f;

    [Header("Building Selection")]
    public Color highlightColor = Color.yellow;
    public float focusDistance = 10f;
    public float focusTransitionSpeed = 5f;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private Vector2 lastTouchPosition;
    private Vector2 lastMousePosition;
    private bool isTransitioning = false;
    private Transform focusedBuilding;
    private Color originalBuildingColor;
    private Renderer buildingRenderer;
    [SerializeField] private Vector3 orbitCenter;

    [Header("Camera Return Settings")]
    public float returnDuration = 1.0f;
    private bool isReturning = false;
    private bool isZooming = false;
    private Vector3 returnStartPosition;
    private Quaternion returnStartRotation;
    private float returnStartZoom;
    private float returnStartTime;

    void Start()
    {
        originalCameraPosition = mapCamera.transform.position;
        originalCameraRotation = mapCamera.transform.rotation;
    }

    void Update()
    {
        if (isReturning)
        {
            HandleCameraReturn();
            return;
        }

        if (isTransitioning)
        {
            HandleFocusTransition();
            return;
        }

        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        else if (enableMouseInput)
        {
            HandleMouseInput();
        }
    }

    private void HandleTouchInput()
    {
        bool anyTouchOnUI = false;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
            {
                anyTouchOnUI = true;
                break;
            }
        }

        if (anyTouchOnUI)
            return;

        Touch touch = Input.GetTouch(0);

        if (Input.touchCount == 1)
        {
            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                TrySelectBuilding(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && focusedBuilding == null)
            {
                Vector2 delta = touch.position - lastTouchPosition;
                RotateCameraAround(delta);
                lastTouchPosition = touch.position;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float prevMagnitude = (touch1.position - touch1.position - (Vector2)touch2.position).magnitude;
                float currentMagnitude = (touch1.position - touch2.position).magnitude;
                float difference = currentMagnitude - prevMagnitude;

                Zoom(difference * zoomSpeed);

                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;
                Vector2 prevMiddlePos = (touch1PrevPos + touch2PrevPos) / 2;
                Vector2 middlePos = (touch1.position + touch2.position) / 2;
                Vector2 delta = middlePos - prevMiddlePos;

                Pan(delta);
            }
        }
    }

    private void HandleMouseInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1) && focusedBuilding == null)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastMousePosition;
            RotateCameraAround(delta * mouseDragSensitivity);
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TrySelectBuilding(Input.mousePosition);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Zoom(scroll * mouseZoomSensitivity * 100f);
        }

        if (Input.GetMouseButtonDown(2))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(2))
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastMousePosition;
            Pan(delta * mouseDragSensitivity);
            lastMousePosition = Input.mousePosition;
        }
    }

    private void RotateCameraAround(Vector2 delta)
    {
        float horizontalRotation = delta.x * rotationSpeed;
        mapCamera.transform.RotateAround(orbitCenter, Vector3.up, horizontalRotation);

        float verticalRotation = -delta.y * rotationSpeed;
        float currentAngle = mapCamera.transform.eulerAngles.x;
        float newAngle = currentAngle + verticalRotation;

        if (newAngle > 180f) newAngle -= 360f;
        newAngle = Mathf.Clamp(newAngle, minTiltAngle, maxTiltAngle);

        mapCamera.transform.eulerAngles = new Vector3(
            newAngle,
            mapCamera.transform.eulerAngles.y,
            mapCamera.transform.eulerAngles.z);

        AdjustCameraDistance();
    }

    private void AdjustCameraDistance()
    {
        Vector3 direction = mapCamera.transform.position - orbitCenter;
        direction.y = 0;

        float distance = Vector3.Distance(new Vector3(mapCamera.transform.position.x, 0, mapCamera.transform.position.z),
                                        new Vector3(orbitCenter.x, 0, orbitCenter.z));
        direction = direction.normalized * distance;

        float height = mapCamera.transform.position.y;

        mapCamera.transform.position = orbitCenter + direction;
        mapCamera.transform.position = new Vector3(
            mapCamera.transform.position.x,
            height,
            mapCamera.transform.position.z);
    }

    private void TrySelectBuilding(Vector2 screenPosition)
    {
        if (isReturning) return;
        if (isZooming) return;

        Ray ray = mapCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Building"))
            {
                SelectBuilding(hit.transform);
            }
        }
    }

    private void SelectBuilding(Transform building)
    {
        ResetBuildingSelection();

        focusedBuilding = building;
        buildingRenderer = building.GetComponent<Renderer>();

        if (buildingRenderer != null)
        {
            originalBuildingColor = buildingRenderer.material.color;
            buildingRenderer.material.color = highlightColor;
        }

        BuildingController building1 = building.GetComponent<BuildingController>();
        if (building1 != null)
        {
            building1.OpenInfo();
        }
        isTransitioning = true;
        isZooming = true;
    }

    private void ResetBuildingSelection()
    {
        if (focusedBuilding != null && buildingRenderer != null)
        {
            buildingRenderer.material.color = originalBuildingColor;
        }

        focusedBuilding = null;
        buildingRenderer = null;
    }

    public void DeselectAndReturnCamera()
    {
        isZooming = false;
        ResetBuildingSelection();
        isTransitioning = false;
        StartCameraReturn();
    }

    private void StartCameraReturn()
    {
        isReturning = true;
        returnStartTime = Time.time;
        returnStartPosition = mapCamera.transform.position;
        returnStartRotation = mapCamera.transform.rotation;
        returnStartZoom = mapCamera.orthographic ? mapCamera.orthographicSize : mapCamera.fieldOfView;
    }

    private void HandleCameraReturn()
    {
        float elapsed = Time.time - returnStartTime;
        float progress = Mathf.Clamp01(elapsed / returnDuration);

        float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

        mapCamera.transform.position = Vector3.Lerp(
            returnStartPosition,
            originalCameraPosition,
            smoothProgress);

        mapCamera.transform.rotation = Quaternion.Slerp(
            returnStartRotation,
            originalCameraRotation,
            smoothProgress);

        float targetZoom = (minZoom + maxZoom) * 0.5f;
        float currentZoom = Mathf.Lerp(
            returnStartZoom,
            targetZoom,
            smoothProgress);

        if (mapCamera.orthographic)
        {
            mapCamera.orthographicSize = currentZoom;
        }
        else
        {
            mapCamera.fieldOfView = currentZoom;
        }

        if (progress >= 1f)
        {
            isReturning = false;
            mapCamera.transform.position = originalCameraPosition;
            mapCamera.transform.rotation = originalCameraRotation;
            if (mapCamera.orthographic)
            {
                mapCamera.orthographicSize = targetZoom;
            }
            else
            {
                mapCamera.fieldOfView = targetZoom;
            }
        }
    }

    private void HandleFocusTransition()
    {
        if (focusedBuilding == null)
        {
            isTransitioning = false;
            return;
        }

        Vector3 targetPosition = focusedBuilding.position - mapCamera.transform.forward * focusDistance;

        mapCamera.transform.position = Vector3.Lerp(
            mapCamera.transform.position,
            targetPosition,
            Time.deltaTime * focusTransitionSpeed);

        if (Vector3.Distance(mapCamera.transform.position, targetPosition) < 0.1f)
        {
            isTransitioning = false;
        }
    }

    private void Zoom(float increment)
    {
        float newSize = mapCamera.orthographic ?
            Mathf.Clamp(mapCamera.orthographicSize - increment * zoomSpeed, minZoom, maxZoom) :
            Mathf.Clamp(mapCamera.fieldOfView - increment * zoomSpeed, minZoom, maxZoom);

        if (mapCamera.orthographic)
        {
            mapCamera.orthographicSize = newSize;
        }
        else
        {
            mapCamera.fieldOfView = newSize;
        }
    }

    private void Pan(Vector2 delta)
    {
        Vector3 translation = new Vector3(-delta.x, 0, -delta.y) * moveSpeed * (mapCamera.orthographic ? mapCamera.orthographicSize : mapCamera.fieldOfView);
        mapCamera.transform.Translate(translation, Space.World);
    }

    private void ClampCameraPosition()
    {
        float currentAngle = mapCamera.transform.eulerAngles.x;
        if (currentAngle > 180f) currentAngle -= 360f;
        currentAngle = Mathf.Clamp(currentAngle, minTiltAngle, maxTiltAngle);

        mapCamera.transform.eulerAngles = new Vector3(
            currentAngle,
            mapCamera.transform.eulerAngles.y,
            mapCamera.transform.eulerAngles.z);

        Vector3 pos = mapCamera.transform.position;
        pos.y = Mathf.Clamp(pos.y, minZoom * 0.5f, maxZoom * 2f);
        mapCamera.transform.position = pos;

        Vector3 centerToCamera = mapCamera.transform.position - orbitCenter;
        centerToCamera.y = 0;
        float maxRadius = Mathf.Max(Mathf.Abs(mapBoundsX.y - mapBoundsX.x),
                                Mathf.Abs(mapBoundsZ.y - mapBoundsZ.x)) * 0.5f;

        if (centerToCamera.magnitude > maxRadius)
        {
            centerToCamera = centerToCamera.normalized * maxRadius;
            mapCamera.transform.position = new Vector3(
                orbitCenter.x + centerToCamera.x,
                mapCamera.transform.position.y,
                orbitCenter.z + centerToCamera.z);
        }
    }

    public void ResetCamera()
    {
        isTransitioning = false;
        ResetBuildingSelection();
        mapCamera.transform.position = originalCameraPosition;
        mapCamera.transform.rotation = originalCameraRotation;

        if (mapCamera.orthographic)
        {
            mapCamera.orthographicSize = minZoom;
        }
        else
        {
            mapCamera.fieldOfView = minZoom;
        }
    }

    public void ShowBuildingInfo(string info)
    {
        Debug.Log("Информация о корпусе: " + info);
    }
}