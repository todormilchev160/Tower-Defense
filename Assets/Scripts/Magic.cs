
using UnityEngine;

public class Magic : MonoBehaviour
{
    [Header("Movement")]
    public float startingSpeed = 0.5f;
    public float acceleration = 8f;
    public float maxSpeed = 35f;
    public float lifeTime = 8f;

    [Header("Collision")]
    public float collisionRadius = 0.15f;

    [Header("Damage")]
    public float damage = 40f;
    public LayerMask hitLayers = ~0;
    public float damageMoreDealtOnOrcs;

    private Vector3 direction;
    private float currentSpeed;
    private Collider[] ownColliders;
    private bool hasHit;

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
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Update()
    {
        if (hasHit || direction.sqrMagnitude < 0.0001f)
            return;

        currentSpeed = Mathf.Min(
            currentSpeed + acceleration * Time.deltaTime,
            maxSpeed
        );

        float distance = currentSpeed * Time.deltaTime;

        // Check the entire movement path with a small radius.
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            collisionRadius,
            direction,
            distance,
            hitLayers,
            QueryTriggerInteraction.Collide
        );

        // Sort hits so the closest enemy is checked first.
        System.Array.Sort(hits,
            (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            Collider col = hit.collider;

           
            if (col.transform == transform ||
                col.transform.IsChildOf(transform))
            {
                continue;
            }

            EnemyHealth health =
                col.GetComponentInParent<EnemyHealth>();
            if (health == null)
                continue;

            HitEnemy(health);
            return;
        }

        transform.position += direction * distance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        // Respect the configured hit layers.
        if (((1 << other.gameObject.layer) & hitLayers.value) == 0)
            return;

        // Ignore projectile colliders.
        if (other.transform == transform ||
            other.transform.IsChildOf(transform))
            return;

        EnemyHealth health =
            other.GetComponentInParent<EnemyHealth>();

        if (health != null)
        {
            HitEnemy(health);
        }
    }

    void HitEnemy(EnemyHealth health)
    {
        if (hasHit)
            return;

        hasHit = true;
        if(health.orc)
        {
            health.TakeDamage(damage+damageMoreDealtOnOrcs);
        }
        else
        {
             health.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
