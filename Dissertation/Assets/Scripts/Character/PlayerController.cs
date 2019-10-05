using UnityEngine;
using Dissertation.Input;

namespace Dissertation.Character.Player
{
	public class PlayerController : BaseCharacterController
	{
		//Jumping state variables
		private float _jumpStartTime;
		private float _jumpAvailable; //The amount of "jump power" that you have left for this jump. To allow tapping the button and holding it to jump to different heights
		private bool _canJump = false;

		private PlayerConfig _playerConfig;

		protected override void Awake()
		{
			base.Awake();

			Debug.Assert(_config is PlayerConfig);
			_playerConfig = _config as PlayerConfig;
		}

		private void Update()
		{
			if (IsGrounded)
			{
				_velocity.y = 0;
			}

			if ((_playerConfig.CanDoubleJump || IsGrounded) && !InputManager.GetButton(InputAction.Jump))
			{
				_jumpAvailable = _playerConfig.JumpHeight;
				_canJump = true;
			}

			float horizontalMovement = InputManager.GetAxis(InputAction.MoveHorizontal);
			if (horizontalMovement > 0)
			{
				if (transform.localScale.x < 0f)
				{
					transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
				}
			}
			else if (horizontalMovement < 0)
			{
				if (transform.localScale.x > 0f)
				{
					transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
				}
			}

			// we can only jump whilst grounded
			if (_canJump && InputManager.GetButton(InputAction.Jump) && _jumpAvailable > 0.0f)
			{
				if (InputManager.GetButtonDown(InputAction.Jump))
				{
					_jumpStartTime = Time.time;
				}

				float jumpThisFrame = _playerConfig.GetJumpSpeed(Time.time - _jumpStartTime) * Time.deltaTime;
				_jumpAvailable -= jumpThisFrame;
				_velocity.y = Mathf.Sqrt(2f * jumpThisFrame * -_config.Gravity);
			}
			else
			{
				// apply gravity before moving
				_velocity.y += _config.Gravity * Time.deltaTime;
			}

			if (InputManager.GetButtonUp(InputAction.Jump))
			{
				_canJump = false;
			}

			_velocity.x = horizontalMovement * _config.RunSpeed;

			// if holding down bump up our movement amount and turn off one way platform detection for a frame.
			// this lets us jump down through one way platforms
			if (IsGrounded && InputManager.GetButtonDown(InputAction.Drop))
			{
				_velocity.y *= 3f;
				IgnoreOneWayPlatformsThisFrame = true;
			}

			_velocity = MoveBy(_velocity, _velocity * Time.deltaTime);
		}
	}
}