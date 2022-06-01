using UnityEngine;

public class TeleportScript : MonoBehaviour
{
    // where to teleport to
    [SerializeField] private Transform target;

    // how long to pause after teleport
    private float pauseAfter = .05f;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if the player enters the trigger, teleport them to the target
        if (other.gameObject.CompareTag("Player"))
        {
            gameManager.PauseMovement(pauseAfter);
            other.gameObject.transform.position = target.position;
        }
    }
}