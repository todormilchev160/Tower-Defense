
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TroopRangeVisualizer : MonoBehaviour
{
    [Header("Range")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private int segments = 100;
    [SerializeField] private float heightOffset = 0.1f;

    [Header("Appearance")]
    [SerializeField] private float lineWidth = 0.08f;
    [SerializeField] private Color lineColor = Color.red;

    [Header("Visibility")]
    [SerializeField] private bool showAlways = true;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineRenderer.material = new Material(
            Shader.Find("Sprites/Default")
        );

        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        DrawCircle();

        lineRenderer.enabled = showAlways;
    }

    void DrawCircle()
    {
        lineRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            float x = Mathf.Cos(angle) * attackRange;
            float z = Mathf.Sin(angle) * attackRange;

            lineRenderer.SetPosition(
                i,
                new Vector3(x, heightOffset, z)
            );
        }
    }

    public void SetRange(float range)
    {
        attackRange = range;
        DrawCircle();
    }

    public void ShowRange()
    {
        lineRenderer.enabled = true;
    }

    public void HideRange()
    {
        lineRenderer.enabled = false;
    }
}
