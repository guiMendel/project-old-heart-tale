using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetect : MonoBehaviour
{
  public LayerMask detectionLayer;

  // Update is called once per frame
  private void FixedUpdate()
  {
    Collider2D detector = Physics2D.OverlapPoint(transform.position, detectionLayer);

    if (detector != null)
      transform.position = new Vector3(transform.position.x, transform.position.y, detector.transform.position.z);
  }
}
