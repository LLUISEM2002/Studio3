using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private Vector2 moveInput;
    private bool jumpPressed;
    private bool jumpHeld;
    private bool jumpReleased;
    private bool dashPressed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure it persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Get movement input
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Jump input detection
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        jumpReleased = Input.GetButtonUp("Jump");

        // Dash input detection
        dashPressed = Input.GetKeyDown(KeyCode.LeftShift);
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public bool IsJumpPressed()
    {
        return jumpPressed;
    }

    public bool IsJumpHeld()
    {
        return jumpHeld;
    }

    public bool IsJumpReleased()
    {
        return jumpReleased;
    }

    public bool IsDashPressed()
    {
        return dashPressed;
    }
}
