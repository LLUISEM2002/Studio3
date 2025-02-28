using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;         // Initial jump force
    public float maxJumpTime = 0.3f;     // Max time jump can be sustained
    public float jumpHoldForce = 5f;     // Extra force while holding jump
    public float jumpHoldTime = 0f;      // Time jump has been held

    private Rigidbody rb;
    private bool isGrounded;
    private bool isJumping;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents unwanted rotations
    }

    private void Update()
    {
        // Get movement input from InputManager
        Vector2 input = InputManager.Instance.GetMoveInput();
        
        // Move the player
        Vector3 move = new Vector3(input.x, 0, input.y) * moveSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

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
}
