using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDrag : MonoBehaviour
{
    [Header("Movement")]
    public float dragSpeed = 0.02f;

    [Header("Maximum distance from start")]
    public float maxXDistance = 5f;
    public float maxZDistance = 3f;

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

        // Start dragging
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragging = true;
            lastMousePosition = Mouse.current.position.ReadValue();
        }

        // Stop dragging
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            dragging = false;
        }

        if (dragging)
        {
            Vector2 currentMousePosition =
                Mouse.current.position.ReadValue();

            Vector2 mouseDelta =
                currentMousePosition - lastMousePosition;

            // Mouse X -> World X
            // Mouse Y -> World Z
            Vector3 movement = new Vector3(
                -mouseDelta.x * dragSpeed,
                0f,
                -mouseDelta.y * dragSpeed
            );

            transform.position += movement;

            // Clamp camera position
            transform.position = new Vector3(
                Mathf.Clamp(
                    transform.position.x,
                    initialPosition.x - maxXDistance,
                    initialPosition.x + maxXDistance
                ),

                initialPosition.y,

                Mathf.Clamp(
                    transform.position.z,
                    initialPosition.z - maxZDistance,
                    initialPosition.z + maxZDistance
                )
            );

            lastMousePosition = currentMousePosition;
        }
    }
}