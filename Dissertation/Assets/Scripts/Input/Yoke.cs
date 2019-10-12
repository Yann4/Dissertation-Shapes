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

		private Tuple<bool, int> _meleeAttack;
		public bool MeleeAttack
		{
			get { return _meleeAttack.Item1; }
			set
			{
				if (value != _meleeAttack.Item1)
				{
					_meleeAttack = new Tuple<bool, int>(value, Time.frameCount);
				}
			}
		}

		private Tuple<bool, int> _rangedAttack;
		public bool RangedAttack
		{
			get { return _rangedAttack.Item1; }
			set
			{
				if (value != _rangedAttack.Item1)
				{
					_rangedAttack = new Tuple<bool, int>(value, Time.frameCount);
				}
			}
		}

		private Tuple<bool, int> _dashAttack;
		public bool DashAttack
		{
			get { return _dashAttack.Item1; }
			set
			{
				if (value != _dashAttack.Item1)
				{
					_dashAttack = new Tuple<bool, int>(value, Time.frameCount);
				}
			}
		}

		public Yoke()
		{
			int frameCount = Time.frameCount;
			_movement = new Tuple<Vector2, int>(Vector2.zero, frameCount);
			_jump = new Tuple<bool, int>(false, frameCount);
			_drop = new Tuple<bool, int>(false, frameCount);
			_meleeAttack = new Tuple<bool, int>(false, frameCount);
			_rangedAttack = new Tuple<bool, int>(false, frameCount);
			_dashAttack = new Tuple<bool, int>(false, frameCount);
		}

		public void Reset()
		{
			Movement = Vector2.zero;
			Jump = false;
			Drop = false;
			MeleeAttack = false;
			RangedAttack = false;
			DashAttack = false;
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
				case InputAction.MeleeAttack:
					return MeleeAttack;
				case InputAction.RangedAttack:
					return RangedAttack;
				case InputAction.DashAttack:
					return DashAttack;
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
				case InputAction.MeleeAttack:
					return !MeleeAttack && _meleeAttack.Item2 == Time.frameCount;
				case InputAction.RangedAttack:
					return !RangedAttack && _rangedAttack.Item2 == Time.frameCount;
				case InputAction.DashAttack:
					return !DashAttack && _dashAttack.Item2 == Time.frameCount;
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
					return Movement.x != 0.0f;
				case InputAction.MoveVertical:
					Debug.LogWarning("It doesn't make very much sense to call GetDown on an axis value");
					return Movement.y != 0.0f;
				case InputAction.Jump:
					return Jump && _jump.Item2 == Time.frameCount;
				case InputAction.Drop:
					return Drop && _drop.Item2 == Time.frameCount;
				case InputAction.MeleeAttack:
					return MeleeAttack && _meleeAttack.Item2 == Time.frameCount;
				case InputAction.RangedAttack:
					return RangedAttack && _rangedAttack.Item2 == Time.frameCount;
				case InputAction.DashAttack:
					return DashAttack && _dashAttack.Item2 == Time.frameCount;
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
					return Movement.x;
				case InputAction.MoveVertical:
					return Movement.y;
				case InputAction.Jump:
					return BoolToFloat(Jump);
				case InputAction.Drop:
					return BoolToFloat(Drop);
				case InputAction.MeleeAttack:
					return BoolToFloat(MeleeAttack);
				case InputAction.RangedAttack:
					return BoolToFloat(RangedAttack);
				case InputAction.DashAttack:
					return BoolToFloat(DashAttack);
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