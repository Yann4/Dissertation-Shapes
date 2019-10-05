using UnityEngine;

namespace Dissertation.Util
{
	public static class Positional
	{
		public enum Direction
		{
			Left,
			Right
		}

		public static bool IsAtPosition(Transform transform, Vector3 position, float tolerance = 1.0f)
		{
			return Vector3.SqrMagnitude(transform.position - position) < (tolerance * tolerance);
		}
	}
}