using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character.AI
{
	[CreateAssetMenu(fileName = "AgentConfig.asset", menuName = "Dissertation/Scriptables/Character Config/Agent Config")]
	public class AgentConfig : CharacterConfig
	{
		[SerializeField] private States _defaultState = 0;
		public States DefaultState { get { return _defaultState; } }

		[SerializeField] private List<Trait> _traits = new List<Trait>();
		public IEnumerable<Trait> Traits { get { return _traits; } }

		[SerializeField] private float _visionRange = 20.0f;
		public float VisionRange { get { return _visionRange; } }

		[SerializeField] private float _fleeHealthPercentage = 0.2f;
		public float FleeHealthPercentage { get { return _fleeHealthPercentage; } }

		[Header("Dialogue")]
		[SerializeField] public TextAsset[] AvailableConversations = new TextAsset[0];
	}
}