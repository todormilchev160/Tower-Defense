using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDrag : MonoBehaviour
{
    [Header("Movement")]
    public float dragSpeed = 0.02f;

    [Header("Maximum distance from start")]
    public float maxXDistance = 5f;
    public float maxYDistance = 3f;

    private Vector3 initialPosition;
    private Vector2 lastMousePosition;
    private bool dragging;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragging = true;
            lastMousePosition = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            dragging = false;
        }

        if (dragging)
        {
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();
            Vector2 mouseDelta = currentMousePosition - lastMousePosition;

            Vector3 movement = new Vector3(
                -mouseDelta.x * dragSpeed,
                -mouseDelta.y * dragSpeed,
                0
            );

            transform.position += movement;

            transform.position = new Vector3(
                Mathf.Clamp(
                    transform.position.x,
                    initialPosition.x - maxXDistance,
                    initialPosition.x + maxXDistance
                ),
                Mathf.Clamp(
                    transform.position.y,
                    initialPosition.y - maxYDistance,
                    initialPosition.y + maxYDistance
                ),
                initialPosition.z
            );

            lastMousePosition = currentMousePosition;
        }
    }
}