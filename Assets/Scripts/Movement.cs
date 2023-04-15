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

  Coroutine faceTargetCoroutine;

  // === REFS

  Rigidbody2D body;


  private void Awake()
  {
    body = GetComponent<Rigidbody2D>();

    Helper.AssertNotNull(body);
  }

  // === INTERFACE

  public void Halt()
  {
    body.velocity = Vector2.zero;

    if (currentMovement != null) StopCoroutine(currentMovement);
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

  public void Face(Transform target)
  {
    if (faceTargetCoroutine != null) StopCoroutine(faceTargetCoroutine);
    faceTargetCoroutine = StartCoroutine(FaceTargetCoroutine(target));
  }

  public void FaceMovement()
  {
    StopCoroutine(faceTargetCoroutine);
    faceTargetCoroutine = null;
  }

  IEnumerator FaceTargetCoroutine(Transform target)
  {
    var delay = new WaitForSeconds(0.1f);

    while (target != null)
    {
      UpdateFacingDirection(target.position - transform.position);

      yield return delay;
    }

    FaceMovement();
  }

  void UpdateFacingDirection(Vector2 direction)
  {
    float angle = direction.AsDegrees();

    // Snap to steps of 45 degrees
    FacingDirection = Helper.DegreeToVector2(Mathf.Round(angle / 45) * 45);
  }

  IEnumerator MoveToCoroutine(Vector2 targetPosition, UnityAction onReach = null)
  {
    body.velocity = (targetPosition - (Vector2)transform.position).normalized * speed;

    if (faceTargetCoroutine == null)
      UpdateFacingDirection(targetPosition - (Vector2)transform.position);

    while (transform.position.SqrDistance(targetPosition) > 0.001f)
      yield return new WaitForEndOfFrame();

    Halt();

    if (onReach != null)
      onReach();
  }

  IEnumerator FollowDirectionCoroutine(Vector2 direction)
  {
    body.velocity = direction * speed;

    yield break;
  }

  IEnumerator FollowCoroutine(Transform target, bool stopOnReach)
  {
    while (true)
    {
      if (Vector2.Distance(transform.position, target.position) <= reachDistance)
      {
        if (stopOnReach) break;

        body.velocity = Vector2.zero;
      }

      else
      {
        Vector2 targetDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;

        body.velocity = targetDirection.normalized * speed;

        if (faceTargetCoroutine == null)
          UpdateFacingDirection(targetDirection);
      }

      yield return new WaitForEndOfFrame();
    }

    Halt();
  }
}
