using UnityEngine;

namespace Dissertation.Character.Player
{
	[CreateAssetMenu(fileName = "PlayerConfig.asset", menuName = "Dissertation/Scriptables/Character Config/Player Config")]
	public class PlayerConfig : CharacterConfig
	{
		[SerializeField] private CharacterFaction _defaultShape = CharacterFaction.Square;
		public CharacterFaction DefaultShape { get { return _defaultShape; } }

		[SerializeField] private float _transformDuration = 1.0f;
		public float TransformDuration { get { return _transformDuration; } }
	}
}