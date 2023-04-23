using System.Collections;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
  // === PARAMS

  public float speed = 5f;

  // For Follow method only
  public float reachDistance = 0.5f;

  // How long this character can be stuck before a follow or move to task is considered to fail
  public float stuckTolerance = 3f;

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

  public Promise<Null> MoveTo(Vector2 targetPosition)
  {
    return new Promise<Null>((resolve, reject) =>
    {
      Halt();

      currentMovement = StartCoroutine(MoveToCoroutine(targetPosition, resolve, reject));
    });
  }

  public void FollowDirection(Vector2 direction)
  {
    Halt();

    if (direction == Vector2.zero) return;

    currentMovement = StartCoroutine(FollowDirectionCoroutine(direction.normalized));
  }

  public Promise<Null> Follow(Transform target, bool stopOnReach = false)
  {
    return new Promise<Null>((resolve, reject) =>
    {
      Halt();

      currentMovement = StartCoroutine(FollowCoroutine(target, stopOnReach, resolve, reject));
    });
  }

  IEnumerator MoveToCoroutine(Vector2 targetPosition, UnityAction<Null> onReach, UnityAction<string> onFail)
  {
    float sqrDistance;

    stuckTime = 0f;

    while (true)
    {
      sqrDistance = targetPosition.SqrDistance((Vector2)transform.position);

      if (sqrDistance <= Mathf.Pow(speed * Time.deltaTime, 2) || sqrDistance <= 0.005f)
      {
        body.MovePosition(targetPosition);
        Halt();

        onReach(null);

        break;
      }

      body.velocity = (targetPosition - (Vector2)transform.position).normalized * speed;

      if (facingDirection.FollowTarget == null)
        facingDirection.FaceDirection(targetPosition - (Vector2)transform.position);

      yield return new WaitForEndOfFrame();

      // Check if stuck
      if (StuckCheck())
      {
        Halt();

        onFail("Got stuck");
        break;
      }
    }
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

  IEnumerator FollowCoroutine(Transform target, bool stopOnReach, UnityAction<Null> onReach, UnityAction<string> onFail)
  {
    stuckTime = 0f;

    while (true)
    {
      if (Vector2.Distance(transform.position, target.position) <= reachDistance)
      {
        if (stopOnReach)
        {
          Halt();

          onReach(null);
          break;
        }

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

      // Check if stuck
      if (StuckCheck())
      {
        Halt();

        onFail("Got stuck");
        break;
      }
    }
  }

  // If character stays stuckTolerance long without moving, we give up the move task
  float stuckTime = 0f;

  Vector2 lastPosition;

  bool StuckCheck()
  {
    if (lastPosition.SqrDistance(transform.position) >= 0.04)
      stuckTime = 0;

    else
    {
      stuckTime += Time.deltaTime;

      if (stuckTime > stuckTolerance)
        return true;
    }

    lastPosition = transform.position;

    return false;
  }
}
