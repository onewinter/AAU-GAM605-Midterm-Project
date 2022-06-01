using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float firingSpeed = .5f;
    [SerializeField] private bool hasKey;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int health = 10;

    private Rigidbody2D rigidbody2d;
    private GameManager gameManager;
    private SpriteFlash spriteFlash;
    private AudioSource audioSource;
    private int startHealth;
    private float objectWidth;
    private float objectHeight;
    private float timeSinceShot;
    private float timeSinceDamaged;
    private Vector2 newVelocity;
    private Vector2 screenBounds;
    private Vector3 start;
    private Vector3 direction;

    private void Start()
    {
        // initialize some variables
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteFlash = GetComponent<SpriteFlash>();
        audioSource = GetComponent<AudioSource>();
        start = transform.position;
        timeSinceShot = firingSpeed;
        direction = Vector2.down;
        startHealth = health;
        gameManager = FindObjectOfType<GameManager>();

        // get our screen area and player sprite size for clamping
        screenBounds =
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        objectHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
    }

    private void FixedUpdate()
    {
        // don't move if game manager is paused
        if (gameManager.CanMove())
            // move the character based on player input
            rigidbody2d.velocity = newVelocity * playerSpeed;
        else
            rigidbody2d.velocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        // clamp player inside the play space
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth),
            Mathf.Clamp(transform.position.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight),
            0f);
    }

    private void Update()
    {
        // don't move if game manager is paused
        if (!gameManager.CanMove()) return;

        // get keyboard input
        var axisH = Input.GetAxis("Horizontal");
        var axisV = Input.GetAxis("Vertical");
        newVelocity = new Vector2(axisH, axisV);

        // store direction for firing bullets
        if (axisH != 0 || axisV != 0)
            direction = newVelocity.normalized;

        // fire bullets when Space key is held down
        if (Input.GetKey("space"))
            // limit firing rate
            if (timeSinceShot >= firingSpeed)
            {
                timeSinceShot = 0;
                var newBullet = Instantiate(bulletPrefab, transform.position + direction, Quaternion.Euler(0, 0, 0));
                // make the bullet move
                newBullet.GetComponent<Rigidbody2D>().AddForce(direction * bulletSpeed);
            }

        timeSinceShot += Time.deltaTime;
        timeSinceDamaged += Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        // only take damage if it's been more than a second
        if (timeSinceDamaged < 1f) return;

        // change the health value and log name/new value to console
        health -= damage;
        gameManager.UpdateHealth(health);
        spriteFlash.Flash();
        audioSource.Play();
        timeSinceDamaged = 0;
        Debug.Log(gameObject.name + " health value is now: " + health);

        // if health isn't 0 or less, don't do anything else
        if (health > 0) return;

        // object is now dead
        Debug.Log(gameObject.name + " is now dead.");

        // reset the player & game (lose)
        ResetPlayer();
        gameManager.ResetGame(false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // pick up key
        if (other.gameObject.CompareTag("Key"))
        {
            hasKey = true;
            other.gameObject.SetActive(false);
        }
        // unlock lock with key
        else if (other.gameObject.CompareTag("Lock") && hasKey)
        {
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Goal"))
        {
            // reset player & game (win)
            ResetPlayer();
            gameManager.ResetGame(true);
        }
    }

    public void ResetPlayer()
    {
        health = startHealth;
        gameManager.UpdateHealth(health);
        transform.position = start;
        hasKey = false;
    }
}