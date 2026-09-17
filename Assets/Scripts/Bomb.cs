using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float travelTime = 1.2f;
    [SerializeField] private float arcHeight = 5f;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float damage = 5f;

    [Header("Enemy")]
    [SerializeField] private string enemyTag = "Enemy";

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private float timer;
    private bool launched = false;

    public float TravelTime => travelTime;

    public void SetTargetPosition(Vector3 position)
    {
        startPosition = transform.position;
        targetPosition = position;

        timer = 0f;
        launched = true;
    }

    void Update()
    {
        if (!launched)
            return;

        timer += Time.deltaTime;

        float t = timer / travelTime;

        if (t >= 1f)
        {
            transform.position = targetPosition;
            Explode();
            return;
        }

        // Move normally from start to target
        Vector3 position = Vector3.Lerp(
            startPosition,
            targetPosition,
            t
        );

        // Create the arc
        float arc = 4f * arcHeight * t * (1f - t);

        position.y += arc;

        transform.position = position;
    }

    void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius
        );

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag(enemyTag))
                continue;

            EnemyHealth enemyHealth =
                hit.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
            {
                enemyHealth =
                    hit.GetComponentInParent<EnemyHealth>();
            }

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}