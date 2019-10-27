using Dissertation.Character;
using Dissertation.Character.AI;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Dissertation.UI
{
	public class AgentDebugUI : InWorldMenu
	{
		[SerializeField] private Text _longTerm;
		[SerializeField] private Text _normal;
		[SerializeField] private Text _immediate;

		[SerializeField] private Text _hostility;

		[SerializeField] private Text _health;

		private AgentController _owner;

		private const string _hostileText = "<color=red>Hostile to player</color>";
		private const string _friendlyText = "<color=green>Friendly to player</color>";
		private const string _healthText = "<color=green>Current health {0}/{1}</color>";
		private const string _deadText = "<color=red>Dead</color>";

		public void Setup(AgentController owner)
		{
			Debug.Assert(_longTerm != null);
			Debug.Assert(_normal != null);
			Debug.Assert(_immediate != null);

			_owner = owner;
			TrackObject(_owner.transform);

			RefreshText();
		}

		protected override void Update()
		{
			base.Update();

			RefreshText();

			foreach(BaseCharacterController character in App.AIBlackboard.GetVisibleCharacters(_owner))
			{
				Debug.DrawLine(_owner.transform.position, character.transform.position, App.AIBlackboard.CharactersAreHostile(_owner, character) ? Color.red : Color.green, Time.deltaTime);
			}
		}

		private void RefreshText()
		{
			_longTerm.text = ConstructString("Long Term", _owner.GetLongTermStack_Debug());
			_normal.text = ConstructString("Normal", _owner.GetNormalStack_Debug());
			_immediate.text = ConstructString("Immediate", _owner.GetImmediateStack_Debug());

			_hostility.text = App.AIBlackboard.IsHostileToPlayer(_owner) ? _hostileText : _friendlyText;

			_health.text = _owner.Health.IsDead ? _deadText : string.Format(_healthText, _owner.Health.CurrentHealth, _owner.Config.MaxHealth);
		}

		private string ConstructString(string title, State[] states)
		{
			StringBuilder builder = new StringBuilder();
			foreach(State state in states)
			{
				builder.Append(state.Config.StateType.ToString());
				builder.Append("\n");
			}

			builder.Append("\n");
			builder.Append("<color=yellow>");
			builder.Append(title);
			builder.Append("</color>");

			return builder.ToString();
		}
	}
}