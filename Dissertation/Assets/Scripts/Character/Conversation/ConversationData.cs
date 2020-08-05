using Dissertation.NodeGraph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character
{
	[CreateAssetMenu(fileName = "Conversation Data.asset", menuName = "Dissertation/Scriptables/Conversation Data")]
	public class ConversationData : ScriptableObject
	{
		[Serializable]
		public struct DataMap
		{
			public TextAsset Asset;

			[NonSerialized] public ConversationFragment ConversationStart;
		}

		[SerializeField] private DataMap[] Conversations;

		public void Setup()
		{
			for(int idx = 0; idx < Conversations.Length; idx++)
			{
				ConversationNode node = NodeUtils.LoadGraph(Conversations[idx].Asset.bytes, NodeUtils.CreateNode<ConversationNode>) as ConversationNode;
				Conversations[idx].ConversationStart = node.Sentence;
			}
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