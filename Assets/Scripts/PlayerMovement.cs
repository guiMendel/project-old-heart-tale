using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  // === REFS

  Movement movement;

  private void Awake()
  {
    movement = GetComponent<Movement>();

    Helper.AssertNotNull(movement);
  }

  public void Move(InputAction.CallbackContext value)
  {
    if (value.phase == InputActionPhase.Performed)
      movement.FollowDirection(value.ReadValue<Vector2>());

    else if (value.phase == InputActionPhase.Canceled)
      movement.Halt();

  }
}
