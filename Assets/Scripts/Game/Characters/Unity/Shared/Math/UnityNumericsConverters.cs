using NQuaternion = System.Numerics.Quaternion;
using NVector2 = System.Numerics.Vector2;
using NVector3 = System.Numerics.Vector3;
using UQuaternion = UnityEngine.Quaternion;
using UVector2 = UnityEngine.Vector2;
using UVector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Game.Characters.Unity.Shared.Math
{
	public static class UnityNumericsConverters
	{
		public static NVector2 ToNumerics(this UVector2 v) => new(v.x, v.y);

		public static NVector3 ToNumerics(this UVector3 v) => new(v.x, v.y, v.z);

		public static NQuaternion ToNumerics(this UQuaternion q) => new(q.x, q.y, q.z, q.w);

		public static UVector2 ToUnity(this NVector2 v) => new(v.X, v.Y);

		public static UVector3 ToUnity(this NVector3 v) => new(v.X, v.Y, v.Z);

		public static UQuaternion ToUnity(this NQuaternion q) => new(q.X, q.Y, q.Z, q.W);
	}
}
