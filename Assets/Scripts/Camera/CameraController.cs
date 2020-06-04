/**
 * Container for the main camera. 
 * - Tilts on the forward and right axis through input on the vertical and horizontal axis.
 * - Apply movement to play by passing tilt factors
 * - Follow player in the direction of their velocity.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform mCamera;

    private PlayerController player;

    private float horizontalTilt;
    private float verticalTilt;

    private float initialXRotation;

    [SerializeField]
    private float spiralTimer;
    [SerializeField]
    private float spiralSpeed;
    [SerializeField]
    private float spiralSpeedMultiplier;

    [SerializeField]
    private float maxVerticalAngle;
    [SerializeField]
    private float maxHorizontalAngle;
    [SerializeField]
    private float tiltSpeed;

    [SerializeField]
    private float offset;

    [SerializeField]
    private bool useFloorNormal;

    // Start is called before the first frame update
    void Start()
    {
        mCamera = transform.GetChild(0);                // Get Camera from child
        player = FindObjectOfType<PlayerController>();  // Find player

        initialXRotation = transform.eulerAngles.x;     // Store initial x rotation
    }

    void FixedUpdate()
    {
        if (player.currentState == PlayerController.State.NORMAL)
        {
            verticalTilt = Input.GetAxis("Vertical");
            horizontalTilt = Input.GetAxis("Horizontal");

            player.Move(verticalTilt, horizontalTilt, transform.right);
        }
    }

    void Update()
    {
        if (player.currentState == PlayerController.State.NORMAL)
        {
            CameraTilt();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch (player.currentState) {
            case PlayerController.State.WAIT:
                SpiralProcedure();
                break;
            case PlayerController.State.NORMAL:
                // Follow the player
                FollowTarget();
                break;
            case PlayerController.State.DEAD:
                // Only look as the player falls
                transform.LookAt(player.transform.position);
                break;
        }
    }

    void SpiralProcedure()
    {
        // Decrement timer
        if (spiralTimer > 0.0f)
            // Speed up timer if the player is holding down the "Jump" button
            if (Input.GetButton("Jump"))
                spiralTimer -= spiralSpeed * Time.deltaTime * spiralSpeedMultiplier;
            else
                spiralTimer -= spiralSpeed * Time.deltaTime;
        // Timer reaches 0
        else
        {
            // Start the stage (Follow the player)
            spiralTimer = 0.0f;
            StartStage();
        }

        // This moves the camera in a spiral patttern that gets smaller the more the spiral decrements
        float x = Mathf.Sin(spiralTimer) * Mathf.Pow(1.0f + spiralTimer, 2.0f);
        float z = -Mathf.Cos(spiralTimer) * Mathf.Pow(1.0f + spiralTimer, 2.0f);

        transform.position = new Vector3(x, spiralTimer, z) + player.transform.position;

        transform.LookAt(player.transform.position);                                                                // Face the player
        transform.eulerAngles = new Vector3(initialXRotation, transform.eulerAngles.y, transform.eulerAngles.z);    // Adjust X angle
        transform.position = transform.position - (transform.forward * offset) + Vector3.forward;                   // Move back by a distance of offset + move it forward a distance of forward (1.0f)
    }

    void StartStage()
    {
        player.currentState = PlayerController.State.NORMAL;
    }

    void CameraTilt()
    {
        // Rotate camera container along the x axis when tilting the joystick up or down to give a forward and back tilt effect.
        // The further up the joystick is the higher the angle for target rotation will be and vice versa.
        float scaledVerticalTilt = initialXRotation - (verticalTilt * maxVerticalAngle);

        // Using floor normal adjust the rotation of the camera's x axis at rest.
        float angleBetweenFloorNormal = useFloorNormal ? Vector3.SignedAngle(Vector3.up, player.floorNormal, transform.right) : 0.0f;

        Quaternion targetXRotation = Quaternion.Euler(scaledVerticalTilt + angleBetweenFloorNormal, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetXRotation, tiltSpeed * Time.deltaTime);

        // Rotate camera along the z axis when tilting the joystick left or right to give a left and right tilt effect.
        // The further right the joystick is the higher the angle for target rotation will be and vice versa.
        float scaledHorizontalTilt = Input.GetAxis("Horizontal") * maxHorizontalAngle;

        Quaternion targetZRotation = Quaternion.Euler(mCamera.rotation.eulerAngles.x, mCamera.rotation.eulerAngles.y, scaledHorizontalTilt);

        mCamera.rotation = Quaternion.RotateTowards(mCamera.rotation, targetZRotation, tiltSpeed * Time.deltaTime);
    }

    void FollowTarget()
    {
        // Get forward vector minus the y component
        Vector3 vectorA = new Vector3(transform.forward.x, 0.0f, transform.forward.z);

        // Get target's velocity vector minus the y component
        Vector3 vectorB = new Vector3(player.rigidBody.velocity.x, 0.0f, player.rigidBody.velocity.z);

        // Find the angle between vectorA and vectorB
        float rotateAngle = Vector3.SignedAngle(vectorA.normalized, vectorB.normalized, Vector3.up);

        // Get the target's speed (maginitude) without the y component
        // Only set speed factor when vector A and B are almost facing the same direction
        float speedFactor = Vector3.Dot(vectorA, vectorB) > 0.0f ? vectorB.magnitude : 1.0f;

        // Rotate towards the angle between vectorA and vectorB
        // Use speedFactor so camera doesn't rotatate at a constant speed
        // Limit speedFactor to be between 1 and 2
        transform.Rotate(Vector3.up, rotateAngle * Mathf.Clamp(speedFactor, 1.0f, 2.0f) * Time.deltaTime);

        // Position the camera behind target at a distance of offset
        transform.position = player.transform.position - (transform.forward * offset);
        transform.LookAt(player.transform.position);
    }
}
