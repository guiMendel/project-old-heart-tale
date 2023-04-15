using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hazard : MonoBehaviour
{
  private void OnTriggerEnter2D(Collider2D other)
  {
    Health otherHealth = other.GetComponent<Health>();

    if (otherHealth == null) return;

    otherHealth.TakeDamage();
  }

  private void OnCollisionEnter2D(Collision2D other)
  {
    Health otherHealth = other.collider.GetComponent<Health>();

    if (otherHealth == null) return;

    otherHealth.TakeDamage();
  }
}
