using UnityEngine;

namespace ExtensionMethods
{
  public static class VectorExtensions
  {
    public static float SqrDistance(this Vector2 self, Vector2 other)
    {
      return (self - other).SqrMagnitude();
    }

    public static float SqrDistance(this Vector3 self, Vector3 other)
    {
      return (self - other).sqrMagnitude;
    }

  }
}