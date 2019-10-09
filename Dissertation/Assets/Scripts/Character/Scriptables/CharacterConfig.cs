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

	[SerializeField] private bool _canDoubleJump;
	public bool CanDoubleJump { get { return _canDoubleJump; } }

	[Header("Character attributes")]
	[SerializeField] private int _maxHealth = 5;
	public int MaxHealth { get { return _maxHealth; } }

	[Header("Character inventory")]
	[SerializeField] private Inventory.InventoryContents _defaultContents;
	public Inventory.InventoryContents DefaultContents { get { return _defaultContents; } }

	[SerializeField] private GameObject _dropInventoryPrefab;
	public GameObject DropInventoryPrefab { get { return _dropInventoryPrefab; } }
}
