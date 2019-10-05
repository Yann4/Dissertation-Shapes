using UnityEngine;
using Dissertation.Input;

namespace Dissertation.Character.Player
{
	public class PlayerController : BaseCharacterController
	{
		private PlayerConfig _playerConfig;

		protected override void Awake()
		{
			base.Awake();

			Debug.Assert(_config is PlayerConfig);
			_playerConfig = _config as PlayerConfig;
		}

		protected override void Update()
		{
			CharacterYoke.Movement = new Vector2(InputManager.GetAxis(InputAction.MoveHorizontal), InputManager.GetAxis(InputAction.MoveVertical));
			CharacterYoke.Jump = InputManager.GetButton(InputAction.Jump);
			CharacterYoke.Drop = InputManager.GetButton(InputAction.Drop);

			base.Update();
		}
	}
}