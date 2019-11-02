using System.Collections;
using UnityEngine;
using Dissertation.NodeGraph;
using Dissertation.Character.AI;
using Dissertation.UI;
using Dissertation.Util.Localisation;

namespace Dissertation.Character
{
	public class Conversation : MonoBehaviour
	{
		[SerializeField] private TextAsset _conversation;
		[SerializeField] private AgentController _owner;

		private ConversationFragment _currentFragment;

		private bool _dialogueClosed = false;
		private int _optionSelected = 0;

		private void Start()
		{
			ConversationNode startNode = NodeUtils.LoadGraph(_conversation.bytes, NodeUtils.CreateNode<ConversationNode>) as ConversationNode;
			_currentFragment = startNode.Sentence;

			StartCoroutine(RunConversation());
		}

		private IEnumerator RunConversation()
		{
			while(_currentFragment != null)
			{
				if(_currentFragment.IsPlayer)
				{
					yield return ShowPlayerSpeech(_currentFragment.ToSay[0], _currentFragment.ToSay[1]);
				}
				else
				{
					yield return ShowSpeech(_currentFragment.ToSay[0], false);
				}
			}
		}

		private IEnumerator ShowSpeech(string locstring, bool isPlayer)
		{
			_optionSelected = 0;

			Transform toTrack = isPlayer ? App.AIBlackboard.Player.transform : _owner.transform;

			_dialogueClosed = false;

			SpeechBubble dialogue = HUD.Instance.CreateMenu<SpeechBubble>();
			dialogue.OnClose += OnDialogueClosed;
			dialogue.Show(toTrack, LocManager.GetTranslation(locstring), PlayerPressedSkip);

			yield return new WaitUntil(() => _dialogueClosed);

			dialogue.OnClose -= OnDialogueClosed;
		}

		private IEnumerator ShowPlayerSpeech(string option1, string option2)
		{
			if (string.IsNullOrEmpty(option2))
			{
				yield return ShowSpeech(option1, true);
			}
			else
			{
				_dialogueClosed = false;

				DialogueBox dialogue = HUD.Instance.FindMenu<DialogueBox>();
				dialogue.Show("", "", LocManager.GetTranslation(option1), LocManager.GetTranslation(option2), Option1Selected, Option2Selected);

				yield return new WaitUntil(() => _dialogueClosed);
			}
		}

		private void OnDialogueClosed()
		{
			_dialogueClosed = true;

			if (_currentFragment.NextFragments.Length > 0)
			{
				_currentFragment = _currentFragment.NextFragments[_optionSelected];
			}
			else
			{
				_currentFragment = null;
			}
		}

		private void Option1Selected()
		{
			_optionSelected = 0;
			OnDialogueClosed();
		}

		private void Option2Selected()
		{
			_optionSelected = 1;
			OnDialogueClosed();
		}

		private bool PlayerPressedSkip()
		{
			return false;
		}
	}
}