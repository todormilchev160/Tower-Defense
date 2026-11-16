
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0.02f;

    [Header("Movement Boundaries")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minZ = -10f;
    [SerializeField] private float maxZ = 10f;

    [Header("Rotation")]
    [SerializeField] private Transform rotationTarget;
    [SerializeField] private float rotationSpeed = 0.2f;
    [SerializeField] private float rotationDetectionThreshold = 5f;

    [Header("Vertical Rotation Limits")]
    [SerializeField] private float minVerticalAngle = -85f;
    [SerializeField] private float maxVerticalAngle = 85f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 20f;

    private Camera cam;
    private Vector2 lastMousePosition;
    private Vector2 rotationStartMousePosition;

    private enum RotationAxis
    {
        None,
        Horizontal,
        Vertical
    }

    private RotationAxis activeRotationAxis = RotationAxis.None;

    void Start()
    {
        cam = GetComponent<Camera>();
        RemoveTilt();
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        HandleMovement();
        HandleRotation();
        HandleZoom();

        // Keep the camera inside the movement boundaries
        ClampPosition();

        // Prevent unwanted camera roll
        RemoveTilt();
    }

    void HandleMovement()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            lastMousePosition = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.leftButton.isPressed &&
            !Mouse.current.rightButton.isPressed)
        {
            Vector2 currentMousePosition =
                Mouse.current.position.ReadValue();

            Vector2 mouseDelta =
                currentMousePosition - lastMousePosition;

            Vector3 right = transform.right;
            right.y = 0f;
            right.Normalize();

            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 movement =
                (-right * mouseDelta.x -
                 forward * mouseDelta.y) * moveSpeed;

            transform.position += movement;

            lastMousePosition = currentMousePosition;
        }
    }

    void HandleRotation()
    {
        if (rotationTarget == null)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            lastMousePosition = Mouse.current.position.ReadValue();
            rotationStartMousePosition = lastMousePosition;
            activeRotationAxis = RotationAxis.None;
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            activeRotationAxis = RotationAxis.None;
            return;
        }

        if (!Mouse.current.rightButton.isPressed)
            return;

        Vector2 currentMousePosition =
            Mouse.current.position.ReadValue();

        if (activeRotationAxis == RotationAxis.None)
        {
            Vector2 totalMovement =
                currentMousePosition - rotationStartMousePosition;

            if (totalMovement.magnitude < rotationDetectionThreshold)
                return;

            if (Mathf.Abs(totalMovement.x) >= Mathf.Abs(totalMovement.y))
            {
                activeRotationAxis = RotationAxis.Horizontal;
            }
            else
            {
                activeRotationAxis = RotationAxis.Vertical;
            }

            lastMousePosition = currentMousePosition;
            return;
        }

        Vector2 mouseDelta =
            currentMousePosition - lastMousePosition;

        Vector3 pivot = rotationTarget.position;

        if (activeRotationAxis == RotationAxis.Horizontal)
        {
            transform.RotateAround(
                pivot,
                Vector3.up,
                mouseDelta.x * rotationSpeed
            );
        }
        else if (activeRotationAxis == RotationAxis.Vertical)
        {
            Vector3 offset = transform.position - pivot;

            float horizontalDistance =
                new Vector2(offset.x, offset.z).magnitude;

            float currentAngle = Mathf.Atan2(
                offset.y,
                horizontalDistance
            ) * Mathf.Rad2Deg;

            float desiredAngle = Mathf.Clamp(
                currentAngle - mouseDelta.y * rotationSpeed,
                minVerticalAngle,
                maxVerticalAngle
            );

            float distance = offset.magnitude;

            float angleRad = desiredAngle * Mathf.Deg2Rad;

            Vector3 horizontalDirection =
                new Vector3(offset.x, 0f, offset.z).normalized;

            Vector3 newOffset =
                horizontalDirection *
                (Mathf.Cos(angleRad) * distance);

            newOffset.y = Mathf.Sin(angleRad) * distance;

            transform.position = pivot + newOffset;
        }

       // Always look towards the rotation target after orbiting
Vector3 lookDirection = rotationTarget.position - transform.position;

if (lookDirection.sqrMagnitude > 0.000001f)
{
    transform.rotation = Quaternion.LookRotation(
        lookDirection,
        Vector3.up
    );
}

lastMousePosition = currentMousePosition;
    }

    void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Approximately(scroll, 0f))
            return;

        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize -
                scroll * zoomSpeed * 0.01f,
                minZoom,
                maxZoom
            );
        }
        else if (rotationTarget != null)
        {
            Vector3 direction =
                (transform.position -
                 rotationTarget.position).normalized;

            float distance = Vector3.Distance(
                transform.position,
                rotationTarget.position
            );

            float newDistance = Mathf.Clamp(
                distance - scroll * zoomSpeed * 0.01f,
                minZoom,
                maxZoom
            );

            transform.position =
                rotationTarget.position +
                direction * newDistance;
        }
    }

    void ClampPosition()
    {
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.z = Mathf.Clamp(position.z, minZ, maxZ);

        transform.position = position;
    }

    void RemoveTilt()
    {
        Vector3 forward = transform.forward;

        if (forward.sqrMagnitude > 0.000001f &&
            Vector3.Cross(Vector3.up, forward).sqrMagnitude
            > 0.000001f)
        {
            transform.rotation = Quaternion.LookRotation(
                forward,
                Vector3.up
            );
        }
    }
}
