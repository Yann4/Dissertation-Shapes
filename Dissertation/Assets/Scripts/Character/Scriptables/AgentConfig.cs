using UnityEngine;

namespace Dissertation.Character.AI
{
	[CreateAssetMenu(fileName = "AgentConfig.asset", menuName = "Dissertation/Scriptables/Character Config/Agent Config")]
	public class AgentConfig : CharacterConfig
	{
		[SerializeField] private States _defaultState = 0;
		public States DefaultState { get { return _defaultState; } }

		[SerializeField] private float _visionRange = 20.0f;
		public float VisionRange { get { return _visionRange; } }
	}
}