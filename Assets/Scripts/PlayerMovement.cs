using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  // === REFS

  Movement movement;
  Health health;

  private void Awake()
  {
    movement = GetComponent<Movement>();
    health = GetComponent<Health>();

    Helper.AssertNotNull(movement, health);
  }

  private void Start()
  {
    health.OnDeath.AddListener(movement.Halt);
  }

  public void Move(InputAction.CallbackContext value)
  {
    if (health.Alive == false) return;

    if (value.phase == InputActionPhase.Performed)
      movement.FollowDirection(value.ReadValue<Vector2>());

    else if (value.phase == InputActionPhase.Canceled)
      movement.Halt();

  }
}
