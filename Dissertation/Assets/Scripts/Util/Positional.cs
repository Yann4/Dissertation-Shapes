using UnityEngine;

namespace Dissertation.Util
{
	public static class Positional
	{
		public static bool IsAtPosition(Transform transform, Vector3 position, float tolerance = 1.0f)
		{
			return Vector3.SqrMagnitude(transform.position - position) < (tolerance * tolerance);
		}

		public static Transform GetCurrentPlatform(Transform character, float distanceToCheck = 3.0f)
		{
			return GetPlatform(character.position, distanceToCheck);
		}

		public static Transform GetPlatform(Vector3 position, float distanceToCheck = 3.0f)
		{
			RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, distanceToCheck, Layers.GroundMask);
			return hit.transform;
		}
	}
}