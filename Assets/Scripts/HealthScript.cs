using UnityEngine;

public class HealthScript : MonoBehaviour
{
    [SerializeField] private int health = 10;

    private SpriteFlash spriteFlash;
    private AudioSource audioSource;
    private int startHealth;

    private void Start()
    {
        startHealth = health;
        spriteFlash = GetComponent<SpriteFlash>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        // change the health value and log name/new value to console
        health -= damage;
        spriteFlash.Flash();
        audioSource.Play();
        Debug.Log(gameObject.name + " health value is now: " + health);

        // if health isn't 0 or less, don't do anything else
        if (health > 0) return;

        // object is now dead
        Debug.Log(gameObject.name + " is now dead.");
        gameObject.SetActive(false);

        // reset health for when object is reactivated later
        ResetHealth();
    }

    private void ResetHealth()
    {
        health = startHealth;
        spriteFlash.StopFlash();
    }
}