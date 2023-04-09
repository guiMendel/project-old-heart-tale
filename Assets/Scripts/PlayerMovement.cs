using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  // === PARAMS

  public float speed = 5f;
  public float sprintSpeed = 6f;

  // === STATE

  Vector2 direction;

  private void Update()
  {
    transform.Translate(direction * speed * Time.deltaTime);
  }

  public void Move(InputAction.CallbackContext value)
  {
    if (value.phase == InputActionPhase.Performed)
      direction = value.ReadValue<Vector2>();

    else if (value.phase == InputActionPhase.Canceled)
      direction = Vector2.zero;

  }
}
