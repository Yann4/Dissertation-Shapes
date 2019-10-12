using Dissertation.Util;
using System;
using UnityEngine;

namespace Dissertation.Character
{
	public class Spawner : MonoBehaviour
	{
		[SerializeField] private CharacterFaction _spawnerType;
		[SerializeField] private SpawnConfig _config;

		[SerializeField] private bool _primaryPlayerSpawnPoint = false;

		public static Action<BaseCharacterController> OnSpawn;

		private void Start()
		{
			if(_spawnerType == CharacterFaction.Player && !_primaryPlayerSpawnPoint)
			{
				return;
			}

			BaseCharacterController spawned = Instantiate(_config.GetPrefab(_spawnerType)).GetComponent<BaseCharacterController>();
			spawned.OnSpawn(this);

			OnSpawn.InvokeSafe(spawned);
		}
	}
}