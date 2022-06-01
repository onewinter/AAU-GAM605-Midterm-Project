using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private readonly int damage = 1;
    [SerializeField] private readonly float liveDuration = 3f;
    [SerializeField] private readonly float rotationSpeed = 2f;

    private void Start()
    {
        // bullet will self-destruct after x time
        Destroy(gameObject, liveDuration);
    }

    private void Update()
    {
        // rotate the bullet as it goes
        transform.Rotate(new Vector3(0, 0, -45) * rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // bounce off player and other bullets
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bullet")) return;

        // damage an enemy
        if (other.gameObject.CompareTag("Enemy"))
            other.gameObject.GetComponent<HealthScript>().TakeDamage(damage);

        // destroy the bullet upon impact
        Destroy(gameObject);
    }
}