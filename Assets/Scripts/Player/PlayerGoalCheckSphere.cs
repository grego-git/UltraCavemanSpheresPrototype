/**
 * Checks if the player has passed through the goal
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  PlayerGoalCheckSphere : MonoBehaviour
{
    private PlayerController player;

    private void Start()
    {
        player = transform.parent.GetComponent<PlayerController>(); // Get PlayerController component from parent
    }

    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
        // Trigger collides with object tagged as ExitPortal and their current state is NORMAL
        if (other.tag == "ExitPortal" && player.currentState == PlayerController.State.NORMAL)
        {
            player.currentState = PlayerController.State.VICTORY;   // Set player's current state to VICTORY state
        }
    }
}
