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

  // Position to assume in the next fixed update
  Vector2 nextPosition;

  // === REFS

  Rigidbody2D body;


  private void Awake()
  {
    nextPosition = transform.position;

    body = GetComponent<Rigidbody2D>();
  }

  private void FixedUpdate()
  {
    if (body != null) body.MovePosition(nextPosition);
    else transform.position = nextPosition;
  }

  // === INTERFACE

  public void Halt()
  {
    if (currentMovement == null) return;

    StopCoroutine(currentMovement);
    currentMovement = null;
  }

  public void MoveTo(Vector2 targetPosition, UnityAction onReach = null)
  {
    Halt();

    currentMovement = StartCoroutine(MoveToCoroutine(targetPosition, onReach));
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

  void Translate(Vector2 displacement)
  {
    nextPosition = nextPosition + displacement;
  }

  void SetPosition(Vector2 position)
  {
    nextPosition = position;
  }

  IEnumerator MoveToCoroutine(Vector2 targetPosition, UnityAction onReach = null)
  {
    while (transform.position.SqrDistance(targetPosition) > 0.001f)
    {
      UpdateFacingDirection(targetPosition - (Vector2)transform.position);

      SetPosition(Vector2.MoveTowards(
        nextPosition, targetPosition, Time.deltaTime * speed));

      yield return new WaitForEndOfFrame();
    }

    currentMovement = null;

    if (onReach != null)
      onReach();
  }

  IEnumerator FollowDirectionCoroutine(Vector2 direction)
  {
    while (true)
    {
      UpdateFacingDirection(direction);

      Translate(direction * speed * Time.deltaTime);

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

        SetPosition(Vector2.MoveTowards(
          nextPosition, target.position, Time.deltaTime * speed));
      }

      yield return new WaitForEndOfFrame();
    }

    currentMovement = null;
  }
}
