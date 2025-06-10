using UnityEngine;
using UnityEngine.EventSystems;

public class ARObjectTapHandler : MonoBehaviour
{
    [Header("Selection Settings")]
    public Color highlightColor = Color.yellow;
    public float highlightDuration = 0.5f;

    private Transform selectedObject;
    private Renderer objectRenderer;
    private Color originalColor;
    private float highlightTimer;

    void Update()
    {
        if (highlightTimer > 0)
        {
            highlightTimer -= Time.deltaTime;
            if (highlightTimer <= 0)
            {
                ResetSelection();
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleTap(Input.GetTouch(0).position);
        }
    }

    private void HandleTap(Vector2 screenPosition)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var tapHandler = hit.transform.GetComponent<IARTappable>();
            if (tapHandler != null)
            {
                ResetSelection();

                selectedObject = hit.transform;
                objectRenderer = selectedObject.GetComponent<Renderer>();

                if (objectRenderer != null)
                {
                    originalColor = objectRenderer.material.color;
                    objectRenderer.material.color = highlightColor;
                    highlightTimer = highlightDuration;
                }

                tapHandler.OnTap();
            }
        }
        else
        {
            ResetSelection();
        }
    }

    private void ResetSelection()
    {
        if (selectedObject != null && objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }

        selectedObject = null;
        objectRenderer = null;
        highlightTimer = 0;
    }
}
public interface IARTappable
{
    void OnTap();
}