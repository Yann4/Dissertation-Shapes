using UnityEngine;
using Dissertation.NodeGraph;

namespace Dissertation.Character
{
	public class Conversation : MonoBehaviour
	{
		[SerializeField] private TextAsset _conversation;

		private ConversationFragment _currentFragment;

		private void Start()
		{
			ConversationNode startNode = NodeUtils.LoadGraph(_conversation.bytes, NodeUtils.CreateNode<ConversationNode>) as ConversationNode;
			_currentFragment = startNode.Sentence;
		}
	}
}