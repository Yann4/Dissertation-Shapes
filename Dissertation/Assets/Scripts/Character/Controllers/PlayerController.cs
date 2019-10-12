using UnityEngine;
using Dissertation.Input;
using Dissertation.UI;
using System.Collections;

namespace Dissertation.Character.Player
{
	public class PlayerController : BaseCharacterController
	{
		private PlayerConfig _playerConfig;

		private PlayerInventory _inventoryUI;

		protected override void Start()
		{
			base.Start();

			Debug.Assert(_config is PlayerConfig);
			_playerConfig = _config as PlayerConfig;

			HUD.Instance.CreateMenu<PlayerHealthUI>().Setup(this);
			_inventoryUI = HUD.Instance.CreateMenu<PlayerInventory>();
			_inventoryUI.Setup(this);

			Health.OnDied += OnDie;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			Health.OnDied -= OnDie;
		}

		protected override void Update()
		{
			CharacterYoke.Movement = new Vector2(InputManager.GetAxis(InputAction.MoveHorizontal), InputManager.GetAxis(InputAction.MoveVertical));
			CharacterYoke.Jump = InputManager.GetButton(InputAction.Jump);
			CharacterYoke.Drop = InputManager.GetButton(InputAction.Drop);
			CharacterYoke.MeleeAttack = InputManager.GetButton(InputAction.MeleeAttack);
			CharacterYoke.RangedAttack = InputManager.GetButton(InputAction.RangedAttack);
			CharacterYoke.DashAttack = InputManager.GetButton(InputAction.DashAttack);

			if (InputManager.GetButtonDown(InputAction.ShowInventory))
			{
				_inventoryUI.Toggle();
			}

			base.Update();
		}

		private void OnDie()
		{
			StartCoroutine(HandleDeath());
		}

		private IEnumerator HandleDeath()
		{
			HUD.Instance.CreateMenu<PlayerDeathScreen>();

			_characterSprite.enabled = false;

			yield return new WaitForSeconds(1.0f);

			transform.position = _spawnedBy.transform.position;
			_characterSprite.enabled = true;

			Health.Respawn();
		}
	}
}