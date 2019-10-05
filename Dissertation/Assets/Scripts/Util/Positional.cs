using UnityEngine;

namespace Dissertation.Util
{
	public static class Positional
	{
		public static bool IsAtPosition(Transform transform, Vector3 position, float tolerance = 1.0f)
		{
			return Vector3.SqrMagnitude(transform.position - position) < (tolerance * tolerance);
		}

		public static Transform GetCurrentPlatform(Transform character)
		{
			return GetPlatform(character.position);
		}

		public static Transform GetPlatform(Vector3 position)
		{
			RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 20.0f, Layers.GroundMask);
			return hit.transform;
		}
	}
}