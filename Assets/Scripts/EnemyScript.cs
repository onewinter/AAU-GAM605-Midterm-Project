using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // patrol variables
    [SerializeField] private Transform[] pathPoints;
    [SerializeField] private float moverSpeed = 1f;
    // how much health to take from the player on impact
    [SerializeField] private int damage = 1;

    private int current;
    private Vector3 target;
    private float dist = .1f;
    private Rigidbody2D rigidbody2d;
    private Vector3 startPos;
    private GameManager gameManager;

    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        startPos = transform.position;
    }

    private void Update()
    {
        // only run if paths defined
        if (pathPoints.Length == 0) return;

        // get our current target & move towards it
        target = pathPoints[current].position;

        // if we are close enough, switch to the next target
        if (Vector3.Distance(transform.position, target) < dist) current++;

        // switch back to the first item in the array once we reach the end
        if (current >= pathPoints.Length) current = 0;
    }

    private void FixedUpdate()
    {
        // don't move if game manager is paused
        if (gameManager.CanMove())
            // use normalized velocity to move (for the animator)
            rigidbody2d.velocity = Vector3.Normalize(target - transform.position) * moverSpeed;
        else
            rigidbody2d.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // damage the player upon collisions
        if (!other.gameObject.CompareTag("Player")) return;
        other.gameObject.GetComponent<PlayerScript>().TakeDamage(damage);
    }

    public void ResetPosition()
    {
        // reset position to start/restart game and ensure GO is active
        transform.position = startPos;
        current = 0;
        gameObject.SetActive(true);
    }
}