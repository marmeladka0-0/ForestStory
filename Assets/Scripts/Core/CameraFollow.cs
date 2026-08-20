using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private Transform grandfather;
    [SerializeField] private Transform granddaughter;

    [Header("Camera settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

    [Header("2D Camera Size (Zoom)")]
    [SerializeField] private float cameraSize = 7f; // Standard 2D size (increase to zoom out, decrease to zoom in)

    [Header("Map Bounds")]
    [SerializeField] private bool useBounds = false;  // Bounds for the camera
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Camera cam;
    private int currentSelectedID = 0; // 0 - both, 1 - grandfather, 2 - granddaughter

    private void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateCameraSize();
    }

    private void OnValidate()
    {
        // Updates camera size instantly in the Unity Editor when changing the slider/value
        if (cam == null) cam = GetComponent<Camera>();
        UpdateCameraSize();
    }

    private void OnEnable()
    {
        EventManager.OnCharacterSelected += HandleSelectionChanged;
    }

    private void OnDisable()
    {
        EventManager.OnCharacterSelected -= HandleSelectionChanged;
    }

    private void HandleSelectionChanged(int selectedID)
    {
        currentSelectedID = selectedID;
    }

    private void LateUpdate()
    {
        if (grandfather == null || granddaughter == null) return;

        Vector3 targetPosition;

        // Calculate target position based on selected character(s)
        if (currentSelectedID == 1)
        {
            targetPosition = grandfather.position;
        }
        else if (currentSelectedID == 2)
        {
            targetPosition = granddaughter.position;
        }
        else
        {
            targetPosition = (grandfather.position + granddaughter.position) / 2f;
        }

        // Apply offset (Z-axis offset is essential for 2D cameras)
        targetPosition += offset;

        // Clamp camera position within map bounds if enabled
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }

        // Smoothly interpolate camera position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    // Apply orthographic size to the camera component
    private void UpdateCameraSize()
    {
        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = cameraSize;
        }
    }
}