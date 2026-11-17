
using UnityEngine;
using UnityEngine.InputSystem;

public class NewCameraMovement : MonoBehaviour
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
    [SerializeField] private float rotationSmoothTime = 0.08f;

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

    private float yaw;
    private float pitch;
    private float targetYaw;
    private float targetPitch;
    private float yawVelocity;
    private float pitchVelocity;

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

        if (rotationTarget != null)
        {
            Vector3 direction =
                rotationTarget.position - transform.position;

            Quaternion lookRotation =
                Quaternion.LookRotation(direction, Vector3.up);

            yaw = lookRotation.eulerAngles.y;
            pitch = NormalizeAngle(lookRotation.eulerAngles.x);
        }
        else
        {
            yaw = transform.eulerAngles.y;
            pitch = NormalizeAngle(transform.eulerAngles.x);
        }

        targetYaw = yaw;
        targetPitch = Mathf.Clamp(
            pitch,
            minVerticalAngle,
            maxVerticalAngle
        );
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        HandleMovement();
        HandleRotation();
        HandleZoom();

        ClampPosition();
        UpdateCameraRotation();
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

            // Move both the camera and its orbit target
            transform.position += movement;

            if (rotationTarget != null)
                rotationTarget.position += movement;

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

            activeRotationAxis =
                Mathf.Abs(totalMovement.x) >= Mathf.Abs(totalMovement.y)
                ? RotationAxis.Horizontal
                : RotationAxis.Vertical;

            lastMousePosition = currentMousePosition;
            return;
        }

        Vector2 mouseDelta =
            currentMousePosition - lastMousePosition;

        if (activeRotationAxis == RotationAxis.Horizontal)
        {
            targetYaw += mouseDelta.x * rotationSpeed;
        }
        else if (activeRotationAxis == RotationAxis.Vertical)
        {
            targetPitch = Mathf.Clamp(
                targetPitch - mouseDelta.y * rotationSpeed,
                minVerticalAngle,
                maxVerticalAngle
            );
        }

        lastMousePosition = currentMousePosition;
    }

    void UpdateCameraRotation()
    {
        yaw = Mathf.SmoothDampAngle(
            yaw,
            targetYaw,
            ref yawVelocity,
            rotationSmoothTime
        );

        pitch = Mathf.SmoothDampAngle(
            pitch,
            targetPitch,
            ref pitchVelocity,
            rotationSmoothTime
        );

        Quaternion newRotation = Quaternion.Euler(
            pitch,
            yaw,
            0f
        );

        if (rotationTarget != null)
        {
            float distance = Vector3.Distance(
                transform.position,
                rotationTarget.position
            );

            transform.position =
                rotationTarget.position -
                newRotation * Vector3.forward * distance;
        }

        transform.rotation = newRotation;
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
                rotationTarget.position -
                transform.forward * newDistance;
        }
    }

    void ClampPosition()
    {
        Vector3 position = transform.position;

        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(position.x, minX, maxX),
            position.y,
            Mathf.Clamp(position.z, minZ, maxZ)
        );

        Vector3 difference = clampedPosition - position;

        transform.position = clampedPosition;

        if (rotationTarget != null)
            rotationTarget.position += difference;
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}