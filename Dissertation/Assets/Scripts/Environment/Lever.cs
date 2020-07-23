using Dissertation.Util;
using UnityEngine;

namespace Dissertation.Environment
{
	[RequireComponent(typeof(Collider2D)), RequireComponent(typeof(SpriteRenderer))]
	public class Lever : MonoBehaviour
	{
		private SpriteRenderer _renderer;

		[SerializeField] private Sprite _onSprite;
		[SerializeField] private Sprite _offSprite;

		[SerializeField] private GameObject _toToggle;
		[SerializeField] private bool _startOn = true;
		[SerializeField] private bool _runOnce = true;

		private bool _state;
		private bool _hasRun = false;

		void Start()
		{
			_renderer = GetComponent<SpriteRenderer>();
			SetState(_startOn);
		}

		private void SetState(bool state)
		{
			_renderer.sprite = state ? _onSprite : _offSprite;
			_state = state;
			_toToggle.SetActive(state);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!_runOnce || (_runOnce && !_hasRun))
			{
				SetState(!_state);
				_hasRun = true;
			}
		}
	}
}