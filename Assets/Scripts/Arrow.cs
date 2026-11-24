
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float damage = 10f;
    public float damageLessDealtOnOrcs;

    [Header("Lifetime")]
    public float lifeTime = 5f;

    private Transform target;
    private Vector3 lastDirection;

    private bool hasHit = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            lastDirection =
                (target.position - transform.position).normalized;
        }
        else
        {
            lastDirection = transform.forward;
        }
    }

    void Update()
    {
        if (hasHit)
            return;

        // Follow the target while it exists
        if (target != null)
        {
            Vector3 direction =
                target.position - transform.position;

            if (direction.sqrMagnitude > 0.001f)
            {
                lastDirection = direction.normalized;
            }
        }

        // Continue flying in the last direction
        // if the target has been destroyed
        transform.position +=
            lastDirection * speed * Time.deltaTime;

        if (lastDirection.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(lastDirection);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        HitEnemy(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        HitEnemy(collision.collider);
    }

    void HitEnemy(Collider other)
    {
        if (hasHit)
            return;

        EnemyHealth enemyHealth =
            other.GetComponentInParent<EnemyHealth>();

        if (enemyHealth == null)
            return;

        if (enemyHealth.IsDead())
            return;

        hasHit = true;
        if(enemyHealth.orc)
        {
            enemyHealth.TakeDamage(damage-damageLessDealtOnOrcs);
        }
        else
        {
            enemyHealth.TakeDamage(damage);

        }

        Destroy(gameObject);
    }
}
