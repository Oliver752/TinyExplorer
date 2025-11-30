using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 20f;
    public float lifeTime = 2f;

    private Rigidbody rb;
    private bool hasHit = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Correct property name is "velocity"
        rb.linearVelocity = transform.forward * speed;
        rb.useGravity = false;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // OPTIONAL: ignore player hitbox if needed
        if (other.CompareTag("Player")) return;

        // Look for EnemyHealth on this object or its parents (covers multi-collider enemies)
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            hasHit = true;
            enemy.TakeDamage(damage);

            Destroy(gameObject);   // ❗ only destroy if we actually hit an enemy
        }

        // ❌ DO NOT destroy the bullet here for non-enemy hits
        // so it can pass through ground / faulty hitboxes, and die only by lifeTime
    }
}
