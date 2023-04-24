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

  // === EVENTS

  public Event.ETransform OnDetectTarget;

  public Event.ETransform OnLoseTarget;

  // === STATE

  public List<Transform> ActiveTargets { get; private set; } = new List<Transform>();

  // === REFS

  FacingDirection facingDirection;
  GroundLevel groundLevel;

  private void Awake()
  {
    facingDirection = GetComponent<FacingDirection>();
    groundLevel = GetComponent<GroundLevel>();

    Helper.AssertNotNull(facingDirection, groundLevel);
  }

  private void Start()
  {
    StartCoroutine(DetectTargets());
  }

  public bool InVision(Transform target) => ActiveTargets.Contains(target);

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

    var previousTargets = new List<Transform>(ActiveTargets);
    ActiveTargets.Clear();

    print(targetsInRange);

    foreach (var target in targetsInRange)
    {
      // Check if can be target
      if (target.CompareTag("Trespassing") == false) continue;

      // Can't see above
      GroundLevel targetLevel = target.GetComponent<GroundLevel>();
      if (targetLevel == null || targetLevel.Level > groundLevel.Level) continue;

      Vector2 targetDirection = (target.transform.position - transform.position).normalized;

      // Check angle
      if (Vector2.Angle(targetDirection, facingDirection.Direction) > angle / 2) continue;

      float distance = Vector2.Distance(transform.position, target.transform.position);

      // Check obstruction
      List<RaycastHit2D> hits = new List<RaycastHit2D>();

      // Get filter
      ContactFilter2D filter = new ContactFilter2D();
      filter.SetLayerMask(obstructionLayer);

      if (Physics2D.Raycast(transform.position, targetDirection, filter, hits, distance) > 0)
      {
        bool obstructed = false;

        // Can only be obstructed by objects in same level
        foreach (var hit in hits)
        {
          GroundLevel hitLevel = hit.collider.GetComponent<GroundLevel>();

          print((groundLevel.Level, hitLevel.Level));
          
          if (hitLevel != null && hitLevel.Level == groundLevel.Level)
          {
            obstructed = true;
            break;
          }
        }

        if (obstructed) continue;
      }

      ActiveTargets.Add(target.transform);
    }

    // Detect new targets
    foreach (var activeTarget in ActiveTargets) if (previousTargets.Contains(activeTarget) == false)
        OnDetectTarget.Invoke(activeTarget);

    // Lose old targets
    foreach (var previousTarget in previousTargets) if (ActiveTargets.Contains(previousTarget) == false)
        OnLoseTarget.Invoke(previousTarget);
  }

}
