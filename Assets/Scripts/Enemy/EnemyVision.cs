using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
  // === PARAMS

  public float range = 4f;
  [Range(0, 360)] public float angle = 45f;

  public LayerMask targetLayer;
  public LayerMask obstructionLayer;

  public float targetDetectionFrequency = 0.2f;

  // === STATE

  public List<Transform> ActiveTargets { get; private set; } = new List<Transform>();

  // === REFS

  Movement movement;

  private void Awake()
  {
    movement = GetComponent<Movement>();

    Helper.AssertNotNull(movement);
  }

  private void Start()
  {
    StartCoroutine(DetectTargets());
  }

  IEnumerator DetectTargets()
  {
    while (true)
    {
      UpdateTargetsInView();
      yield return new WaitForSeconds(targetDetectionFrequency);
    }
  }

  void UpdateTargetsInView()
  {
    Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, range, targetLayer);

    ActiveTargets.Clear();

    foreach (var target in targetsInRange)
    {
      Vector2 targetDirection = (target.transform.position - transform.position).normalized;

      // Check angle
      // print((targetDirection, movement.FacingDirection));
      if (Vector2.Angle(targetDirection, movement.FacingDirection) > angle / 2) continue;

      float distance = Vector2.Distance(transform.position, target.transform.position);

      // Check obstruction
      if (Physics2D.Raycast(transform.position, targetDirection, distance, obstructionLayer)) continue;

      ActiveTargets.Add(target.transform);
    }
  }

}
