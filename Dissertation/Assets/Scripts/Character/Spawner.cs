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
		public Action<BaseCharacterController> OnSpawnNonStatic;

		private void Start()
		{
			if(_spawnerType == CharacterFaction.Player && !_primaryPlayerSpawnPoint)
			{
				return;
			}

			BaseCharacterController spawned = Instantiate(_config.GetPrefab(_spawnerType), transform.position, Quaternion.identity).GetComponent<BaseCharacterController>();
			spawned.OnSpawn(this);

			OnSpawn.InvokeSafe(spawned);
			OnSpawnNonStatic.InvokeSafe(spawned);
		}
	}
}