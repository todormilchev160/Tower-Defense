using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TroopRallyPoint : MonoBehaviour
{
    public static Vector3 RallyPosition { get; private set; }
    public static bool HasRallyPoint { get; private set; }

    [Header("Selection")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float navMeshSearchDistance = 1f;

    [Header("NavMesh Visual")]
    [SerializeField] private Material navMeshMaterial;
    [SerializeField] private float visualHeightOffset = 0.05f;

    private GameObject navMeshVisual;
    private bool selectingRallyPoint = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        CreateNavMeshVisual();

        if (navMeshVisual != null)
            navMeshVisual.SetActive(false);


    }

    void Update()
    {
        if (!selectingRallyPoint)
            return;

        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // Don't place rally point when clicking UI
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        TryPlaceRallyPoint();
    }

    // Connect your UI button to this
    public void StartRallyPointSelection()
    {
        selectingRallyPoint = true;

        if (navMeshVisual != null)
            navMeshVisual.SetActive(true);
    }

    void TryPlaceRallyPoint()
    {
        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Ray ray =
            mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            NavMeshHit navHit;

            if (NavMesh.SamplePosition(
                hit.point,
                out navHit,
                navMeshSearchDistance,
                NavMesh.AllAreas))
            {
                SetRallyPoint(navHit.position);
            }
        }
    }

    void SetRallyPoint(Vector3 position)
    {
        RallyPosition = position;
        HasRallyPoint = true;

        selectingRallyPoint = false;

        if (navMeshVisual != null)
            navMeshVisual.SetActive(false);
    }

    void CreateNavMeshVisual()
    {
        NavMeshTriangulation triangulation =
            NavMesh.CalculateTriangulation();

        if (triangulation.vertices.Length == 0)
        {
            Debug.LogWarning("No NavMesh found.");
            return;
        }

        Vector3[] vertices =
            new Vector3[triangulation.vertices.Length];

        // Raise it slightly so it doesn't flicker with the ground
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] =
                triangulation.vertices[i]
                + Vector3.up * visualHeightOffset;
        }

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangulation.indices;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        navMeshVisual =
            new GameObject("NavMesh Rally Visual");

        MeshFilter meshFilter =
            navMeshVisual.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer =
            navMeshVisual.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;
        meshRenderer.material = navMeshMaterial;

        navMeshVisual.SetActive(false);
    }
}