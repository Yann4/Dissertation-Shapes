using Dissertation.Util;
using System;
using UnityEngine;

namespace Dissertation.Character
{
	public class Spawner : MonoBehaviour
	{
		[SerializeField] private CharacterFaction _spawnerType;
		[SerializeField] private CharacterConfig _characterConfig;

		private static SpawnConfig _config;

		[SerializeField] private bool _primaryPlayerSpawnPoint = false;

		public static Action<BaseCharacterController> OnSpawn;
		public Action<BaseCharacterController> OnSpawnNonStatic;

		private void Awake()
		{
			if (_config == null)
			{
				_config = Resources.Load<SpawnConfig>("SpawnConfig");
			}
		}

		private void Start()
		{
			if(_spawnerType == CharacterFaction.Player && !_primaryPlayerSpawnPoint)
			{
				return;
			}

			BaseCharacterController spawned = Instantiate(_config.GetPrefab(_spawnerType), transform.position, Quaternion.identity).GetComponent<BaseCharacterController>();
			spawned.OnSpawn(this, _characterConfig);

			OnSpawn.InvokeSafe(spawned);
			OnSpawnNonStatic.InvokeSafe(spawned);
		}
	}
}