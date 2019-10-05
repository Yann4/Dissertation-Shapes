using UnityEngine;

namespace Dissertation.Character.AI
{
	[CreateAssetMenu(fileName = "AgentConfig.asset", menuName = "Dissertation/Scriptables/Agent Config")]
	public class AgentConfig : CharacterConfig
	{
		[SerializeField] private States _defaultState = 0;
		public States DefaultState { get { return _defaultState; } }
	}
}