using System.Collections;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
  // === PARAMS

  // Initial speed of agent. Actual speed is set through navMeshAgent
  public float initialSpeed = 5f;

  // For Follow method only
  public float reachDistance = 0.5f;

  // How long this character can be stuck before a follow or move to task is considered to fail
  public float stuckTolerance = 3f;

  // === STATE

  Coroutine currentMovement;

  public float Speed
  {
    get => navAgent.speed;
    set => navAgent.speed = value;
  }

  // === REFS

  FacingDirection facingDirection;
  NavMeshAgent navAgent;


  private void Awake()
  {
    facingDirection = GetComponent<FacingDirection>();
    navAgent = GetComponent<NavMeshAgent>();

    Helper.AssertNotNull(facingDirection, navAgent);
  }

  private void Start()
  {
    navAgent.updateRotation = false;
    navAgent.updateUpAxis = false;
    navAgent.speed = initialSpeed;
  }

  // === INTERFACE

  public void Halt()
  {
    navAgent.ResetPath();

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

  private bool NavigationEnded()
  {
    return navAgent.pathPending == false
            && navAgent.remainingDistance <= navAgent.stoppingDistance
            && (navAgent.hasPath == false || Mathf.Approximately(navAgent.velocity.sqrMagnitude, 0));
  }

  IEnumerator MoveToCoroutine(Vector2 targetPosition, UnityAction<Null> onReach, UnityAction<string> onFail)
  {
    ResetStuckCheck();

    navAgent.SetDestination(targetPosition);

    var delay = new WaitForSeconds(0.1f);

    while (true)
    {
      // Check if stopped
      if (NavigationEnded())
      {
        Halt();

        // Check if the path type is a complete path, not a partial or invalid path
        if (navAgent.pathStatus == NavMeshPathStatus.PathComplete) onReach(null);

        else onFail("Couldn't reach final destination");

        break;
      }

      if (facingDirection.FollowTarget == null && navAgent.velocity.sqrMagnitude > 0.1f)
        facingDirection.FaceDirection(navAgent.velocity);

      yield return delay;

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
    while (true)
    {
      // Never let X axis be 0, otherwise Unity bugs and doesn't add any velocity
      Vector2 targetVelocity = direction * Speed + (direction.x == 0
        ? Vector2.right * Mathf.Sign(direction.y) * 0.01f
        : Vector2.zero);

      navAgent.velocity = targetVelocity;

      if (facingDirection.FollowTarget == null)
        facingDirection.FaceDirection(direction);

      yield return null;
    }
  }

  IEnumerator FollowCoroutine(Transform target, bool stopOnReach, UnityAction<Null> onReach, UnityAction<string> onFail)
  {
    void SetTargetDestination() => navAgent.SetDestination(target.position);

    ResetStuckCheck();

    var delay = new WaitForSeconds(0.1f);

    while (true)
    {
      SetTargetDestination();

      if (
        stopOnReach
        && navAgent.pathStatus == NavMeshPathStatus.PathComplete
        && NavigationEnded())
      {
        Halt();

        onReach(null);
        break;
      }

      if (facingDirection.FollowTarget == null && navAgent.velocity.sqrMagnitude > 0.1f)
        facingDirection.FaceDirection(target.position - transform.position);

      yield return delay;

      // Check if stuck
      if (StuckCheck())
      {
        Halt();

        onFail("Got stuck");
        break;
      }
    }
  }

  // === STUCK CHECK

  Vector2 lastPosition;

  float stuckStartTime = -1;

  void ResetStuckCheck()
  {
    lastPosition = transform.position;
    stuckStartTime = -1;
  }

  bool StuckCheck()
  {
    if (Mathf.Approximately(lastPosition.SqrDistance(transform.position), 0) == false)
      stuckStartTime = -1;

    else
    {
      if (stuckStartTime == -1) stuckStartTime = Time.time;

      else if (Time.time - stuckStartTime >= stuckTolerance) return true;
    }

    lastPosition = transform.position;

    return false;
  }
}
