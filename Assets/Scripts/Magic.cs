using UnityEngine;

public class Magic : MonoBehaviour
{
    [Header("Movement")]
    public float startingSpeed = 0.5f;
    public float acceleration = 8f;
    public float maxSpeed = 35f;
    public float lifeTime = 8f;

    [Header("Damage")]
    public float damage = 40f;
    public LayerMask hitLayers = ~0;

    private Vector3 direction;
    private float currentSpeed;
    private Collider[] ownColliders;

    void Awake()
    {
        currentSpeed = startingSpeed;
        ownColliders = GetComponentsInChildren<Collider>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;

        if (direction.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    void Update()
    {
        currentSpeed = Mathf.Min(
            currentSpeed + acceleration * Time.deltaTime,
            maxSpeed
        );

        float distance = currentSpeed * Time.deltaTime;

        if (Physics.Raycast(
            transform.position,
            direction,
            out RaycastHit hit,
            distance,
            hitLayers,
            QueryTriggerInteraction.Collide))
        {
            // Ignore colliders belonging to the projectile itself
            bool ownHit = false;

            foreach (Collider col in ownColliders)
            {
                if (hit.collider == col)
                {
                    ownHit = true;
                    break;
                }
            }

            if (!ownHit)
            {
                EnemyHealth health =
                    hit.collider.GetComponentInParent<EnemyHealth>();

                if (health != null)
                {
                    health.TakeDamage(damage);
                    Destroy(gameObject);
                    return;
                }
            }
        }

        transform.position += direction * distance;
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyHealth health =
            other.GetComponentInParent<EnemyHealth>();

        if (health != null)
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}