using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderSorter : MonoBehaviour
{
  // === REFS

  SpriteRenderer spriteRenderer;

  private void Awake()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();

    Helper.AssertNotNull(spriteRenderer);
  }

  private void Update() => spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
}
