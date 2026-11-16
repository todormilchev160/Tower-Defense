using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MouseHoverDetector : MonoBehaviour
{
    [Header("Grass Settings")]
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private float grassHeight = 0f;

    private Camera cam;
    private HoverActivation currentHover;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current == null || cam == null)
            return;

        // Keep button active while mouse is over UI
        if (currentHover != null &&
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = cam.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        // Horizontal plane at grass height
        Plane grassPlane = new Plane(
            Vector3.up,
            new Vector3(0, grassHeight, 0)
        );

        HoverActivation newHover = null;

        if (grassPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);

            // Check which grass block occupies this position
            Collider[] hits = Physics.OverlapBox(
                worldPoint,
                new Vector3(0.01f, 0.1f, 0.01f),
                Quaternion.identity,
                grassLayer
            );

            foreach (Collider hit in hits)
            {
                HoverActivation hover =
                    hit.GetComponentInParent<HoverActivation>();

                if (hover != null)
                {
                    newHover = hover;
                    break;
                }
            }

            Debug.DrawLine(
                cam.transform.position,
                worldPoint,
                Color.green
            );
        }

        if (newHover != currentHover)
        {
            if (currentHover != null)
                currentHover.SetHover(false);

            if (newHover != null)
                newHover.SetHover(true);

            currentHover = newHover;
        }
    }
}