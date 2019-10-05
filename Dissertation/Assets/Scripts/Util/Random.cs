using UnityEngine;

namespace Dissertation.Util
{
	public static class Rand
	{
		private static System.Random _random = new System.Random();

		public static int Next()
		{
			return _random.Next();
		}

		public static int Next(int max)
		{
			return _random.Next(max);
		}

		public static int Next(int min, int max)
		{
			return _random.Next(min, max);
		}

		public static float Next(float max)
		{
			return UnityEngine.Random.Range(0.0f, max);
		}

		public static float Next(float min, float max)
		{
			return UnityEngine.Random.Range(min, max);
		}

		//Assumes a flat platform
		public static Vector2 GetRandomPointOnPlatform(Transform platform)
		{
			Debug.Assert(platform != null);
			BoxCollider2D collider = platform.GetComponent<BoxCollider2D>();
			Debug.Assert(collider != null);

			return new Vector2(Rand.Next(collider.bounds.min.x, collider.bounds.max.x), collider.bounds.max.y);
		}
	}
}