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

  // === STATE

  Coroutine currentMovement;

  // === REFS

  Rigidbody2D body;
  FacingDirection facingDirection;


  private void Awake()
  {
    body = GetComponent<Rigidbody2D>();
    facingDirection = GetComponent<FacingDirection>();

    Helper.AssertNotNull(body, facingDirection);
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

  IEnumerator MoveToCoroutine(Vector2 targetPosition, UnityAction onReach = null)
  {
    float sqrDistance;

    while (true)
    {
      sqrDistance = targetPosition.SqrDistance((Vector2)transform.position);

      if (sqrDistance <= Mathf.Pow(speed * Time.deltaTime, 2) || sqrDistance <= 0.005f)
      {
        body.MovePosition(targetPosition);
        break;
      }

      body.velocity = (targetPosition - (Vector2)transform.position).normalized * speed;

      if (facingDirection.FollowTarget == null)
        facingDirection.FaceDirection(targetPosition - (Vector2)transform.position);

      yield return new WaitForEndOfFrame();
    }

    Halt();

    if (onReach != null)
      onReach();
  }

  IEnumerator FollowDirectionCoroutine(Vector2 direction)
  {
    var delay = new WaitForSeconds(0.1f);

    while (true)
    {
      body.velocity = direction * speed;

      if (facingDirection.FollowTarget == null)
        facingDirection.FaceDirection(direction);

      yield return delay;
    }
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

        if (facingDirection.FollowTarget == null)
          facingDirection.FaceDirection(targetDirection);
      }

      yield return new WaitForEndOfFrame();
    }

    Halt();
  }
}
