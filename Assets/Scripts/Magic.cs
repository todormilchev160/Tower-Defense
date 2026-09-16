using UnityEngine;

public class Magic : MonoBehaviour
{
    public float startingSpeed = 0.5f;
    public float acceleration = 8f;
    public float maxSpeed = 35f;
    public float lifeTime = 8f;
    public float damage=40;

    private Vector3 direction;
    private float currentSpeed;
    private EnemyHealth enemyHealth;

    void Start()
    {
        currentSpeed = startingSpeed;
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Update()
    {
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        transform.position += direction * currentSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyHealth=other.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}