/**
 * Handles main UI components. This includes displaying: player score, timer, and player speed
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private GameManager gm;
    private PlayerController player;

    public Text scoreText;
    public Text speedText;
    public Text secsText;
    public Text secsDecimalText;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();           // Find GameManager
        player = FindObjectOfType<PlayerController>();  // Find player
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + gm.playerScore;
        speedText.text = player.rigidBody.velocity.magnitude.ToString("F2") + "m/s"; // "F2" makes it so only 2 decimal points are displayed

        // If player's in a NORMAL state
        if (player.currentState == PlayerController.State.NORMAL || (player.currentState == PlayerController.State.DEAD && gm.timer <= 0.0f))
        {
            string timer = gm.timer.ToString("F2");

            string timerSecs = timer.Substring(0, timer.IndexOf(".") == -1 ? 0 : timer.IndexOf("."));           // Get the non decimal portion of the GameManager's timer
            string timerNano = timer.Substring(timer.IndexOf(".") + 1, timer.Length - timer.IndexOf(".") - 1);  // Get the decimal portion of the GameManager's timer

            secsText.text = gm.timer < 0.0f ? "00:" : timerSecs + ":";
            secsDecimalText.text = gm.timer < 0.0f ? "00" : timerNano;
        }
    }
}
