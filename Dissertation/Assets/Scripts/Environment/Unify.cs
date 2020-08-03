using Dissertation.Character.Player;
using System;
using System.Collections;
using UnityEngine;

namespace Dissertation.Environment
{
	public class Unify : MonoBehaviour
	{
		private enum UnlockAction { Win, ToggleActive };

		[SerializeField, Tooltip("Should have 3 renderers in the order square, triangle, circle")] private SpriteRenderer[] _renderers = new SpriteRenderer[3];
		[SerializeField] private bool _squareEnabled = true;
		[SerializeField] private bool _triangleEnabled = true;
		[SerializeField] private bool _circleEnabled = true;

		[SerializeField] private float _triggerDelay = 5.0f;

		[SerializeField] private UnlockAction _onUnlock;
		[SerializeField, Tooltip("Only used for certain values of onUnlock")] private GameObject _unlockSubject;

		private Coroutine[] _unlock = new Coroutine[3];

		private bool _hasRunUnlock = false;
		private bool IsUnlocked
		{
			get
			{
				return (!_squareEnabled || _unlock[0] != null) &&
						(!_triangleEnabled || _unlock[1] != null) &&
						(!_circleEnabled || _unlock[2] != null);
			}
		}

		private void Start()
		{
			Debug.Assert(_renderers.Length == 3, "Should have 3 renderers in the order square, triangle, circle", gameObject);

			_renderers[0].enabled = _squareEnabled;
			_renderers[1].enabled = _triangleEnabled;
			_renderers[2].enabled = _circleEnabled;
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			PlayerController player = collision.gameObject.GetComponent<PlayerController>();
			if(player != null)
			{
				CheckLock(player);
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			PlayerController player = collision.gameObject.GetComponent<PlayerController>();
			if (player != null)
			{
				CheckLock(player);
			}
		}

		private void CheckLock(PlayerController player)
		{
			if (player != null && !_hasRunUnlock)
			{
				switch (player.CurrentShape)
				{
					case Character.CharacterFaction.Square:
						if (_squareEnabled && _unlock[0] == null)
						{
							_unlock[0] = StartCoroutine(Unlock(player.CurrentShape));
						}
						break;
					case Character.CharacterFaction.Triangle:
						if (_squareEnabled && _unlock[1] == null)
						{
							_unlock[1] = StartCoroutine(Unlock(player.CurrentShape));
						}
						break;
					case Character.CharacterFaction.Circle:
						if (_squareEnabled && _unlock[2] == null)
						{
							_unlock[2] = StartCoroutine(Unlock(player.CurrentShape));
						}
						break;
					case Character.CharacterFaction.Player:
					default:
						break;
				}

				if (IsUnlocked)
				{
					for (int idx = 0; idx < _unlock.Length; idx++)
					{
						if (_unlock[idx] != null)
						{
							StopCoroutine(_unlock[idx]);
						}
					}

					switch (_onUnlock)
					{
						case UnlockAction.Win:
							App.EndGame(true);
							break;
						case UnlockAction.ToggleActive:
							Debug.Assert(_unlockSubject != null, "Unlock subject is null");
							_unlockSubject.SetActive(!_unlockSubject.activeSelf);
							break;
						default:
							throw new NotImplementedException(string.Format("Unlock action {0} not implemented", _onUnlock));
					}

					_hasRunUnlock = true;
				}
			}
		}

		private IEnumerator Unlock(Character.CharacterFaction faction)
		{
			if(faction == Character.CharacterFaction.Player)
			{
				Debug.LogError("Can't unlock player faction");
				yield break;
			}

			int factionIdx = (int)faction - 1;
			_renderers[factionIdx].enabled = false;

			yield return new WaitForSeconds(_triggerDelay);

			_renderers[factionIdx].enabled = true;
			_unlock[factionIdx] = null;
		}
	}
}