/**
 * This component handles player movment
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        WAIT,
        NORMAL,
        DEAD,
        VICTORY,
        STATES
    }

    [HideInInspector]
    public State currentState;

    [HideInInspector]
    public Rigidbody rigidBody;

    [HideInInspector]
    public Vector3 floorNormal;

    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private float groundCheckRadius;

    public float maxSpeed;

    [SerializeField]
    private float moveForce;
    [SerializeField]
    private float victoryForce;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();  // Get rigidbody component

        currentState = State.WAIT;            // Set current state to WAIT
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.VICTORY:
                ApplyVictoryForce();
                break;
        }
    }

    /// <summary>
    /// Applies force to player object
    /// </summary>
    /// <param name="verticalTilt">Scales force applied in the forward direction (Ranges between 1 and -1)</param>
    /// <param name="horizontalTilt">Scales force applied in the horizontal direction (Ranges between 1 and -1)</param>
    /// <param name="right">The horizontal direction</param>
    public void Move(float verticalTilt, float horizontalTilt, Vector3 right)
    {
        // Only apply movement when the player is grounded
        if (OnGround())
        {
            CalculateFloorNormal();

            // No input from player
            if (horizontalTilt == 0.0f && verticalTilt == 0.0f && rigidBody.velocity.magnitude > 0.0f)
            {
                rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, moveForce * 0.1f * Time.deltaTime); // Slow down
            }
            else
            {
                // Get a direction perpendicular to the camera's right vector and the floor's normal (The forward direction)
                Vector3 forward = Vector3.Cross(right, floorNormal);

                // Apply moveForce scaled by verticalTilt in the forward direction (Half the move force when moving backwards)
                Vector3 forwardForce = (verticalTilt > 0.0f ? 1.0f : 0.5f) * moveForce * verticalTilt * forward;
                // Apply moveForce scaled by horizontalTilt in the right direction
                Vector3 rightForce = moveForce * horizontalTilt * right;

                Vector3 forceVector = forwardForce + rightForce;

                rigidBody.AddForce(forceVector);
            }
        }
    }

    /// <summary>
    /// Checks if the player is grounded (Above an object within whatIsGround layer mask)
    /// </summary>
    /// <returns>
    /// True if CheckSphere overlaps with collider within the whatIsGround layer mask, else return False
    /// </returns>
    public bool OnGround()
    {
        return Physics.CheckSphere(transform.position - (Vector3.up * 0.5f), groundCheckRadius, whatIsGround);
    }

    /// <summary>
    /// Applies an upward force to the player to lift them up to the next stage
    /// </summary>
    private void ApplyVictoryForce()
    {
        Vector3 flatVel = new Vector3(rigidBody.velocity.x, 0.0f, rigidBody.velocity.z);

        // Move in the opposite direction of velocity minus the y component
        if (flatVel.magnitude > 0.1f)
            rigidBody.AddForce(-flatVel);


        // Increment force in the up direction
        rigidBody.AddForce(victoryForce++ * Vector3.up);
    }

    /// <summary>
    /// Sets floor normal by casting ray below player
    /// </summary>
    private void CalculateFloorNormal()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, whatIsGround))
        {
            floorNormal = hit.normal;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (currentState == State.NORMAL)
        {
            // Player collides with collectible
            if (other.tag == "Collectible")
            {
                Collectible collectible = other.GetComponent<Collectible>();    // Get collectible

                // If collectible has not been picked up
                if (collectible != null && !collectible.pickedUp)
                    collectible.PickedUp(transform);
            }
        }
    }
}
