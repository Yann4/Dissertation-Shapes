using UnityEngine;

namespace Dissertation.Character
{
	[CreateAssetMenu(fileName = "SpawnConfig.asset", menuName = "Dissertation/Scriptables/Spawn Config")]
	public class SpawnConfig : ScriptableObject
	{
		[System.Serializable]
		public struct SpawnPair
		{
			public GameObject Prefab;
			public CharacterFaction Type;
		}

		[SerializeField] private SpawnPair[] _toSpawn;

		public GameObject GetPrefab(CharacterFaction type)
		{
			foreach(SpawnPair pair in _toSpawn)
			{
				if(pair.Type == type)
				{
					return pair.Prefab;
				}
			}

			return null;
		}
	}
}