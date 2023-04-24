using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLevel : MonoBehaviour
{
  // === PARAMS

  public int initialLevel = 0;

  // Layer of objects that warp other objects to their level
  public LayerMask detectionLayer;

  // === STATE

  public int Level { get; private set; } = 0;

  private void Awake()
  {
    Level = initialLevel;
  }


  private void FixedUpdate()
  {
    Collider2D detector = Physics2D.OverlapPoint(transform.position, detectionLayer);

    if (detector != null)
      Level = (int)detector.transform.position.z;
  }
}
