/**
 * Handles collectible objects
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private enum State
    {
        NORMAL,
        MOVE_ABOVE_PLAYER,
        MOVE_TO_SCORE
    }

    private State currentState;

    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float rotationSpeedMultiplier;
    [SerializeField]
    private float moveSpeed;

    private AudioSource audioSource;

    private Transform player;
    private Transform scorePoint;

    private Vector3 initPos;
    private Vector3 initScale;

    private float timer;

    public bool pickedUp;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.NORMAL;                // Set current state NORMAL
        audioSource = GetComponent<AudioSource>();  // Get AudioSource component
        initScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate collectible
        // Speed up rotation if picked up by multiplying by rotationSpeedMultiplier
        transform.Rotate(Vector3.up, rotationSpeed * (pickedUp ? rotationSpeedMultiplier : 1.0f) * Time.deltaTime);

        if (pickedUp)
        {
            switch (currentState)
            {
                case State.MOVE_TO_SCORE:
                    if (timer < 1.0f)
                    {
                        transform.position = Vector3.Lerp(initPos, scorePoint.position, timer);                 // Move to score point
                        transform.localScale = Vector3.Lerp(initScale * 0.5f, initScale * 0.25f, timer);    // Shrink
                    }
                    else
                    {
                        // Increment player score
                        FindObjectOfType<GameManager>().playerScore++;
                        // Destroy object
                        Destroy(gameObject);
                    }
                    break;
                case State.MOVE_ABOVE_PLAYER:
                    if (timer < 1.0f)
                    {
                        transform.position = Vector3.Lerp(initPos, player.position + Vector3.up, timer);        // Move above player
                        transform.localScale = Vector3.Lerp(initScale, initScale * 0.5f, timer);            // Shrink
                    }
                    else
                    {
                        // Change state to move to score
                        currentState = State.MOVE_TO_SCORE;
                        // Get score point
                        scorePoint = GameObject.FindGameObjectWithTag("ScorePoint").transform;
                        // Set initPos to current position
                        initPos = transform.position;
                        // Reset timer
                        timer = 0.0f;
                    }
                    break;
            }

            timer += moveSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Changes state of collectible and sets player to passed transform.
    /// </summary>
    /// <param name="player">Player transform</param>
    public void PickedUp(Transform player)
    {
        pickedUp = true;                                // Collectible picked up
        currentState = State.MOVE_ABOVE_PLAYER;         // Change collectible state to move above player

        this.player = player;                           // Set player to passed transform

        initPos = transform.position;                   // Set initPos to current position

        if (audioSource != null)
            audioSource.PlayOneShot(audioSource.clip);  // Play pick up audio clip
    }
}
