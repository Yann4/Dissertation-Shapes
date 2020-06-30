using UnityEngine;
using Dissertation.Input;
using Dissertation.UI;
using System.Collections;

namespace Dissertation.Character.Player
{
	public class PlayerController : BaseCharacterController
	{
		[SerializeField] SpriteRenderer _square;
		[SerializeField] SpriteRenderer _triangle;
		[SerializeField] SpriteRenderer _circle;

		private PlayerConfig _playerConfig;

		private PlayerInventory _inventoryUI;

		private bool _squareUnlocked;
		private bool _triangleUnlocked;
		private bool _circleUnlocked;

		private bool SquareUnlocked
		{
			set
			{
				_squareUnlocked = value;
				App.WorldState.SetState(new Narrative.WorldProperty(ID, Narrative.EProperty.CanMelee, value));
			}

			get
			{
				return _squareUnlocked;
			}
		}

		private bool TriangleUnlocked
		{
			set
			{
				_triangleUnlocked = value;
				App.WorldState.SetState(new Narrative.WorldProperty(ID, Narrative.EProperty.CanShoot, value));
			}

			get
			{
				return _triangleUnlocked;
			}
		}

		private bool CircleUnlocked
		{
			set
			{
				_circleUnlocked = value;
				App.WorldState.SetState(new Narrative.WorldProperty(ID, Narrative.EProperty.CanDash, value));
			}

			get
			{
				return _circleUnlocked;
			}
		}

		private CharacterFaction _shape;
		public CharacterFaction CurrentShape
		{
			get { return _shape; }
			set
			{
				switch (value)
				{
					case CharacterFaction.Square:
						_activeAttack = Attack.Melee;
						_shape = value;
						break;
					case CharacterFaction.Triangle:
						_activeAttack = Attack.Ranged;
						_shape = value;
						break;
					case CharacterFaction.Circle:
						_activeAttack = Attack.Dash;
						_shape = value;
						break;
					case CharacterFaction.Player:
					default:
						Debug.Assert(false, "Don't do that");
						break;
				}
			}
		}

		private Coroutine _transforming = null;
		public bool IsTransforming { get { return _transforming != null; } }

		protected override void Start()
		{
			base.Start();

			Debug.Assert(_config is PlayerConfig);
			_playerConfig = _config as PlayerConfig;

			HUD.Instance.CreateMenu<PlayerHealthUI>().Setup(this);
			_inventoryUI = HUD.Instance.CreateMenu<PlayerInventory>();
			_inventoryUI.Setup(this);

			Health.OnDied += OnDie;

			_square.color = Color.clear;
			_triangle.color = Color.clear;
			_circle.color = Color.clear;

			SquareUnlocked = false;
			CircleUnlocked = false;
			TriangleUnlocked = false;

			switch (_playerConfig.DefaultShape)
			{
				case CharacterFaction.Square:
					_square.color = Color.white;
					SquareUnlocked = true;
					break;
				case CharacterFaction.Triangle:
					_triangle.color = Color.white;
					TriangleUnlocked = true;
					break;
				case CharacterFaction.Circle:
					_circle.color = Color.white;
					CircleUnlocked = true;
					break;
				case CharacterFaction.Player:
				default:
					Debug.Assert(false, "Don't have that as the default shape");
					break;
			}

			CurrentShape = _playerConfig.DefaultShape;

			SetWorldState();
		}

		private void SetWorldState()
		{
			App.WorldState.SetState(new Narrative.WorldProperty(ID, Narrative.EProperty.CanDoubleJump, MaxJumps > 1));
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
			CharacterYoke.Interact = InputManager.GetButton(InputAction.Interact);

			if (InputManager.GetButtonDown(InputAction.ShowInventory))
			{
				_inventoryUI.Toggle();
			}

			if(!IsTransforming && InputManager.GetButtonDown(InputAction.Transform))
			{
				CharacterFaction next = NextFaction(CurrentShape);

				if (next != CurrentShape)
				{
					_transforming = StartCoroutine(TransformInto(_playerConfig.TransformDuration, next));
				}
			}

			base.Update();
		}

		private void OnDie(BaseCharacterController died)
		{
			StartCoroutine(HandleDeath());
		}

		public void UnlockShape(CharacterFaction shape)
		{
			switch (shape)
			{
				case CharacterFaction.Square:
					SquareUnlocked = true;
					break;
				case CharacterFaction.Triangle:
					TriangleUnlocked = true;
					break;
				case CharacterFaction.Circle:
					CircleUnlocked = true;
					break;
			}
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

		private IEnumerator TransformInto(float duration, CharacterFaction transformTo)
		{
			SpriteRenderer current;
			switch (CurrentShape)
			{
				case CharacterFaction.Square:
					current = _square;
					break;
				case CharacterFaction.Triangle:
					current = _triangle;
					break;
				case CharacterFaction.Circle:
					current = _circle;
					break;
				case CharacterFaction.Player:
				default:
					current = null;
					Debug.Assert(false, "Don't do that");
					break;
			}

			SpriteRenderer next;
			switch (transformTo)
			{
				case CharacterFaction.Square:
					next = _square;
					break;
				case CharacterFaction.Triangle:
					next = _triangle;
					break;
				case CharacterFaction.Circle:
					next = _circle;
					break;
				case CharacterFaction.Player:
				default:
					next = null;
					Debug.Assert(false, "Don't do that");
					break;
			}

			if (current == null || next == null)
			{
				yield break;
			}

			float t = 0.0f;
			float startTime = Time.time;
			while(t <= 1.0f)
			{
				t = (Time.time - startTime) / duration;
				next.color = Color.Lerp(Color.clear, Color.white, t);
				current.color = Color.Lerp(Color.white, Color.clear, t);

				yield return null;
			}

			CurrentShape = transformTo;
			_characterSprite = next;
			_transforming = null;
		}

		public override bool CanDashAttack()
		{
			return base.CanDashAttack() && !IsTransforming;
		}

		public override bool CanMeleeAttack()
		{
			return base.CanMeleeAttack() && !IsTransforming;
		}

		public override bool CanRangedAttack()
		{
			return base.CanRangedAttack() && !IsTransforming;
		}

		private CharacterFaction NextFaction(CharacterFaction from)
		{
			CharacterFaction next = from + 1;
			int numFactions = System.Enum.GetValues(typeof(CharacterFaction)).Length;

			while (!FactionUnlocked(next))
			{
				if ((int)next >= numFactions)
				{
					next = 0;
				}
				else
				{
					next++;
				}
			}

			return next;
		}

		private bool FactionUnlocked(CharacterFaction faction)
		{
			switch (faction)
			{
				case CharacterFaction.Square:
					return SquareUnlocked;
				case CharacterFaction.Triangle:
					return TriangleUnlocked;
				case CharacterFaction.Circle:
					return CircleUnlocked;
			}

			return false;
		}
	}
}