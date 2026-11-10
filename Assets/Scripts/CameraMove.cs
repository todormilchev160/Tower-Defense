using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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


        // =================================================
        // DON'T MOVE CAMERA WHILE SELECTING RALLY POINT
        // =================================================

        if (
            TroopRallyPoint.Instance != null &&
            TroopRallyPoint.Instance.IsSelectingRallyPoint
        )
        {
            dragging = false;
            return;
        }


        // =================================================
        // DON'T START DRAGGING WHEN CLICKING UI
        // =================================================

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (
                EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject()
            )
            {
                return;
            }

            dragging = true;

            lastMousePosition =
                Mouse.current.position.ReadValue();
        }


        // =================================================
        // STOP DRAGGING
        // =================================================

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            dragging = false;
        }


        // =================================================
        // DRAG
        // =================================================

        if (dragging)
        {
            Vector2 currentMousePosition =
                Mouse.current.position.ReadValue();


            Vector2 mouseDelta =
                currentMousePosition -
                lastMousePosition;


            Vector3 movement =
                new Vector3(
                    -mouseDelta.x * dragSpeed,
                    0f,
                    -mouseDelta.y * dragSpeed
                );


            transform.position += movement;


            // Clamp camera
            transform.position =
                new Vector3(
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


            lastMousePosition =
                currentMousePosition;
        }
    }
}