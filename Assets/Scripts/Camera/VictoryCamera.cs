/**
 * The VictoryCamera looks at the player when they clear a stage.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCamera : MonoBehaviour
{
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();  // Find player

        GetComponent<Camera>().enabled = false;         // Disable so only the main camera renders, this will be enabled by GameManager when the player clears the stage
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform.position);    // Look at the player as they proceed to the next stage
    }
}
