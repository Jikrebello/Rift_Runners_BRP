using System.Numerics;

namespace Assets.Scripts.Game.SharedKernel.Math
{
	public static class NumericsMath
	{
		public static float Length(this Vector2 v) => v.Length();

		public static Vector2 Normalized(this Vector2 v) =>
			v == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(v);
	}
}
