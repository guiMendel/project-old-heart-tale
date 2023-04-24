using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chase : CharacterState
{
  // === PARAMS

  public float chaseSpeedModifier = 1.5f;

  public float noSightChaseTolerance = 1f;

  // === STATE

  Transform chaseTarget;

  Coroutine loseTargetTimer;

  // === REFS

  Movement movement;
  EnemyVision enemyVision;

  protected override void Awake()
  {
    base.Awake();

    movement = GetComponent<Movement>();
    enemyVision = GetComponent<EnemyVision>();

    Helper.AssertNotNull(movement, enemyVision);

    enemyVision.OnDetectTarget.AddListener(Spot);

    enemyVision.OnLoseTarget.AddListener((target) =>
    {
      if (IsActive && chaseTarget == target) loseTargetTimer = StartCoroutine(MaybeLoseTarget());
    });
  }

  protected override IEnumerator OnDeactivate()
  {
    chaseTarget = null;
    movement.Speed = movement.initialSpeed;

    yield break;
  }

  void Spot(Transform target)
  {
    if (manager.ActiveState is Patrol)
    {
      Activate(target);
      return;
    }

    if (IsActive)
    {
      StopCoroutine(loseTargetTimer);

      chaseTarget = target;

      FollowTarget();
    }
  }

  private void FollowTarget() => movement.Follow(chaseTarget).OnFail(_ => LoseTarget());

  public void Activate(Transform target)
  {
    chaseTarget = target;

    manager.ActiveState = this;
  }

  protected override IEnumerator OnActivate()
  {
    FollowTarget();

    movement.Speed = movement.initialSpeed * chaseSpeedModifier;

    yield break;
  }

  IEnumerator MaybeLoseTarget()
  {
    yield return new WaitForSeconds(noSightChaseTolerance);

    if (IsActive == false) yield break;

    LoseTarget();
  }

  private void LoseTarget()
  {
    Null PatrolIfLost()
    {
      if (IsActive && chaseTarget == null) GetComponent<Patrol>().Activate();

      return null;
    }

    Vector2 lastSeenPosition = chaseTarget.position;

    chaseTarget = null;

    // Go to last seen position
    movement.MoveTo(lastSeenPosition)
      .Then(_ => PatrolIfLost())
      .OnFail(_ => PatrolIfLost());
  }
}
