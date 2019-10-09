using Dissertation.Character.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dissertation.UI
{
	public class PlayerHealthUI : MenuBase
	{
		[SerializeField] private LayoutGroup _layout;
		[SerializeField] private GameObject _healthObjectPrefab;

		private PlayerController _player;
		private List<GameObject> _healthPips = new List<GameObject>();

		public void Setup(PlayerController player)
		{
			_player = player;

			Debug.Assert(_player != null);
			Debug.Assert(_layout != null);
			Debug.Assert(_healthObjectPrefab != null);

			_player.Health.OnHealthChanged += OnPlayerHealthChanged;
			_player.Health.OnRespawn += OnRespawn;

			for (int idx = 0; idx < _player.Config.MaxHealth; idx++)
			{
				_healthPips.Add(Instantiate(_healthObjectPrefab, _layout.transform));
			}

			OnPlayerHealthChanged(_player.Health.CurrentHealth);
		}

		private void OnDestroy()
		{
			_player.Health.OnHealthChanged -= OnPlayerHealthChanged;
			_player.Health.OnRespawn -= OnRespawn;
		}

		private void OnPlayerHealthChanged(int currentHealth)
		{
			int idx = 0;
			foreach(GameObject pip in _healthPips)
			{
				pip.SetActive(idx < currentHealth);
				idx++;
			}

			StartCoroutine(DelayedToggleLayout());
		}

		private void OnRespawn()
		{
			OnPlayerHealthChanged(_player.Health.CurrentHealth);
		}

		private IEnumerator DelayedToggleLayout()
		{
			_layout.enabled = true;
			yield return null;
			yield return null;
			_layout.enabled = false;
		}
	}
}