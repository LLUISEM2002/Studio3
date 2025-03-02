using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float maxJumpTime = 0.3f;
    public float jumpHoldForce = 5f;
    public float jumpHoldTime = 0f;
    public float rotationSpeed = 10f;
    public float doubleJumpForce = 8f;

    // Dash variables
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody rb;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool isJumping;
    private bool jumpReleasedAfterFirstJump;
    private bool isDashing;
    private float dashTimeRemaining;
    private float lastDashTime;

    private Transform cameraTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        cameraTransform = Camera.main.transform;

        // Locks the cursor to the center of the screen and hides it for aesthetic purposes
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Get movement input
        Vector2 input = InputManager.Instance.GetMoveInput();
        Vector3 moveDirection = CameraRelativeMovement(input);

        // Check if dash is activated
        if (InputManager.Instance.IsDashPressed() && Time.time > lastDashTime + dashCooldown && !isDashing)
        {
            StartDash(moveDirection);
        }

        // Handle dashing movement
        if (isDashing)
        {
            DashMovement();
            return; // Prevents normal movement & jumping while dashing
        }

        // Apply normal movement
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        // Rotate player towards movement direction
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Jump logic
        if (isGrounded && InputManager.Instance.IsJumpPressed())
        {
            isJumping = true;
            jumpReleasedAfterFirstJump = false;
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
            jumpReleasedAfterFirstJump = true;
        }

        // Double Jump Logic
        if (!isGrounded && canDoubleJump && jumpReleasedAfterFirstJump && InputManager.Instance.IsJumpPressed())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, doubleJumpForce, rb.linearVelocity.z);
            canDoubleJump = false;
        }
    }

    private void StartDash(Vector3 moveDirection)
    {
        if (moveDirection == Vector3.zero) return; // Prevent dashing with no input

        isDashing = true;
        dashTimeRemaining = dashDuration;
        lastDashTime = Time.time;

        moveDirection.y = 0; // So that the dash is only horizontal

        // Disable gravity and freeze Y velocity to prevent falling
        rb.useGravity = false;   
        rb.linearVelocity = moveDirection.normalized * dashSpeed;

        // Notify GameManager to update the UI cooldown
        GameManager.Instance.StartDashCooldown(dashCooldown);
    }

    private void DashMovement()
    {
        dashTimeRemaining -= Time.deltaTime;

        // Keep velocity strictly horizontal during dash
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (dashTimeRemaining <= 0)
        {
            isDashing = false;
            rb.useGravity = true; // Re-enable gravity after dash ends
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
        canDoubleJump = true;
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

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * input.y + right * input.x;
    }
}
