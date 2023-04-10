using UnityEngine;
using UnityEngine.Assertions;

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

    public static float AsDegrees(this Vector2 self)
    {
      return Vector2.SignedAngle(Vector2.right, self);
    }

    public static float AsDegrees(this Vector3 self)
    {
      return Vector3.SignedAngle(Vector2.right, self, Vector3.back);
    }

  }
}