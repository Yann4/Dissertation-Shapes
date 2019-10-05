using UnityEngine;

namespace Dissertation.Character.Player
{
	[CreateAssetMenu(fileName = "PlayerConfig.asset", menuName = "Dissertation/Scriptables/Player Config")]
	public class PlayerConfig : CharacterConfig
	{
		[SerializeField] private float _jumpHeight = 3f;
		public float JumpHeight { get { return _jumpHeight; } }

		[SerializeField] private AnimationCurve _jumpSpeed;
		public float GetJumpSpeed(float t) { return _jumpSpeed.Evaluate(t); }

		[SerializeField] private bool _canDoubleJump;
		public bool CanDoubleJump { get { return _canDoubleJump; } }
	}
}