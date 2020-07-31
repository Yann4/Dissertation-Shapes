using Dissertation.Character;
using Dissertation.Character.Player;
using UnityEngine;

namespace Dissertation.Environment
{
	public class Wall : MonoBehaviour
	{
		[SerializeField] Sprite[] _damageStates;

		private SpriteRenderer _renderer;
		private int _hitsRemaining = 3;

		private void Start()
		{
			_renderer = GetComponent<SpriteRenderer>();
			_hitsRemaining = _damageStates.Length - 1;

			Debug.Assert(_renderer != null);
			Debug.Assert(_hitsRemaining > 0);

			_renderer.sprite = _damageStates[_hitsRemaining];
		}

		public void Damage(DamageSource source)
		{
			PlayerController player = source.Owner as PlayerController;
			if (player != null && player.CurrentShape == CharacterFaction.Square)
			{
				_hitsRemaining--;

				if(_hitsRemaining >= 0)
				{
					_renderer.sprite = _damageStates[_hitsRemaining];
				}
				else
				{
					gameObject.SetActive(false);
				}
			}
		}
	}
}