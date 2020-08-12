#if UNITY_EDITOR
#define DEBUG_CC2D_RAYS
#endif

using Dissertation.Environment;
using Dissertation.Input;
using Dissertation.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character
{
	public abstract class BaseCharacterController : MonoBehaviour
	{
		/*
		 * This code has been adapted from https://github.com/prime31/CharacterController2D
		 * which was released under Attribution-NonCommercial-ShareAlike 3.0 Unported
		 * (https://creativecommons.org/licenses/by-nc-sa/3.0/legalcode) (with a simplified 
		 * explanation here https://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US)
		*/

		struct CharacterRaycastOrigins
		{
			public Vector3 TopLeft;
			public Vector3 BottomRight;
			public Vector3 BottomLeft;
		}

		public class CharacterCollisionState2D
		{
			public bool Right;
			public bool Left;
			public bool Above;
			public bool Below;
			public bool BecameGroundedThisFrame;
			public bool WasGroundedLastFrame;
			public bool MovingDownSlope;
			public float SlopeAngle;

			public bool HasCollision()
			{
				return Below || Right || Left || Above;
			}

			public void Reset()
			{
				Right = Left = Above = Below = BecameGroundedThisFrame = MovingDownSlope = false;
				SlopeAngle = 0f;
			}

			public override string ToString()
			{
				return string.Format("[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}, movingDownSlope: {4}, angle: {5}, wasGroundedLastFrame: {6}, becameGroundedThisFrame: {7}",
									 Right, Left, Above, Below, MovingDownSlope, SlopeAngle, WasGroundedLastFrame, BecameGroundedThisFrame);
			}
		}

		[SerializeField, Range(0.001f, 0.3f)] private float _skinWidth = 0.02f;

		/// <summary>
		/// defines how far in from the edges of the collider rays are cast from. If cast with a 0 extent it will often result in ray hits that are
		/// not desired (for example a foot collider casting horizontally from directly on the surface can result in a hit)
		/// </summary>
		public float SkinWidth
		{
			get { return _skinWidth; }
			set
			{
				_skinWidth = value;
				RecalculateDistanceBetweenRays();
			}
		}

		/// <summary>
		/// the max slope angle that the CC2D can climb
		/// </summary>
		/// <value>The slope limit.</value>
		[SerializeField, Range(0f, 90f)] private float _slopeLimit = 30f;

		/// <summary>
		/// the threshold in the change in vertical movement between frames that constitutes jumping
		/// </summary>
		/// <value>The jumping threshold.</value>
		[SerializeField] private float _jumpingThreshold = 0.07f;

		/// <summary>
		/// curve for multiplying speed based on slope (negative = down slope and positive = up slope)
		/// </summary>
		[SerializeField] private AnimationCurve _slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90f, 1.5f), new Keyframe(0f, 1f), new Keyframe(90f, 0f));

		[SerializeField, Range(2, 20)] private int _totalHorizontalRays = 8;
		[SerializeField, Range(2, 20)] private int _totalVerticalRays = 4;

		[SerializeField] protected CharacterConfig _config;
		public CharacterConfig Config { get { return _config; } }

		[SerializeField] private Collider2D _collider;
		[SerializeField] private Rigidbody2D _rigidBody2D;

		[SerializeField] private CharacterHealth _health;

		public CharacterHealth Health { get { return _health; } }

		[SerializeField] protected SpriteRenderer _characterSprite;

		[SerializeField] protected GameObject _dashAttack;

		private SpriteRenderer _dashAttackHighlightSprite;
		private Collider2D _dashAttackCollider;

		[SerializeField] private Inventory _inventory;
		public Inventory Inventory { get { return _inventory; } private set { _inventory = value; } }

		public Home CharacterHome;

		public CharacterEvents Events { get; private set; } = new CharacterEvents();

		public bool IsGrounded { get { return _collisionState.Below; } }
		protected bool CanMove { get { return !Health.IsDead && !IsMeleeAttacking && !IsDashAttacking; } }

		private float _startedFalling = 0.0f;
		protected float FallingFor { get { return Time.time - _startedFalling; } }

		public float RunSpeedModifier = 1.0f;

		/// <summary>
		/// when true, one way platforms will be ignored when moving vertically for a single frame
		/// </summary>
		protected bool IgnoreOneWayPlatformsThisFrame;

		/// <summary>
		/// this is used to calculate the downward ray that is cast to check for slopes. We use the somewhat arbitrary value 75 degrees
		/// to calculate the length of the ray that checks for slopes.
		/// </summary>
		private readonly float _slopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);

		private CharacterCollisionState2D _collisionState = new CharacterCollisionState2D();

		const float kSkinWidthFloatFudgeFactor = 0.001f;

		/// <summary>
		/// holder for our raycast origin corners (TR, TL, BR, BL)
		/// </summary>
		private CharacterRaycastOrigins _raycastOrigins;

		private LayerMask _enemyMask;
		protected LayerMask EnemyMask { get { return _collideWithEnemies ? _enemyMask : (LayerMask)0; } }

		private bool _collideWithEnemies = true;

		/// <summary>
		/// stores our raycast hit during movement
		/// </summary>
		private RaycastHit2D _raycastHit;

		/// <summary>
		/// stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
		/// horizontally and vertically so that we can send the events after all collision state is set
		/// </summary>
		private List<RaycastHit2D> _raycastHitsThisFrame = new List<RaycastHit2D>(2);

		// horizontal/vertical movement data
		private float _verticalDistanceBetweenRays;
		private float _horizontalDistanceBetweenRays;

		// we use this flag to mark the case where we are travelling up a slope and we modified our delta.y to allow the climb to occur.
		// the reason is so that if we reach the end of the slope we can make an adjustment to stay grounded
		private bool _isGoingUpSlope = false;

		private RaycastHit2D _lastControllerColliderHit;
		private Vector3 _velocity;
		public Facing FacingDirection { get; private set; } = Facing.Right;

		//Jumping state variables
		private float _jumpStartTime;
		private float _jumpAvailable; //The amount of "jump power" that you have left for this jump. To allow tapping the button and holding it to jump to different heights
		private bool _canJump = false;
		private int _numJumps = 0;
		public int MaxJumps;

		public enum Attack { Melee, Ranged, Dash }

		protected Attack _activeAttack;

		//Attacking state variables
		public bool IsAttacking { get { return IsMeleeAttacking || IsRangedAttacking || IsDashAttacking; } }
		public bool IsMeleeAttacking { get; private set; }
		public bool IsRangedAttacking { get; private set; }
		public bool IsDashAttacking { get; private set; }

		private float _meleeAttackEndTime;
		private float _lastRangedAttackTime;
		private float _dashAttackEndTime;

		public Yoke CharacterYoke { get; private set; }
		private EntityID _entity;
		public long ID { get { return _entity.ObjectID; } }

		protected Spawner _spawnedBy;
		protected GameObject _meleeAttack;
		protected PrefabPool _rangedAttackPool;

		public virtual void OnSpawn(Spawner spawner, CharacterConfig config)
		{
			_spawnedBy = spawner;
			if (config != null)
			{
				_config = config;
			}

			Debug.Assert(Config != null);
			_entity = gameObject.AddComponent<EntityID>();
			_entity.SetID(Config.CharacterID);
		}

		protected virtual void Start()
		{
			Debug.Assert(Config != null);
			Debug.Assert(_collider != null);
			Debug.Assert(_rigidBody2D != null);

			// here, we trigger our properties that have setters with bodies
			SkinWidth = _skinWidth;

			_enemyMask = Config.EnemyMask;
			_activeAttack = Config.DefaultAttack;

			MaxJumps = Config.MaxJumps;

			UpdateLayerCollision();

			CharacterYoke = new Yoke();

			Inventory.Initialise(this, Config.DefaultContents);

			_characterSprite.color = Config.SpriteTint;

			//Setup melee attack
			_meleeAttackEndTime = Time.time;
			_lastRangedAttackTime = Time.time;

			Debug.Assert(Config.MeleeAttackPrefab != null);
			_meleeAttack = Instantiate(Config.MeleeAttackPrefab, transform);
			Debug.Assert(_meleeAttack != null);
			_meleeAttack.SetActive(false);
			DamageSource meleeDamageSource = _meleeAttack.GetComponentInChildren<DamageSource>();
			meleeDamageSource.Setup(this, Config.BaseMeleeDamage);
			meleeDamageSource.OnHit += OnMeleeHit;

			//Setup ranged attack
			_rangedAttackPool = new PrefabPool(Config.RangedAttackPrefab, 5, false, transform);

			//Setup dash attack
			_dashAttackHighlightSprite = _dashAttack.GetComponent<SpriteRenderer>();
			_dashAttackHighlightSprite.color = Config.SpriteTint;

			_dashAttackCollider = _dashAttack.GetComponent<Collider2D>();
			_dashAttackCollider.enabled = false;

			DamageSource dashDamageSource = _dashAttack.GetComponent<DamageSource>();
			dashDamageSource.Setup(this, Config.DashAttackBaseDamage);
			dashDamageSource.OnHit += OnDashHit;

			_dashAttack.SetActive(false);
		}

		protected virtual void OnDestroy()
		{
			DamageSource melee = _meleeAttack.GetComponentInChildren<DamageSource>();
			melee.OnHit -= OnMeleeHit;

			Destroy(_meleeAttack);

			DamageSource dash = _dashAttack.GetComponent<DamageSource>();
			dash.OnHit -= OnDashHit;
		}

		protected virtual void Update()
		{
			HandleAttacking();

			HandleMovement();

			//If we've been falling for 5 seconds, kill ourselves
			if (!IsGrounded && FallingFor >= 5.0f)
			{
				Health.Kill();
			}

			HandleSpriteFacing();
		}

		private void HandleAttacking()
		{
			if (CanMeleeAttack() && CharacterYoke.GetButtonDown(InputAction.MeleeAttack))
			{
				StartCoroutine(MeleeAttack());
			}

			if(CanRangedAttack() && CharacterYoke.GetButtonDown(InputAction.RangedAttack))
			{
				StartCoroutine(RangedAttack());
				_lastRangedAttackTime = Time.time;
			}

			if(CanDashAttack() && CharacterYoke.GetButtonDown(InputAction.DashAttack))
			{
				StartCoroutine(DashAttack());
			}
		}

		private void HandleMovement()
		{
			if (IsGrounded)
			{
				_velocity.y = 0;
				_numJumps = 0;
			}

			if (CanMove)
			{
				float horizontalMovement = CharacterYoke.Movement.x;

				if ((_numJumps < MaxJumps || IsGrounded) && !CharacterYoke.Jump)
				{
					_jumpAvailable = _config.JumpHeight;
					_canJump = true;
				}

				// we can only jump whilst grounded
				if (_canJump && CharacterYoke.Jump && _jumpAvailable > 0.0f)
				{
					if (CharacterYoke.GetButtonDown(InputAction.Jump))
					{
						_jumpStartTime = Time.time;
						_numJumps++;
					}

					float jumpThisFrame = _config.GetJumpSpeed(Time.time - _jumpStartTime) * Time.deltaTime;
					_jumpAvailable -= jumpThisFrame;
					_velocity.y = Mathf.Sqrt(2f * jumpThisFrame * -_config.Gravity);
				}
				else
				{
					// apply gravity before moving
					_velocity.y += _config.Gravity * Time.deltaTime;
				}

				if (CharacterYoke.GetButtonUp(InputAction.Jump))
				{
					_canJump = false;
				}

				_velocity.x = horizontalMovement * _config.RunSpeed * RunSpeedModifier;

				// if holding down bump up our movement amount and turn off one way platform detection for a frame.
				// this lets us jump down through one way platforms
				if (IsGrounded && CharacterYoke.GetButtonDown(InputAction.Drop))
				{
					_velocity.y *= 3f;
					IgnoreOneWayPlatformsThisFrame = true;
				}
			}
			else
			{
				_velocity.x = 0.0f;
			}

			_velocity = MoveBy(_velocity, _velocity * Time.deltaTime);
		}

		private void HandleSpriteFacing()
		{
			float horizontalMovement = CharacterYoke.Movement.x;
			Transform toRotate = _characterSprite.transform;
			if (horizontalMovement > 0 && toRotate.localScale.x < 0f)
			{
				toRotate.localScale = new Vector3(-toRotate.localScale.x, toRotate.localScale.y, toRotate.localScale.z);
				FacingDirection = Facing.Right;
			}
			else if (horizontalMovement < 0 && toRotate.localScale.x > 0f)
			{
				toRotate.localScale = new Vector3(-toRotate.localScale.x, toRotate.localScale.y, toRotate.localScale.z);
				FacingDirection = Facing.Left;
			}
		}

		public virtual bool CanMeleeAttack()
		{
			return !IsAttacking && !Health.IsDead && _activeAttack == Attack.Melee
				&& (Time.time - _meleeAttackEndTime >= Config.MeleeAttackCooldown);
		}

		public virtual bool CanRangedAttack()
		{
			return !IsAttacking && !Health.IsDead && _activeAttack == Attack.Ranged
				&& (Time.time - _lastRangedAttackTime >= Config.RangedAttackCooldown);
		}

		public virtual bool CanDashAttack()
		{
			return !IsAttacking && !Health.IsDead && !_dashAttack.activeSelf && _activeAttack == Attack.Dash
				&& (Time.time - _dashAttackEndTime >= Config.DashAttackCooldown);
		}

		protected IEnumerator MeleeAttack()
		{
			_meleeAttack.transform.localScale = Vector3.zero;

			Vector3 position = _collider.bounds.center;
			float target = 1.0f;

			float vertical = CharacterYoke.GetAxis(InputAction.MoveVertical);
			if (vertical != 0.0f)
			{
				if(vertical < 0.0f)
				{
					//Trying to hit down
					//If we're on the ground, we can't slash down
					if(IsGrounded)
					{
						yield break;
					}

					_meleeAttack.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
					position.y -= _collider.bounds.extents.y;
				}
				else
				{
					_meleeAttack.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
					position.y += _collider.bounds.extents.y;
				}
			}
			else
			{
				_meleeAttack.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

				if (FacingDirection == Facing.Right)
				{
					target = -1.0f;
					position.x += _collider.bounds.extents.x;
				}
				else
				{
					target = 1.0f;
					position.x -= _collider.bounds.extents.x;
				}
			}

			IsMeleeAttacking = true;
			Events.OnMeleeAttackBegin.InvokeSafe();

			_meleeAttack.transform.position = position;
			_meleeAttack.SetActive(true);

			float duration = 0.0f;
			float t = 0.0f;

			while( t <= 1.0f )
			{
				_meleeAttack.transform.localScale = new Vector3(Mathf.Lerp(0.0f, target, t), 1.0f, 1.0f);
				duration += Time.deltaTime;
				t = duration/ Config.MeleeAttackSpeed;

				yield return null;
			}

			_meleeAttack.SetActive(false);
			IsMeleeAttacking = false;
			_meleeAttackEndTime = Time.time;

			Events.OnMeleeAttackEnd.InvokeSafe();
		}

		private IEnumerator RangedAttack()
		{
			IsRangedAttacking = true;
			Events.OnRangedAttackBegin.InvokeSafe();

			Projectile projectile = _rangedAttackPool.GetInstance<Projectile>(null);

			Vector3 position = _collider.bounds.center;
			Vector3 direction = Vector3.zero;

			float vertical = CharacterYoke.GetAxis(InputAction.MoveVertical);
			if (vertical != 0.0f)
			{
				if (vertical < 0.0f)
				{
					//Trying to hit down
					//If we're on the ground, we can't slash down
					if (IsGrounded)
					{
						yield break;
					}

					position.y -= _collider.bounds.extents.y;
					direction = Vector3.down;
				}
				else
				{
					position.y += _collider.bounds.extents.y;
					direction = Vector3.up;
				}
			}
			else
			{
				if (FacingDirection == Facing.Right)
				{
					direction = Vector3.right;
					position.x += _collider.bounds.extents.x;
				}
				else
				{
					direction = Vector3.left;
					position.x -= _collider.bounds.extents.x;
				}
			}

			projectile.transform.position = position;
			projectile.Setup(this, Config.BaseRangedDamage, direction * Config.ProjectileSpeed, _rangedAttackPool);
			projectile.OnHit += OnRangedHit;
			projectile.OnReturnToPool += OnProjectileReturnToPool;
			yield return new WaitForSeconds(Config.RangedAttackSpeed);

			IsRangedAttacking = false;
			Events.OnRangedAttackEnd.InvokeSafe();
		}

		private IEnumerator DashAttack()
		{
			IsDashAttacking = true;
			Events.OnDashAttackBegin.InvokeSafe();
			_collideWithEnemies = false;

			_dashAttackHighlightSprite.color = Config.DashCooldownHighlight;
			_dashAttack.SetActive(true);
			_dashAttackCollider.enabled = true;

			Vector3 velocity = Vector3.zero;
			if (FacingDirection == Facing.Right)
			{
				velocity = Vector3.right;
			}
			else
			{
				velocity = Vector3.left;
			}

			float speed = Config.DashAttackDistance / Config.DashAttackDuration;
			velocity *= speed;
			float startTime = Time.time;

			while (Time.time - startTime <= Config.DashAttackDuration)
			{
				_velocity = MoveBy(_velocity, velocity * Time.deltaTime);
				yield return null;
			}

			_dashAttackEndTime = Time.time;
			StartCoroutine(DashCooldownHighlight());

			_collideWithEnemies = true;
			_dashAttackCollider.enabled = false;
			IsDashAttacking = false;
			Events.OnDashAttackEnd.InvokeSafe();
		}

		private IEnumerator DashCooldownHighlight()
		{
			Color targetColour = Color.white;
			_dashAttackHighlightSprite.color = Config.DashCooldownHighlight;

			float t = 0.0f;
			float perc = 0.0f;
			while (perc < 1.0f)
			{
				_dashAttackHighlightSprite.color = Color.Lerp(Config.DashCooldownHighlight, targetColour, perc);
				t += Time.deltaTime;
				perc = t / Config.DashAttackCooldown;
				yield return null;
			}

			_dashAttackHighlightSprite.color = Color.white;
			_dashAttack.SetActive(false);
		}

		/// <summary>
		/// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
		/// stop when run into.
		/// </summary>
		/// <param name="deltaMovement">Delta movement.</param>
		private Vector3 MoveBy(Vector3 velocity, Vector3 deltaMovement)
		{
			// save off our current grounded state which we will use for wasGroundedLastFrame and becameGroundedThisFrame
			_collisionState.WasGroundedLastFrame = _collisionState.Below;

			// clear our state
			_collisionState.Reset();
			_raycastHitsThisFrame.Clear();
			_isGoingUpSlope = false;

			PrimeRaycastOrigins();

			// first, we check for a slope below us before moving
			// only check slopes if we are going down and grounded
			if (deltaMovement.y < 0f && _collisionState.WasGroundedLastFrame)
				HandleVerticalSlope(ref deltaMovement);

			// now we check movement in the horizontal dir
			if (deltaMovement.x != 0f)
				MoveHorizontally(ref deltaMovement);

			// next, check movement in the vertical dir
			if (deltaMovement.y != 0f)
				MoveVertically(ref deltaMovement);

			// move then update our state
			deltaMovement.z = 0;
			transform.Translate(deltaMovement, Space.World);

			// only calculate velocity if we have a non-zero deltaTime
			if (Time.deltaTime > 0f)
				velocity = deltaMovement / Time.deltaTime;

			// set our becameGrounded state based on the previous and current collision state
			if (!_collisionState.WasGroundedLastFrame && _collisionState.Below)
			{
				_collisionState.BecameGroundedThisFrame = true;
				_startedFalling = 0.0f;
			}

			//Check if we started falling this frame
			if(_collisionState.WasGroundedLastFrame && !_collisionState.Below)
			{
				_startedFalling = Time.time;
			}

			// if we are going up a slope we artificially set a y velocity so we need to zero it out here
			if (_isGoingUpSlope)
				velocity.y = 0;

			// send off the collision events if we have a listener
			if (Events.OnControllerCollidedEvent != null)
			{
				for (int i = 0; i < _raycastHitsThisFrame.Count; i++)
				{
					Events.OnControllerCollidedEvent(_raycastHitsThisFrame[i]);
				}
			}

			IgnoreOneWayPlatformsThisFrame = false;

			return velocity;
		}

		/// <summary>
		/// moves directly down until grounded
		/// </summary>
		private void WarpToGrounded()
		{
			do
			{
				MoveBy(_velocity, new Vector3(0, -1f, 0));
			} while (!IsGrounded);
		}

		/// <summary>
		/// this should be called anytime you have to modify the BoxCollider2D at runtime. It will recalculate the distance between the rays used for collision detection.
		/// It is also used in the skinWidth setter in case it is changed at runtime.
		/// </summary>
		private void RecalculateDistanceBetweenRays()
		{
			// figure out the distance between our rays in both directions
			// horizontal
			float colliderUseableHeight = (_collider.bounds.size.y * Mathf.Abs(transform.localScale.y)) - (2f * SkinWidth);
			_verticalDistanceBetweenRays = colliderUseableHeight / (_totalHorizontalRays - 1);

			// vertical
			float colliderUseableWidth = (_collider.bounds.size.x * Mathf.Abs(transform.localScale.x)) - (2f * SkinWidth);
			_horizontalDistanceBetweenRays = colliderUseableWidth / (_totalVerticalRays - 1);
		}

		/// <summary>
		/// resets the raycastOrigins to the current extents of the box collider inset by the skinWidth. It is inset
		/// to avoid casting a ray from a position directly touching another collider which results in wonky normal data.
		/// </summary>
		/// <param name="futurePosition">Future position.</param>
		/// <param name="deltaMovement">Delta movement.</param>
		private void PrimeRaycastOrigins()
		{
			// our raycasts need to be fired from the bounds inset by the skinWidth
			Bounds modifiedBounds = _collider.bounds;
			modifiedBounds.Expand(-2f * SkinWidth);

			_raycastOrigins.TopLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y);
			_raycastOrigins.BottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y);
			_raycastOrigins.BottomLeft = modifiedBounds.min;
		}

		/// <summary>
		/// we have to use a bit of trickery in this one. The rays must be cast from a small distance inside of our
		/// collider (skinWidth) to avoid zero distance rays which will get the wrong normal. Because of this small offset
		/// we have to increase the ray distance skinWidth then remember to remove skinWidth from deltaMovement before
		/// actually moving the player
		/// </summary>
		private void MoveHorizontally(ref Vector3 deltaMovement)
		{
			var isGoingRight = deltaMovement.x > 0;
			var rayDistance = Mathf.Abs(deltaMovement.x) + _skinWidth;
			var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
			var initialRayOrigin = isGoingRight ? _raycastOrigins.BottomRight : _raycastOrigins.BottomLeft;

			for (var i = 0; i < _totalHorizontalRays; i++)
			{
				var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + (i * _verticalDistanceBetweenRays));

				DrawRay(ray, rayDirection * rayDistance, Color.red);

				// if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
				// walk up sloped oneWayPlatforms
				if (i == 0 && _collisionState.WasGroundedLastFrame)
				{
					_raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, _config.PlatformMaskAndOneWay | EnemyMask);
				}
				else
				{
					_raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, _config.PlatformMask | EnemyMask);
				}

				if (_raycastHit)
				{
					// the bottom ray can hit a slope but no other ray can so we have special handling for these cases
					if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(_raycastHit.normal, Vector2.up)))
					{
						_raycastHitsThisFrame.Add(_raycastHit);
						// if we weren't grounded last frame, that means we're landing on a slope horizontally.
						// this ensures that we stay flush to that slope
						if (!_collisionState.WasGroundedLastFrame)
						{
							float flushDistance = Mathf.Sign(deltaMovement.x) * (_raycastHit.distance - SkinWidth);
							transform.Translate(new Vector2(flushDistance, 0));
						}
						break;
					}

					// set our new deltaMovement and recalculate the rayDistance taking it into account
					deltaMovement.x = _raycastHit.point.x - ray.x;
					rayDistance = Mathf.Abs(deltaMovement.x);

					// remember to remove the skinWidth from our deltaMovement
					if (isGoingRight)
					{
						deltaMovement.x -= _skinWidth;
						_collisionState.Right = true;
					}
					else
					{
						deltaMovement.x += _skinWidth;
						_collisionState.Left = true;
					}

					_raycastHitsThisFrame.Add(_raycastHit);

					// we add a small fudge factor for the float operations here. if our rayDistance is smaller
					// than the width + fudge bail out because we have a direct impact
					if (rayDistance < _skinWidth + kSkinWidthFloatFudgeFactor)
						break;
				}
			}
		}

		/// <summary>
		/// handles adjusting deltaMovement if we are going up a slope.
		/// </summary>
		/// <returns><c>true</c>, if horizontal slope was handled, <c>false</c> otherwise.</returns>
		/// <param name="deltaMovement">Delta movement.</param>
		/// <param name="angle">Angle.</param>
		private bool HandleHorizontalSlope(ref Vector3 deltaMovement, float angle)
		{
			// disregard 90 degree angles (walls)
			if (Mathf.RoundToInt(angle) == 90)
				return false;

			// if we can walk on slopes and our angle is small enough we need to move up
			if (angle < _slopeLimit)
			{
				// we only need to adjust the deltaMovement if we are not jumping
				// TODO: this uses a magic number which isn't ideal! The alternative is to have the user pass in if there is a jump this frame
				if (deltaMovement.y < _jumpingThreshold)
				{
					// apply the slopeModifier to slow our movement up the slope
					var slopeModifier = _slopeSpeedMultiplier.Evaluate(angle);
					deltaMovement.x *= slopeModifier;

					// we dont set collisions on the sides for this since a slope is not technically a side collision.
					// smooth y movement when we climb. we make the y movement equivalent to the actual y location that corresponds
					// to our new x location using our good friend Pythagoras
					deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
					var isGoingRight = deltaMovement.x > 0;

					// safety check. we fire a ray in the direction of movement just in case the diagonal we calculated above ends up
					// going through a wall. if the ray hits, we back off the horizontal movement to stay in bounds.
					var ray = isGoingRight ? _raycastOrigins.BottomRight : _raycastOrigins.BottomLeft;
					RaycastHit2D raycastHit;
					if (_collisionState.WasGroundedLastFrame)
						raycastHit = Physics2D.Raycast(ray, deltaMovement.normalized, deltaMovement.magnitude, _config.PlatformMaskAndOneWay | EnemyMask);
					else
						raycastHit = Physics2D.Raycast(ray, deltaMovement.normalized, deltaMovement.magnitude, _config.PlatformMask | EnemyMask);

					if (raycastHit)
					{
						// we crossed an edge when using Pythagoras calculation, so we set the actual delta movement to the ray hit location
						deltaMovement = (Vector3)raycastHit.point - ray;
						if (isGoingRight)
							deltaMovement.x -= _skinWidth;
						else
							deltaMovement.x += _skinWidth;
					}

					_isGoingUpSlope = true;
					_collisionState.Below = true;
					_collisionState.SlopeAngle = -angle;
				}
			}
			else // too steep. get out of here
			{
				deltaMovement.x = 0;
			}

			return true;
		}

		private void MoveVertically(ref Vector3 deltaMovement)
		{
			var isGoingUp = deltaMovement.y > 0;
			var rayDistance = Mathf.Abs(deltaMovement.y) + _skinWidth;
			var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
			var initialRayOrigin = isGoingUp ? _raycastOrigins.TopLeft : _raycastOrigins.BottomLeft;

			// apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
			initialRayOrigin.x += deltaMovement.x;

			// if we are moving up, we should ignore the layers in oneWayPlatformMask
			LayerMask mask = 0;
			if ((isGoingUp && !_collisionState.WasGroundedLastFrame) || IgnoreOneWayPlatformsThisFrame)
			{
				mask = _config.PlatformMask | EnemyMask;
			}
			else
			{
				mask = _config.PlatformMaskAndOneWay | EnemyMask;
			}

			for (var i = 0; i < _totalVerticalRays; i++)
			{
				var ray = new Vector2(initialRayOrigin.x + (i * _horizontalDistanceBetweenRays), initialRayOrigin.y);

				DrawRay(ray, rayDirection * rayDistance, Color.red);
				_raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);
				if (_raycastHit)
				{
					// set our new deltaMovement and recalculate the rayDistance taking it into account
					deltaMovement.y = _raycastHit.point.y - ray.y;
					rayDistance = Mathf.Abs(deltaMovement.y);

					// remember to remove the skinWidth from our deltaMovement
					if (isGoingUp)
					{
						deltaMovement.y -= _skinWidth;
						_collisionState.Above = true;
					}
					else
					{
						deltaMovement.y += _skinWidth;
						_collisionState.Below = true;
					}

					_raycastHitsThisFrame.Add(_raycastHit);

					// this is a hack to deal with the top of slopes. if we walk up a slope and reach the apex we can get in a situation
					// where our ray gets a hit that is less then skinWidth causing us to be ungrounded the next frame due to residual velocity.
					if (!isGoingUp && deltaMovement.y > 0.00001f)
						_isGoingUpSlope = true;

					// we add a small fudge factor for the float operations here. if our rayDistance is smaller
					// than the width + fudge bail out because we have a direct impact
					if (rayDistance < _skinWidth + kSkinWidthFloatFudgeFactor)
						break;
				}
			}
		}

		public virtual void UnlockAbility(Inventory.Ability ability)
		{
			switch (ability)
			{
				case Inventory.Ability.DoubleJump:
					MaxJumps = 2;
					break;
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// checks the center point under the BoxCollider2D for a slope. If it finds one then the deltaMovement is adjusted so that
		/// the player stays grounded and the slopeSpeedModifier is taken into account to speed up movement.
		/// </summary>
		/// <param name="deltaMovement">Delta movement.</param>
		private void HandleVerticalSlope(ref Vector3 deltaMovement)
		{
			// slope check from the center of our collider
			var centerOfCollider = (_raycastOrigins.BottomLeft.x + _raycastOrigins.BottomRight.x) * 0.5f;
			var rayDirection = -Vector2.up;

			// the ray distance is based on our slopeLimit
			var slopeCheckRayDistance = _slopeLimitTangent * (_raycastOrigins.BottomRight.x - centerOfCollider);

			var slopeRay = new Vector2(centerOfCollider, _raycastOrigins.BottomLeft.y);
			DrawRay(slopeRay, rayDirection * slopeCheckRayDistance, Color.yellow);

			_raycastHit = Physics2D.Raycast(slopeRay, rayDirection, slopeCheckRayDistance, _config.PlatformMaskAndOneWay);

			if (_raycastHit)
			{
				// bail out if we have no slope
				var angle = Vector2.Angle(_raycastHit.normal, Vector2.up);
				if (angle == 0)
					return;

				// we are moving down the slope if our normal and movement direction are in the same x direction
				var isMovingDownSlope = Mathf.Sign(_raycastHit.normal.x) == Mathf.Sign(deltaMovement.x);
				if (isMovingDownSlope)
				{
					// going down we want to speed up in most cases so the slopeSpeedMultiplier curve should be > 1 for negative angles
					var slopeModifier = _slopeSpeedMultiplier.Evaluate(-angle);
					// we add the extra downward movement here to ensure we "stick" to the surface below
					deltaMovement.y += _raycastHit.point.y - slopeRay.y - SkinWidth;
					deltaMovement = new Vector3(0, deltaMovement.y, 0) +
									(Quaternion.AngleAxis(-angle, Vector3.forward) * new Vector3(deltaMovement.x * slopeModifier, 0, 0));
					_collisionState.MovingDownSlope = true;
					_collisionState.SlopeAngle = angle;
				}
			}
		}

		public void AddEnemy(CharacterFaction faction)
		{
			if(faction == Config.Faction)
			{
				return;
			}

			switch (faction)
			{
				case CharacterFaction.Player:
					_enemyMask |= Layers.PlayerMask;
					break;
				case CharacterFaction.Square:
					_enemyMask |= Layers.SquareMask;
					break;
				case CharacterFaction.Triangle:
					_enemyMask |= Layers.TriangleMask;
					break;
				case CharacterFaction.Circle:
					_enemyMask |= Layers.CircleMask;
					break;
			}
		}

		public void RemoveEnemy(CharacterFaction faction)
		{
			if (faction == Config.Faction)
			{
				return;
			}

			switch (faction)
			{
				case CharacterFaction.Player:
					_enemyMask &= ~Layers.PlayerMask;
					break;
				case CharacterFaction.Square:
					_enemyMask &= ~Layers.SquareMask;
					break;
				case CharacterFaction.Triangle:
					_enemyMask &= ~Layers.TriangleMask;
					break;
				case CharacterFaction.Circle:
					_enemyMask &= ~Layers.CircleMask;
					break;
			}
		}

		private void UpdateLayerCollision()
		{
			// we want to set our CC2D to ignore all collision layers except what is in our triggerMask
			for (int i = 0; i < 32; i++)
			{
				// see if our triggerMask contains this layer and if not ignore it
				if ((Config.TriggerMask.value & 1 << i) == 0)
				{
					Physics2D.IgnoreLayerCollision(gameObject.layer, i);
				}
			}
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			Events.OnTriggerEnterEvent.InvokeSafe(col);
		}

		private void OnTriggerStay2D(Collider2D col)
		{
			Events.OnTriggerStayEvent.InvokeSafe(col);
		}

		private void OnTriggerExit2D(Collider2D col)
		{
			Events.OnTriggerExitEvent.InvokeSafe(col);
		}

		private void OnMeleeHit(BaseCharacterController hit)
		{
			Events.OnMeleeAttackConnect.InvokeSafe(hit);
		}

		private void OnRangedHit(BaseCharacterController hit)
		{
			Events.OnRangedAttackConnect.InvokeSafe(hit);
		}

		private void OnProjectileReturnToPool(Projectile projectile)
		{
			projectile.OnHit -= OnRangedHit;
			projectile.OnReturnToPool -= OnProjectileReturnToPool;
		}

		private void OnDashHit(BaseCharacterController obj)
		{
			Events.OnDashAttackConnect.InvokeSafe(obj);
		}

		[System.Diagnostics.Conditional("DEBUG_CC2D_RAYS")]
		private void DrawRay(Vector3 start, Vector3 dir, Color color)
		{
			Debug.DrawRay(start, dir, color);
		}
	}
}