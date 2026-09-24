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


    // =====================================================
    // START SELECTION
    // =====================================================

    public void StartRallyPointSelection(
        Barracks barracks
    )
    {
        if (barracks == null)
            return;


        currentBarracks = barracks;

        selectingRallyPoint = true;


        // Generate ONLY the NavMesh inside this
        // Barracks' rally range
        GenerateRangeNavMesh();


        navMeshVisual.SetActive(true);
    }


    // =====================================================
    // PLACE RALLY POINT
    // =====================================================

    void TryPlaceRallyPoint()
    {
        if (currentBarracks == null)
            return;


        Vector2 mousePosition =
            Mouse.current.position.ReadValue();


        Ray ray =
            mainCamera.ScreenPointToRay(
                mousePosition
            );


        if (!Physics.Raycast(
            ray,
            out RaycastHit hit
        ))
        {
            return;
        }


        NavMeshHit navHit;


        bool foundNavMesh =
            NavMesh.SamplePosition(
                hit.point,
                out navHit,
                navMeshSearchDistance,
                NavMesh.AllAreas
            );


        if (!foundNavMesh)
            return;


        // ==========================================
        // RANGE CHECK
        // ==========================================

        float distance =
            Vector3.Distance(
                currentBarracks.transform.position,
                navHit.position
            );


        if (
            distance >
            currentBarracks.GetRallyRange()
        )
        {
            Debug.Log(
                "Rally point is outside the allowed range!"
            );

            return;
        }


        // ==========================================
        // SET RALLY POINT
        // ==========================================

        currentBarracks.SetRallyPoint(
            navHit.position
        );


        selectingRallyPoint = false;

        currentBarracks = null;


        navMeshVisual.SetActive(false);
    }


    // =====================================================
    // CREATE VISUAL OBJECT
    // =====================================================

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


    // =====================================================
    // GENERATE NAVMESH INSIDE RANGE
    // =====================================================

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


        // Go through every NavMesh triangle
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