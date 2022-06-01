using UnityEngine;

public class BobScript : MonoBehaviour
{
    //adjust this to change speed
    [SerializeField] private float speed = 3f;
    //adjust this to change how high it goes
    [SerializeField] private float height = 0.1f;

    private Vector3 startPos;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        startPos = transform.position;
    }

    private void Update()
    {
        if (!gameManager.CanMove()) return;

        //calculate what the new Y position will be
        var newY = Mathf.Sin(Time.time * speed) * height + startPos.y;
        //set the object's Y to the new calculated Y
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}