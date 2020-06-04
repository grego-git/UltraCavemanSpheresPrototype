/**
 * This handles the behavior of the Congrats screen. Right now just loops back to title screen
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CongratsScreen : MonoBehaviour
{
    private bool pressedStart;

    // Update is called once per frame
    void Update()
    {
        // Player presses start for the first time
        if (!pressedStart && Input.GetButtonDown("Start"))
        {
            pressedStart = true;
            SceneManager.LoadScene("SampleScene");  // Load title screen
        }
    }
}
