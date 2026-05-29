using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool attack;
    public bool interact;
    public bool inventory;
    public bool quickSlot1;
    public bool quickSlot2;
    public bool quickSlot3;
    public bool quickSlot4;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
    }

    public void OnInteract(InputValue value)
    {
        interact = value.isPressed;
    }

    public void OnInventory(InputValue value)
    {
        inventory = value.isPressed;
    }

    public void OnQuickSlot1(InputValue value) { quickSlot1 = value.isPressed; }
    public void OnQuickSlot2(InputValue value) { quickSlot2 = value.isPressed; }
    public void OnQuickSlot3(InputValue value) { quickSlot3 = value.isPressed; }
    public void OnQuickSlot4(InputValue value) { quickSlot4 = value.isPressed; }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnAttack(InputValue value)
    {
        AttackInput(value.isPressed);
    }
#endif


    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void AttackInput(bool newAttackState)
    {
        attack = newAttackState;
    }

    public void InteractInput(bool newInteractState)
    {
        interact = newInteractState;
    }

    public void InventoryInput(bool newInventoryState)
    {
        inventory = newInventoryState;
    }
}