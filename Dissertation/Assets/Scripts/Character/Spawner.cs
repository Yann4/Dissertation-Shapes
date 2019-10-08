using UnityEngine;

namespace Dissertation.Character
{
	public class Spawner : MonoBehaviour
	{
		[SerializeField] private CharacterType _spawnerType;
		[SerializeField] private SpawnConfig _config;

		[SerializeField] private bool _primaryPlayerSpawnPoint = false;

		private void Start()
		{
			if(_spawnerType == CharacterType.Player && !_primaryPlayerSpawnPoint)
			{
				return;
			}

			BaseCharacterController spawned = Instantiate(_config.GetPrefab(_spawnerType)).GetComponent<BaseCharacterController>();
			spawned.OnSpawn(this);
		}
	}
}