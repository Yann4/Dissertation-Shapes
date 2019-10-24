using Dissertation.Character;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterConfig.asset", menuName = "Dissertation/Scriptables/Character Config/Character Config")]
public class CharacterConfig : ScriptableObject
{
	/// <summary>
	/// mask with all layers that the player should interact with
	/// </summary>
	[SerializeField] private LayerMask _platformMask = 0;
	public LayerMask PlatformMask { get { return _platformMask; } }

	/// <summary>
	/// mask with all layers that should act as one-way platforms. Note that one-way platforms should always be EdgeCollider2Ds. This is because it does not support being
	/// updated anytime outside of the inspector for now.
	/// </summary>
	[SerializeField]
	private LayerMask _oneWayPlatformMask = 0;
	public LayerMask OneWayPlatformMask { get { return _oneWayPlatformMask; } }
	public LayerMask PlatformMaskAndOneWay { get { return _platformMask | OneWayPlatformMask; } }

	/// <summary>
	/// mask with all layers that trigger events should fire when intersected
	/// </summary>
	[SerializeField] private LayerMask _triggerMask = 0;
	public LayerMask TriggerMask { get { return _triggerMask; } }

	[Header("Movement config")]
	[SerializeField] private float _gravity = -25f;
	public float Gravity { get { return _gravity; } }

	[SerializeField] private float _runSpeed = 8f;
	public float RunSpeed { get { return _runSpeed; } }

	[SerializeField] private float _jumpHeight = 3f;
	public float JumpHeight { get { return _jumpHeight; } }

	[SerializeField] private AnimationCurve _jumpSpeed;
	public float GetJumpSpeed(float t) { return _jumpSpeed.Evaluate(t); }

	[SerializeField] private int _maxJumps;
	public int MaxJumps { get { return _maxJumps; } }

	[Header("Character attributes")]
	[SerializeField] private CharacterFaction _faction;
	public CharacterFaction Faction { get { return _faction; } }

	[SerializeField] private int _maxHealth = 5;
	public int MaxHealth { get { return _maxHealth; } }

	[Header("Melee Attack")]
	[SerializeField] private int _baseMeleeDamage = 2;
	public int BaseMeleeDamage { get { return _baseMeleeDamage; } }

	[SerializeField, Tooltip("Time to complete one attack")] private float _meleeAttackSpeed = 0.3f;
	public float MeleeAttackSpeed { get { return _meleeAttackSpeed; } }

	[SerializeField, Tooltip("Delay between attacks")] private float _meleeAttackCooldown = 0.75f;
	public float MeleeAttackCooldown { get { return _meleeAttackCooldown; } }

	[Header("Ranged Attack")]
	[SerializeField] private int _baseRangedDamage = 11;
	public int BaseRangedDamage { get { return _baseRangedDamage; } }

	[SerializeField, Tooltip("Speed projectile travels")] private float _projectileSpeed = 13.0f;
	public float ProjectileSpeed { get { return _projectileSpeed; } }

	[SerializeField, Tooltip("Time to complete one attack")] private float _rangedAttackSpeed = 0.1f;
	public float RangedAttackSpeed { get { return _rangedAttackSpeed; } }

	[SerializeField, Tooltip("Delay between attacks")] private float _rangedAttackCooldown = 0.75f;
	public float RangedAttackCooldown { get { return _rangedAttackCooldown; } }

	[Header("Dash Attack")]
	[SerializeField] private float _dashAttackDistance = 10.0f;
	public float DashAttackDistance { get { return _dashAttackDistance; } }

	[SerializeField] private float _dashAttackDuration = 1.0f;
	public float DashAttackDuration { get { return _dashAttackDuration; } }

	[SerializeField] private float _dashAttackCooldown = 1.0f;
	public float DashAttackCooldown { get { return _dashAttackCooldown; } }

	[SerializeField] private int _dashAttackBaseDamage = 3;
	public int DashAttackBaseDamage { get { return _dashAttackBaseDamage; } }

	[SerializeField] private Color _dashCooldownHighlight = Color.black;
	public Color DashCooldownHighlight { get { return _dashCooldownHighlight; } }

	[Header("Character inventory")]
	[SerializeField] private Inventory.InventoryContents _defaultContents;
	public Inventory.InventoryContents DefaultContents { get { return _defaultContents.Copy(); } }

	[SerializeField] private GameObject _dropInventoryPrefab;
	public GameObject DropInventoryPrefab { get { return _dropInventoryPrefab; } }

	[Header("Attack prefabs")]
	[SerializeField] private GameObject _meleeAttackPrefab;
	public GameObject MeleeAttackPrefab { get { return _meleeAttackPrefab; } }

	[SerializeField] private GameObject _rangedAttackPrefab;
	public GameObject RangedAttackPrefab { get { return _rangedAttackPrefab; } }
}
