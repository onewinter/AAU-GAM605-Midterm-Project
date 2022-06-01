using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject healthText;
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private float typeDelay = 0.1f;
    
    private GameObject itemKey;
    private GameObject itemLock;
    private TextMeshProUGUI dialogBoxText;
    private EnemyScript[] enemies;
    private string displayText = "";
    private bool allowMovement;
    private float timeSincePaused;
    private float timeToPause;
    private bool dialogOpen;
    private bool typingFinished;
    private float lastCharacterTime;
    private int nextCharacter = 1;

    private void Start()
    {
        // init vars
        timeSincePaused = 0;
        allowMovement = false;
        dialogOpen = true;
        dialogBoxText = dialogBox.GetComponentInChildren<TextMeshProUGUI>();
        dialogBoxText.text = "";

        // store enemies, key and lock game objects for later
        itemKey = GameObject.FindWithTag("Key");
        itemLock = GameObject.FindWithTag("Lock");
        enemies = FindObjectsOfType<EnemyScript>();

        // show opening dialog
        ShowDialog(
            "Someone has locked your princess in the tower!\n.\n.\nJust kidding. You are, however, late for a sunset date with your love.  Avoid or defeat the enemies blocking your way and join her on the rooftop!\n\nShoot with spacebar.\nPress Enter to continue.");
    }

    private void Update()
    {
        // when typing is finished, enter key closes the dialog (or esc before)
        if ((Input.GetKeyDown("return") && typingFinished) || Input.GetKeyDown("escape"))
        {
            dialogBox.SetActive(false);
            dialogOpen = false;
            allowMovement = true;
            dialogBoxText.text = "";
        }

        // print out the message one character at a time
        if (nextCharacter <= displayText.Length)
        {
            typingFinished = false;
            if (typeDelay <= lastCharacterTime)
            {
                var text = displayText.Substring(0, nextCharacter);
                dialogBoxText.text = text;
                nextCharacter++;
                lastCharacterTime = 0;
            }
            else
            {
                lastCharacterTime += Time.deltaTime;
            }
        }
        else
        {
            typingFinished = true;
        }

        // don't bother to check on unpausing if the dialog box is open
        if (dialogOpen) return;

        // unpause the game
        if (timeSincePaused >= timeToPause)
            allowMovement = true;

        timeSincePaused += Time.deltaTime;
    }

    public void PauseMovement(float pause)
    {
        allowMovement = false;
        timeToPause = pause;
        timeSincePaused = 0;
    }

    public bool CanMove()
    {
        return allowMovement;
    }

    // pop up the dialog box and pause the game
    private void ShowDialog(string text)
    {
        nextCharacter = 1;
        displayText = text;
        allowMovement = false;
        dialogOpen = true;
        dialogBox.SetActive(true);
    }

    public void UpdateHealth(int health)
    {
        healthText.GetComponent<TextMeshProUGUI>().text = health.ToString();
    }

    // reset the game after a win or loss
    public void ResetGame(bool didWeWin)
    {
        // reset our movers and ensure they're active
        foreach (var enemy in enemies) enemy.ResetPosition();

        // reset our items for pickup
        itemKey.SetActive(true);
        itemLock.SetActive(true);

        // display win/loss message
        if (didWeWin)
        {
            ShowDialog(
                "You made it to the rooftop on time! You settle into the bench with your love and enjoy the sunset together.\n\nPress Enter to play again.");
            Debug.Log("Goal reached.");
        }
        else
        {
            ShowDialog("You have died!\n\nPress Enter to try again.");
            Debug.Log("Resetting the game.");
        }
    }
}