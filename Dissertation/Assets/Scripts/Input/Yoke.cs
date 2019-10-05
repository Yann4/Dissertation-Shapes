using System;
using UnityEngine;

namespace Dissertation.Input
{
	public class Yoke
	{
		private Tuple<Vector2, int> _movement;
		public Vector2 Movement
		{
			get { return _movement.Item1; }
			set
			{
				if (value != _movement.Item1)
				{
					_movement = new Tuple<Vector2, int>(value, Time.frameCount);
				}
			}
		}

		private Tuple<bool, int> _jump;
		public bool Jump
		{
			get { return _jump.Item1; }
			set
			{
				if (value != _jump.Item1)
				{
					_jump = new Tuple<bool, int>(value, Time.frameCount);
				}
			}
		}

		private Tuple<bool, int> _drop;
		public bool Drop
		{
			get { return _drop.Item1; }
			set
			{
				if (value != _drop.Item1)
				{
					_drop = new Tuple<bool, int>(value, Time.frameCount);
				}
			}
		}

		public Yoke()
		{
			int frameCount = Time.frameCount;
			_movement = new Tuple<Vector2, int>(Vector2.zero, frameCount);
			_jump = new Tuple<bool, int>(false, frameCount);
			_drop = new Tuple<bool, int>(false, frameCount);
		}

		public void Reset()
		{
			int currentFrame = Time.frameCount;

			if (_movement.Item2 != currentFrame)
			{
				Movement = Vector2.zero;
			}

			if (_jump.Item2 != currentFrame)
			{
				Jump = false;
			}

			if (_drop.Item2 != currentFrame)
			{
				Drop = false;
			}
		}

		public bool GetButton(InputAction action)
		{
			switch (action)
			{
				case InputAction.MoveHorizontal:
					return Movement.y != 0.0f;
				case InputAction.MoveVertical:
					return Movement.x != 0.0f;
				case InputAction.Jump:
					return Jump;
				case InputAction.Drop:
					return Drop;
				default:
					Debug.AssertFormat(false, "Action {0} needs adding to the yoke", action);
					return false;
			}
		}

		public bool GetButtonUp(InputAction action)
		{
			switch (action)
			{
				case InputAction.MoveHorizontal:
					Debug.LogWarning("It doesn't make very much sense to call GetUp on an axis value");
					return Movement.y != 0.0f;
				case InputAction.MoveVertical:
					Debug.LogWarning("It doesn't make very much sense to call GetUp on an axis value");
					return Movement.x != 0.0f;
				case InputAction.Jump:
					return !Jump && _jump.Item2 == Time.frameCount;
				case InputAction.Drop:
					return !Drop && _drop.Item2 == Time.frameCount;
				default:
					Debug.AssertFormat(false, "Action {0} needs adding to the yoke", action);
					return false;
			}
		}

		public bool GetButtonDown(InputAction action)
		{
			switch (action)
			{
				case InputAction.MoveHorizontal:
					Debug.LogWarning("It doesn't make very much sense to call GetDown on an axis value");
					return Movement.y != 0.0f;
				case InputAction.MoveVertical:
					Debug.LogWarning("It doesn't make very much sense to call GetDown on an axis value");
					return Movement.x != 0.0f;
				case InputAction.Jump:
					return Jump && _jump.Item2 == Time.frameCount;
				case InputAction.Drop:
					return Drop && _drop.Item2 == Time.frameCount;
				default:
					Debug.AssertFormat(false, "Action {0} needs adding to the yoke", action);
					return false;
			}
		}

		public float GetAxis(InputAction action)
		{
			switch (action)
			{
				case InputAction.MoveHorizontal:
					return Movement.y;
				case InputAction.MoveVertical:
					return Movement.x;
				case InputAction.Jump:
					return BoolToFloat(Jump);
				case InputAction.Drop:
					return BoolToFloat(Drop);
				default:
					Debug.AssertFormat(false, "Action {0} needs adding to the yoke", action);
					return 0.0f;
			}
		}

		private float BoolToFloat(bool val)
		{
			return val ? 1.0f : 0.0f;
		}
	}
}