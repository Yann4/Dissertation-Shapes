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

		[Header("Dialogue")]
		[SerializeField] public string[] AvailableConversations = new string[0];
	}
}