using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditor
{
  [CustomEditor(typeof(ExtendedRuleTile))]
  [CanEditMultipleObjects]
  public class ExtendedRuleTileEditor : RuleTileEditor
  {
    public Texture2D textureAny;
    public Texture2D textureEmpty;
    public Texture2D textureSpecified;

    public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
    {
      switch (neighbor)
      {
        case ExtendedRuleTile.Neighbor.Any:
          GUI.DrawTexture(rect, textureAny);
          return;
        case ExtendedRuleTile.Neighbor.Empty:
          GUI.DrawTexture(rect, textureEmpty);
          return;
        case ExtendedRuleTile.Neighbor.Specified:
          GUI.DrawTexture(rect, textureSpecified);
          return;
      }

      base.RuleOnGUI(rect, position, neighbor);
    }

  }

}