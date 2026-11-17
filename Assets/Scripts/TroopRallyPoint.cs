using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TroopRallyPoint : MonoBehaviour
{
    public static TroopRallyPoint Instance
    {
        get;
        private set;
    }


    [Header("Selection")]
    [SerializeField] private Camera mainCamera;

    [SerializeField]
    private float navMeshSearchDistance = 1f;


    [Header("NavMesh Visual")]
    [SerializeField]
    private Material navMeshMaterial;

    [SerializeField]
    private float visualHeightOffset = 0.05f;


    private GameObject navMeshVisual;

    private MeshFilter navMeshFilter;

    private bool selectingRallyPoint = false;

    private Barracks currentBarracks;

    public bool IsSelectingRallyPoint
    {
        get { return selectingRallyPoint; }
    }


    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }


        CreateVisualObject();


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


        if (
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject()
        )
        {
            return;
        }


        TryPlaceRallyPoint();
    }

    public void StartRallyPointSelection(
        Barracks barracks
    )
    {
        if (barracks == null)
            return;


        currentBarracks = barracks;

        selectingRallyPoint = true;
        GenerateRangeNavMesh();


        navMeshVisual.SetActive(true);
    }
    void TryPlaceRallyPoint()
    {
        if (currentBarracks == null || mainCamera == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        // First try to hit the NavMesh directly
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        Mesh mesh = new Mesh();
        mesh.vertices = triangulation.vertices;
        mesh.triangles = triangulation.indices;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        float closestDistance = Mathf.Infinity;
        Vector3 closestPoint = Vector3.zero;
        bool found = false;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 a = vertices[triangles[i]];
            Vector3 b = vertices[triangles[i + 1]];
            Vector3 c = vertices[triangles[i + 2]];

            if (RayIntersectsTriangle(ray, a, b, c, out float distance))
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = ray.GetPoint(distance);
                    found = true;
                }
            }
        }

        if (!found)
        {
            Debug.LogWarning("No NavMesh under mouse!");
            return;
        }

        if (!NavMesh.SamplePosition(
            closestPoint,
            out NavMeshHit navHit,
            navMeshSearchDistance,
            NavMesh.AllAreas))
        {
            return;
        }

        Vector3 barracksPosition = currentBarracks.transform.position;
        Vector3 rallyPosition = navHit.position;

        barracksPosition.y = 0f;
        rallyPosition.y = 0f;

        float distanceToBarracks =
            Vector3.Distance(barracksPosition, rallyPosition);

        if (distanceToBarracks > currentBarracks.GetRallyRange())
        {
            Debug.LogWarning("Rally point outside range!");
            return;
        }

        currentBarracks.SetRallyPoint(navHit.position);

        selectingRallyPoint = false;
        currentBarracks = null;

        navMeshVisual.SetActive(false);
    }
    bool RayIntersectsTriangle(
        Ray ray,
        Vector3 a,
        Vector3 b,
        Vector3 c,
        out float distance)
    {
        distance = 0f;

        Vector3 edge1 = b - a;
        Vector3 edge2 = c - a;

        Vector3 h = Vector3.Cross(ray.direction, edge2);
        float determinant = Vector3.Dot(edge1, h);

        if (Mathf.Abs(determinant) < 0.000001f)
            return false;

        float inverse = 1f / determinant;

        Vector3 s = ray.origin - a;
        float u = inverse * Vector3.Dot(s, h);

        if (u < 0f || u > 1f)
            return false;

        Vector3 q = Vector3.Cross(s, edge1);
        float v = inverse * Vector3.Dot(ray.direction, q);

        if (v < 0f || u + v > 1f)
            return false;

        distance = inverse * Vector3.Dot(edge2, q);

        return distance > 0f;
    }

    void CreateVisualObject()
    {
        navMeshVisual =
            new GameObject(
                "NavMesh Rally Range Visual"
            );


        navMeshFilter =
            navMeshVisual.AddComponent<MeshFilter>();


        MeshRenderer renderer =
            navMeshVisual.AddComponent<MeshRenderer>();


        renderer.material =
            navMeshMaterial;
    }
    void GenerateRangeNavMesh()
    {
        if (currentBarracks == null)
            return;


        NavMeshTriangulation triangulation =
            NavMesh.CalculateTriangulation();


        List<Vector3> vertices =
            new List<Vector3>();


        List<int> triangles =
            new List<int>();


        Vector3 barracksPosition =
            currentBarracks.transform.position;


        float range =
            currentBarracks.GetRallyRange();
        for (
            int i = 0;
            i < triangulation.indices.Length;
            i += 3
        )
        {
            Vector3 pointA =
                triangulation.vertices[
                    triangulation.indices[i]
                ];


            Vector3 pointB =
                triangulation.vertices[
                    triangulation.indices[i + 1]
                ];


            Vector3 pointC =
                triangulation.vertices[
                    triangulation.indices[i + 2]
                ];


            // Center of this NavMesh triangle
            Vector3 triangleCenter =
                (
                    pointA +
                    pointB +
                    pointC
                )
                / 3f;


            float distance =
                Vector3.Distance(
                    barracksPosition,
                    triangleCenter
                );


            // Ignore triangles outside range
            if (distance > range)
                continue;


            int startIndex =
                vertices.Count;


            // Raise visual slightly
            // so it doesn't flicker with the ground
            Vector3 offset =
                Vector3.up *
                visualHeightOffset;


            vertices.Add(
                pointA + offset
            );


            vertices.Add(
                pointB + offset
            );


            vertices.Add(
                pointC + offset
            );


            triangles.Add(
                startIndex
            );


            triangles.Add(
                startIndex + 1
            );


            triangles.Add(
                startIndex + 2
            );
        }


        Mesh mesh =
            new Mesh();


        mesh.SetVertices(
            vertices
        );


        mesh.SetTriangles(
            triangles,
            0
        );


        mesh.RecalculateNormals();

        mesh.RecalculateBounds();


        // Remove old generated mesh
        if (navMeshFilter.mesh != null)
        {
            Destroy(
                navMeshFilter.mesh
            );
        }


        navMeshFilter.mesh =
            mesh;
    }
}