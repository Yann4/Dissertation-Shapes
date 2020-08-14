using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dissertation.Character;
using Dissertation.Character.Player;

namespace Dissertation.UI
{
	public class Respawner : MenuBase
	{
		[SerializeField] private float _respawnTime = 60.0f * 3f;
		[SerializeField] private GameObject _respawnUI;

		private List<BaseCharacterController> _deadCharacters = new List<BaseCharacterController>();

		private void Awake()
		{
			Spawner.OnSpawn += OnCharacterSpawn;
		}

		private void Start()
		{
			StartCoroutine(Respawn());
		}

		private void OnCharacterSpawn(BaseCharacterController character)
		{
			if(character is PlayerController)
			{
				return;
			}

			character.Health.OnDied += OnDie;
		}

		private void OnDie(BaseCharacterController obj)
		{
			if (!_deadCharacters.Find(character => obj == character))
			{
				_deadCharacters.Add(obj);
			}
		}

		private IEnumerator Respawn()
		{
			yield return new WaitForSeconds(_respawnTime);

			if (_deadCharacters.Count > 0)
			{
				foreach(BaseCharacterController character in _deadCharacters)
				{
					character.Health.Respawn();
				}

				_respawnUI.SetActive(true);

				yield return new WaitForSeconds(3.0f);

				_respawnUI.SetActive(false);

				_deadCharacters.Clear();
			}

			StartCoroutine(Respawn());
		}
	}
}