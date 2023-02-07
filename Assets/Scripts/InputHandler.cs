using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private PlayerInputControl _PlayerInputControl;

    public Vector2 InputMoveVector { get { return _InputMoveVector; } }
    private Vector2 _InputMoveVector;

    private void OnEnable()
    {
        if (_PlayerInputControl == null)
        {
            _PlayerInputControl = new PlayerInputControl();
        }
        
        _PlayerInputControl.Player.Enable();
        _PlayerInputControl.Player.Move.performed += SetInputMoveVector;
        _PlayerInputControl.Player.Move.canceled += SetInputMoveVector;
#if DEBUG
        _PlayerInputControl.Player.Debug.performed += ShowDebugMenu;
#endif
    }

    private void OnDisable()
    {
        _PlayerInputControl.Player.Disable();
        _PlayerInputControl.Player.Move.performed -= SetInputMoveVector;
        _PlayerInputControl.Player.Move.canceled -= SetInputMoveVector;
#if DEBUG
        _PlayerInputControl.Player.Debug.performed -= ShowDebugMenu;
#endif
    }

    private void SetInputMoveVector(InputAction.CallbackContext context)
    {
        _InputMoveVector = context.ReadValue<Vector2>();
    }

    private void ShowDebugMenu(InputAction.CallbackContext context)
    {
        UIManager.Show<DebugMenu>();
    }
}
