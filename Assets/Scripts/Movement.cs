using System.Collections;
using System.Collections.Generic;
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

  // === STATE

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

  public void Follow(Transform target, bool stopOnReach)
  {
    Halt();

    currentMovement = StartCoroutine(FollowCoroutine(target, stopOnReach));
  }

  IEnumerator MoveToCoroutine(Vector2 targetPosition)
  {
    while (transform.position.SqrDistance(targetPosition) > 0.001f)
    {
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
      transform.Translate(direction * speed * Time.deltaTime);

      yield return new WaitForEndOfFrame();
    }

    // currentMovement = null;
  }

  IEnumerator FollowCoroutine(Transform target, bool stopOnReach)
  {
    float distance = Vector2.Distance(transform.position, target.position);

    while (stopOnReach == false || distance > reachDistance)
    {
      if (distance > reachDistance)
        transform.position = Vector2.MoveTowards(
          transform.position, target.position, Time.deltaTime * speed);

      yield return new WaitForEndOfFrame();
    }

    currentMovement = null;
  }
}
