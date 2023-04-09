using System;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
  // === PARAMS

  public Transform waypoints;

  public float chaseSpeedModifier = 1.5f;

  public float noSightChaseTolerance = 1f;

  // === STATE

  int currentWaypoint = 0;

  Transform chaseTarget;

  Coroutine loseTargetTimer;

  float initialSpeed;

  // === REFS

  Movement movement;
  EnemyVision enemyVision;

  private void Awake()
  {
    movement = GetComponent<Movement>();
    enemyVision = GetComponent<EnemyVision>();

    Helper.AssertNotNull(movement, enemyVision);

    initialSpeed = movement.speed;

    enemyVision.OnDetectTarget.AddListener(Chase);
    enemyVision.OnLoseTarget.AddListener(
      (target) => loseTargetTimer = StartCoroutine(MaybeLoseTarget(target)));
  }

  private void Start()
  {
    Debug.Assert(waypoints.childCount > 0, "Must assign waypoints to the enemy movement script");

    // movement.Follow(FindObjectOfType<PlayerMovement>().transform);

    AdvanceTarget();
  }

  void Patrol()
  {
    movement.MoveTo(waypoints.GetChild(currentWaypoint).position, AdvanceTarget);
  }

  void Chase(Transform target)
  {
    print(("Chase", target.gameObject));

    if (chaseTarget != null)
    {
      if (target == chaseTarget && loseTargetTimer != null)
      {
        StopCoroutine(loseTargetTimer);
        loseTargetTimer = null;
      }

      return;
    }

    chaseTarget = target;

    movement.Follow(target);
    movement.speed = initialSpeed * chaseSpeedModifier;
  }

  IEnumerator MaybeLoseTarget(Transform target)
  {
    if (chaseTarget != target) yield break;

    yield return new WaitForSeconds(noSightChaseTolerance);

    print(("Lose", target.gameObject));

    chaseTarget = null;

    movement.MoveTo(target.position, () =>
    {
      movement.speed = initialSpeed;
      Patrol();
    });
  }

  void AdvanceTarget()
  {
    currentWaypoint = (currentWaypoint + 1) % waypoints.childCount;
    Patrol();
  }
}
