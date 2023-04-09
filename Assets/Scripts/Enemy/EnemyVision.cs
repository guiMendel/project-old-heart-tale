using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
  // === PARAMS

  public float range = 4f;
  [Range(0, 360)] public float angle = 45f;
}
