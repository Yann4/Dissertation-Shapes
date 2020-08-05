using System.Collections;
using UnityEngine;
using Dissertation.Character.AI;
using Dissertation.UI;
using Dissertation.Util.Localisation;
using System;
using Dissertation.Util;
using System.Collections.Generic;
using Dissertation.Character.Player;
using Dissertation.Input;

namespace Dissertation.Character
{
	public class Conversation : MonoBehaviour
	{
		[SerializeField] private AgentController _owner;
		[SerializeField] private BoxCollider2D _conversationTrigger;

		public bool IsInConversation { get; private set; } = false;

		private ConversationPrompt _prompt;

		private ConversationFragment _currentFragment;
		private Stack<string> _availableConversations = new Stack<string>();

		private bool _dialogueClosed = false;
		private int _optionSelected = 0;

		private List<BaseCharacterController> _potentialParticipants = new List<BaseCharacterController>();
		private BaseCharacterController _talkingTo = null;

		public static Action<ConversationFragment, AgentController> ConversationStarted;
		public static Action<AgentController> ConversationEnded;

		private void Start()
		{
			SetupConversations(_owner._agentConfig.AvailableConversations);

			_prompt = HUD.Instance.CreateMenu<ConversationPrompt>();
			_prompt.Setup(_owner);
			_prompt.SetVisible(false);
		}

		private void OnDestroy()
		{
			if(_prompt != null)
			{
				HUD.Instance.DestroyMenu(_prompt);
			}
		}

		private void Update()
		{
			if (IsConversationAvailable())
			{
				foreach (BaseCharacterController character in _potentialParticipants)
				{
					if (character.CharacterYoke.GetButtonDown(InputAction.Interact))
					{
						TryStartConversation(character);
					}
				}
			}
		}

		//Sets up stack of conversations
		private void SetupConversations( TextAsset[] conversationReferences )
		{
			foreach(TextAsset reference in conversationReferences)
			{
				ConversationFragment conversation = App.AIBlackboard.GetConversation(reference);
				if (conversation != null)
				{
					_availableConversations.Push(reference.name);
				}
				else
				{
					Debug.LogError("Couldn't find conversation with reference " + reference);
				}
			}
		}

		//Starts specific conversation
		public void StartConversation( string conversationReference, BaseCharacterController listener )
		{
			_currentFragment = App.AIBlackboard.GetConversation(conversationReference);
			_talkingTo = listener;

			StartCoroutine(RunConversation());
		}

		//Starts next conversation off stack
		public void TryStartConversation(BaseCharacterController other)
		{
			if ( IsInConversation || !IsConversationAvailable() || !_conversationTrigger.bounds.Contains(other.transform.position) )
			{
				return;
			}

			StartConversation(_availableConversations.Pop(), other);
		}

		private IEnumerator RunConversation()
		{
			SetPromptVisible( false );
			IsInConversation = true;
			ConversationStarted.InvokeSafe(_currentFragment, _owner);

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

			_talkingTo = null;
			IsInConversation = false;
			ConversationEnded.InvokeSafe(_owner);

			if (_conversationTrigger.bounds.Contains(App.AIBlackboard.Player.transform.position))
			{
				SetPromptVisible(true);
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

			ConversationFunctionLibrary.RunFunction(_currentFragment.Output, _currentFragment.OptionOutputData[_optionSelected], _owner, _talkingTo);

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

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (IsConversationAvailable())
			{
				BaseCharacterController character = collision.gameObject.GetComponent<BaseCharacterController>();

				if (character != null)
				{
					if (character.GetType() == typeof(PlayerController))
					{
						SetPromptVisible(true);
					}

					_potentialParticipants.Add(character);
				}
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (IsConversationAvailable())
			{
				BaseCharacterController character = collision.gameObject.GetComponent<BaseCharacterController>();

				if (character != null)
				{
					if (character.GetType() == typeof(PlayerController))
					{
						SetPromptVisible(false);
					}

					_potentialParticipants.Remove(character);
				}
			}
		}

		private void SetPromptVisible(bool visible)
		{
			_prompt.SetVisible( visible && IsConversationAvailable() );
		}

		private bool IsConversationAvailable()
		{
			return _availableConversations.Count > 0 && ConversationFunctionLibrary.IsAvailable(App.AIBlackboard.GetConversation(_availableConversations.Peek()).IsAvailable, _owner);
		}
	}
}