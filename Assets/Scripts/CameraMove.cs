
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0.02f;

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
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        HandleMovement();
        HandleRotation();
        HandleZoom();
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

        // Reset axis when right-click begins
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            lastMousePosition = Mouse.current.position.ReadValue();

            rotationStartMousePosition = lastMousePosition;

            activeRotationAxis = RotationAxis.None;
        }

        // Reset axis when right-click ends
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            activeRotationAxis = RotationAxis.None;
            return;
        }

        if (!Mouse.current.rightButton.isPressed)
            return;

        Vector2 currentMousePosition =
            Mouse.current.position.ReadValue();

        // Determine the initial dragging direction
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

        // ===================================
        // HORIZONTAL ROTATION ONLY
        // ===================================

        if (activeRotationAxis == RotationAxis.Horizontal)
        {
            transform.RotateAround(
                pivot,
                Vector3.up,
                mouseDelta.x * rotationSpeed
            );
        }

        // ===================================
        // VERTICAL ROTATION ONLY
        // ===================================

        else if (activeRotationAxis == RotationAxis.Vertical)
        {
            Vector3 offset =
                transform.position - pivot;

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

            float angleChange =
                desiredAngle - currentAngle;

            Vector3 horizontalDirection = new Vector3(
                offset.x,
                0f,
                offset.z
            );

            if (horizontalDirection.sqrMagnitude > 0.000001f)
            {
                Vector3 orbitRight = Vector3.Cross(
                    Vector3.up,
                    horizontalDirection.normalized
                ).normalized;

                transform.RotateAround(
                    pivot,
                    orbitRight,
                    -angleChange
                );
            }
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
                cam.orthographicSize - scroll * zoomSpeed * 0.01f,
                minZoom,
                maxZoom
            );
        }
        else
        {
            transform.position +=
                transform.forward * scroll * zoomSpeed * 0.01f;
        }
    }
}
