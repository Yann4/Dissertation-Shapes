using System;
using UnityEngine;

namespace Dissertation.Environment
{
	[CreateAssetMenu(fileName = "World Config.asset", menuName = "Dissertation/Scriptables/World Config")]
	public class WorldConfig : ScriptableObject
	{
		[Serializable]
		public struct SceneLocation
		{
			public string WorldSceneName;
			public Vector3 Offset;
		}

		public SceneLocation[] Scenes;
	}
}