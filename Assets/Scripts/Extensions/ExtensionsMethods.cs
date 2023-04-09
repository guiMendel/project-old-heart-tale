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

    public static Vector2 RadianToVector2(float radian)
    {
      return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
      return RadianToVector2(degree * Mathf.Deg2Rad);
    }
  }
}