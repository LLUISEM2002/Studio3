using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;         // Initial jump force
    public float maxJumpTime = 0.3f;     // Max time jump can be sustained
    public float jumpHoldForce = 5f;     // Extra force while holding jump
    public float jumpHoldTime = 0f;      // Time jump has been held for debugging
    public float rotationSpeed = 10f;    // Rotation speed


    private Rigidbody rb;
    private bool isGrounded;
    private bool isJumping;
    private Transform cameraTransform;   // Reference to the camera's transform

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents unwanted rotations
        cameraTransform = Camera.main.transform;

        // Locks the cursor to the center of the screen and hides it for aesthetic purposes
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Get movement input from InputManager
        Vector2 input = InputManager.Instance.GetMoveInput();

        // Convert input into world space movement relative to the camera
        Vector3 moveDirection = CameraRelativeMovement(input);
        
        // Apply movement while keeping Y velocity unchanged
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        // Rotate player towards movement direction
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Jumping Logic
        if (isGrounded && InputManager.Instance.IsJumpPressed())
        {
            isJumping = true;
            jumpHoldTime = 0f;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }

        // Holding Jump for Higher Jumps
        if (isJumping && InputManager.Instance.IsJumpHeld() && jumpHoldTime < maxJumpTime)
        {
            rb.linearVelocity += Vector3.up * jumpHoldForce * Time.deltaTime;
            jumpHoldTime += Time.deltaTime;
        }

        // Stop jumping if button is released or max time is reached
        if (InputManager.Instance.IsJumpReleased() || jumpHoldTime >= maxJumpTime)
        {
            isJumping = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    /// <summary>
    /// Converts movement input to be relative to the camera's direction.
    /// </summary>
    private Vector3 CameraRelativeMovement(Vector2 input)
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Flatten the forward and right vectors to remove vertical influence
        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();

        // Combine the forward and right vectors with input
        return forward * input.y + right * input.x;
    }
}
