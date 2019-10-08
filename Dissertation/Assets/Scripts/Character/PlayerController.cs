using UnityEngine;
using Dissertation.Input;
using Dissertation.UI;

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