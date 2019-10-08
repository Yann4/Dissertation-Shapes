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
			public CharacterType Type;
		}

		[SerializeField] private SpawnPair[] _toSpawn;

		public GameObject GetPrefab(CharacterType type)
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