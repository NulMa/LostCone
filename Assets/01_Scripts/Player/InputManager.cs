
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
        // Control Scheme과 Device를 Keyboard&Mouse로 강제 설정
        if (playerInput != null)
        {
            playerInput.defaultControlScheme = "Keyboard&Mouse";
            // 키보드와 마우스가 연결되어 있다면 해당 장치들로 스킴 전환
            if (Keyboard.current != null && Mouse.current != null)
            {
                playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            }
        }

        // "UI" 액션 맵을 명시적으로 활성화시켜서 ESC 입력을 항상 받을 수 있도록 함
        if (playerInput != null && playerInput.actions != null)
        {
            // Player 맵과 UI 맵을 모두 활성화하여 동시에 입력을 받을 수 있게 함
            var pMap = playerInput.actions.FindActionMap("Player");
            var uMap = playerInput.actions.FindActionMap("UI");
            if(pMap != null) pMap.Enable();
            if(uMap != null) uMap.Enable();
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
        MoveInput = value.Get<Vector2>();
    }

    private void OnJump()
    {
        IsJumpPressed = true;
    }

    private void OnInteraction()
    {
        IsInteractionPressed = true;
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

