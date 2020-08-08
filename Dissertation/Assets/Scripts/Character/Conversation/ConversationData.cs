using Dissertation.NodeGraph;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character
{
	[CreateAssetMenu(fileName = "Conversation Data.asset", menuName = "Dissertation/Scriptables/Conversation Data")]
	public class ConversationData : ScriptableObject
	{
		[Serializable]
		public class DataMap
		{
			public TextAsset Asset;

			[NonSerialized] public ConversationFragment ConversationStart;
		}

		[SerializeField] private List<DataMap> Conversations = new List<DataMap>();

		public void Setup()
		{
			for(int idx = 0; idx < Conversations.Count; idx++)
			{
				ConversationNode node = NodeUtils.LoadGraph(Conversations[idx].Asset.bytes, NodeUtils.CreateNode<ConversationNode>) as ConversationNode;
				Conversations[idx].ConversationStart = node.Sentence;
			}
		}

		public ConversationFragment GetOrLoadConversation(TextAsset conversationAsset)
		{
			ConversationFragment conversation = GetConversation(conversationAsset.name);
			if (conversation == null)
			{
				ConversationNode node = NodeUtils.LoadGraph(conversationAsset.bytes, NodeUtils.CreateNode<ConversationNode>) as ConversationNode;
				Conversations.Add(new DataMap() { ConversationStart = node.Sentence, Asset = conversationAsset });

				conversation = GetConversation(conversationAsset.name);
			}

			return conversation;
		}

		public ConversationFragment GetConversation(string assetName)
		{
			foreach(DataMap map in Conversations)
			{
				if(map.Asset.name == assetName)
				{
					return map.ConversationStart;
				}
			}

			return null;
		}

		public bool IsPlayerConversation(ConversationFragment conversation)
		{
			if(conversation.IsPlayer)
			{
				return true;
			}

			if(conversation.NextFragments.Length == 0)
			{
				return false;
			}

			bool isPlayerConversation = false;
			foreach(ConversationFragment fragment in conversation.NextFragments)
			{
				isPlayerConversation |= IsPlayerConversation(fragment);
				if(isPlayerConversation)
				{
					break;
				}
			}

			return isPlayerConversation;
		}
	}
}