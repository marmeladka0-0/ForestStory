using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private Transform grandfather;  
    [SerializeField] private Transform granddaughter;  

    [Header("Camera settings")]
    [SerializeField] private float smoothSpeed = 5f;     
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

    [Header("Map Bounds")]
    [SerializeField] private bool useBounds = false;  //bounds for the camera
    [SerializeField] private Vector2 minBounds;          
    [SerializeField] private Vector2 maxBounds;          

    private int currentSelectedID = 0; //who is selected

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

        //Count the position of the camera
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

        //Add a Z offset (-10?) to a targetPosition
        targetPosition += offset;

        //If there are some bounds on the map and it is the turn on
        //=> we add some global bounds for camera too
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }

        //move a camera to a target point
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
