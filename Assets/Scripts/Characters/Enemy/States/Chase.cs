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

  float initialSpeed;

  // === REFS

  Movement movement;
  EnemyVision enemyVision;

  protected override void Awake()
  {
    base.Awake();

    movement = GetComponent<Movement>();
    enemyVision = GetComponent<EnemyVision>();

    Helper.AssertNotNull(movement, enemyVision);

    initialSpeed = movement.initialSpeed;

    enemyVision.OnDetectTarget.AddListener(target =>
    {
      if (IsActive || manager.ActiveState is Patrol) Activate(target);
    });

    enemyVision.OnLoseTarget.AddListener((target) =>
    {
      if (IsActive && chaseTarget == target) loseTargetTimer = StartCoroutine(MaybeLoseTarget());
    });
  }

  protected override IEnumerator OnDeactivate()
  {
    chaseTarget = null;
    movement.initialSpeed = initialSpeed;

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

      movement.Follow(chaseTarget);
    }
  }

  public void Activate(Transform target)
  {
    if (target == chaseTarget)
      StopCoroutine(loseTargetTimer);

    chaseTarget = target;

    manager.ActiveState = this;
  }


  protected override IEnumerator OnActivate()
  {
    movement.Follow(chaseTarget).OnFail(_ => LoseTarget());

    movement.initialSpeed = initialSpeed * chaseSpeedModifier;

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
    Null StartPatrol()
    {
      if (IsActive) GetComponent<Patrol>().Activate();

      return null;
    }

    movement.MoveTo(chaseTarget.position)
      .Then(_ => StartPatrol())
      .OnFail(_ => StartPatrol());
  }
}
