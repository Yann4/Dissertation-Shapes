using Dissertation.Character.AI;

namespace Dissertation.UI
{
	public class ConversationPrompt : InWorldMenu
	{
		public void Setup(AgentController owner)
		{
			TrackObject(owner.transform);
		}
	}
}