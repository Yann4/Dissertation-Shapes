using UnityEngine;
using Dissertation.Input;
using Dissertation.UI;
using System.Collections;

namespace Dissertation.Character.Player
{
	public class PlayerController : BaseCharacterController
	{
		private PlayerConfig _playerConfig;

		protected override void Start()
		{
			base.Start();

			Debug.Assert(_config is PlayerConfig);
			_playerConfig = _config as PlayerConfig;

			HUD.Instance.CreateMenu<PlayerHealthUI>().Setup(this);

			Health.OnDied += OnDie;
		}

		private void OnDestroy()
		{
			Health.OnDied -= OnDie;
		}

		protected override void Update()
		{
			CharacterYoke.Movement = new Vector2(InputManager.GetAxis(InputAction.MoveHorizontal), InputManager.GetAxis(InputAction.MoveVertical));
			CharacterYoke.Jump = InputManager.GetButton(InputAction.Jump);
			CharacterYoke.Drop = InputManager.GetButton(InputAction.Drop);

			base.Update();
		}

		private void OnDie()
		{
			StartCoroutine(HandleDeath());
		}

		private IEnumerator HandleDeath()
		{
			HUD.Instance.CreateMenu<PlayerDeathScreen>();

			_sprite.enabled = false;

			yield return new WaitForSeconds(1.0f);

			transform.position = _spawnedBy.transform.position;
			_sprite.enabled = true;

			Health.Respawn();
		}
	}
}