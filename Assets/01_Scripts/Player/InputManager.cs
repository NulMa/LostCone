
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private PlayerInput playerInput;

    // Public properties for other scripts to read
    public Vector2 MoveInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsInteractionPressed { get; private set; }
    public bool IsDashPressed { get; private set; }
    public bool IsDownJumpPressed { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            playerInput = GetComponent<PlayerInput>(); // PlayerInput 컴포넌트 참조
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // "UI" 액션 맵을 명시적으로 활성화시켜서 ESC 입력을 항상 받을 수 있도록 함
        if(playerInput != null && playerInput.actions != null)
        {
            playerInput.actions.FindActionMap("UI").Enable();
        }
    }

    private void LateUpdate()
    {
        // Reset all single-press flags at the end of the frame
        IsJumpPressed = false;
        IsInteractionPressed = false;
        IsDashPressed = false;
        IsDownJumpPressed = false;
    }

    // These methods are called by PlayerInput's "Send Message" behavior
    // The method name must match the Action name in the Input Actions asset

    private void OnMove(InputValue value)
    {
        Debug.Log("Move Input: " + MoveInput);
        MoveInput = value.Get<Vector2>();
    }

    private void OnJump()
    {
        IsJumpPressed = true;
        Debug.Log("Jump Pressed");
    }

    private void OnInteraction()
    {
        IsInteractionPressed = true;
        Debug.Log("Interaction Pressed");
    }

    private void OnDash()
    {
        IsDashPressed = true;
    }

    private void OnDownJump()
    {
        IsDownJumpPressed = true;
    }

    private void OnESC()
    {
        Debug.Log("ESC pressed, toggling settings panel.");
        if (SettingUI.instance != null)
        {
            SettingUI.instance.ToggleSettingPanel();
        }
    }
}

