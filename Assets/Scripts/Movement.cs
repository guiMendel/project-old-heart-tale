using System.Collections;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
  // === PARAMS

  public float speed = 5f;

  // For Follow method only
  public float reachDistance = 0.5f;

  // === EVENTS

  // Called when MoveTo reaches a position
  public UnityEvent OnReachPosition;

  // Called when facing direction changes
  public Event.DoubleVector2 OnChangeFacingDirection;

  // === STATE

  Vector2 _facingDirection = Vector2.down;
  public Vector2 FacingDirection
  {
    get { return _facingDirection; }
    
    private set
    {
      OnChangeFacingDirection.Invoke(value, _facingDirection);
      _facingDirection = value;
    }
  }

  Coroutine currentMovement;

  // === INTERFACE

  public void Halt()
  {
    if (currentMovement == null) return;

    StopCoroutine(currentMovement);
    currentMovement = null;
  }

  public void MoveTo(Vector2 targetPosition)
  {
    Halt();

    currentMovement = StartCoroutine(MoveToCoroutine(targetPosition));
  }

  public void FollowDirection(Vector2 direction)
  {
    Halt();

    if (direction == Vector2.zero) return;

    currentMovement = StartCoroutine(FollowDirectionCoroutine(direction.normalized));
  }

  public void Follow(Transform target, bool stopOnReach = false)
  {
    Halt();

    currentMovement = StartCoroutine(FollowCoroutine(target, stopOnReach));
  }

  void UpdateFacingDirection(Vector2 movementDirection)
  {
    float angle = movementDirection.AsDegrees();

    // Snap to steps of 45 degrees
    FacingDirection = Helper.DegreeToVector2(Mathf.Round(angle / 45) * 45);
  }

  IEnumerator MoveToCoroutine(Vector2 targetPosition)
  {
    while (transform.position.SqrDistance(targetPosition) > 0.001f)
    {
      UpdateFacingDirection(targetPosition - (Vector2)transform.position);

      transform.position = Vector2.MoveTowards(
        transform.position, targetPosition, Time.deltaTime * speed);

      yield return new WaitForEndOfFrame();
    }

    OnReachPosition.Invoke();

    currentMovement = null;
  }

  IEnumerator FollowDirectionCoroutine(Vector2 direction)
  {
    while (true)
    {
      UpdateFacingDirection(direction);

      transform.Translate(direction * speed * Time.deltaTime);

      yield return new WaitForEndOfFrame();
    }

    // currentMovement = null;
  }

  IEnumerator FollowCoroutine(Transform target, bool stopOnReach)
  {
    while (true)
    {
      float distance = Vector2.Distance(transform.position, target.position);

      if (stopOnReach && distance <= reachDistance) break;

      if (distance > reachDistance)
      {
        UpdateFacingDirection(target.position - transform.position);

        transform.position = Vector2.MoveTowards(
          transform.position, target.position, Time.deltaTime * speed);
      }

      yield return new WaitForEndOfFrame();
    }

    currentMovement = null;
  }
}
