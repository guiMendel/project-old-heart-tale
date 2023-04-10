using System.Collections;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RangedAttack))]
public class RangedAttackEditor : Editor
{
  private void OnSceneGUI()
  {
    RangedAttack rangedAttack = (RangedAttack)target;

    Handles.color = Color.red;

    Handles.DrawWireArc(rangedAttack.transform.position, Vector3.back, Vector2.right, 360, rangedAttack.range.min);
    Handles.DrawWireArc(rangedAttack.transform.position, Vector3.back, Vector2.right, 360, rangedAttack.range.max);
  }
}
