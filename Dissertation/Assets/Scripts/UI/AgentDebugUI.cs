using Dissertation.Character;
using Dissertation.Character.AI;
using Dissertation.Util;
using System.Collections.Generic;
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

		[SerializeField] private Text _moneyDesire;
		[SerializeField] private Text _safetyDesire;
		[SerializeField] private Text _powerDesire;

		private AgentController _owner;
		private LineDrawer _drawer;

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

			_drawer = Camera.main.GetComponent<LineDrawer>();
		}

		protected override void Update()
		{
			base.Update();

			RefreshText();

			List<BaseCharacterController> visibleCharacters = App.AIBlackboard.GetVisibleCharacters(_owner);
			GameObject[] visible = new GameObject[visibleCharacters.Count];
			for(int idx = 0; idx < visibleCharacters.Count; idx++)
			{
				visible[idx] = visibleCharacters[idx].gameObject;
			}

			LineDrawer.Graph graph = new LineDrawer.Graph() { CentrePoint = _owner.gameObject, Points = visible };
			_drawer.AddGraph(graph);
		}

		private void RefreshText()
		{
			_longTerm.text = ConstructStackTitleString("Long Term", _owner.GetLongTermStack_Debug());
			_normal.text = ConstructStackTitleString("Normal", _owner.GetNormalStack_Debug());
			_immediate.text = ConstructStackTitleString("Immediate", _owner.GetImmediateStack_Debug());

			_hostility.text = App.AIBlackboard.IsHostileToPlayer(_owner) ? _hostileText : _friendlyText;

			_health.text = _owner.Health.IsDead ? _deadText : string.Format(_healthText, _owner.Health.CurrentHealth, _owner.Config.MaxHealth);

			_moneyDesire.text = ConstructDesireString(DesireType.Money);
			_safetyDesire.text = ConstructDesireString(DesireType.Safety);
			_powerDesire.text = ConstructDesireString(DesireType.Power);
		}

		public override void SetVisible(bool visible)
		{
			base.SetVisible(visible);

			if(!visible)
			{
				_drawer.RemoveGraph(_owner.gameObject);
			}
		}

		private string ConstructStackTitleString(string title, State[] states)
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

		private string ConstructDesireString(DesireType type)
		{
			Desire desire = _owner.GetDesire_Debug(type);
			return string.Format("<color=yellow>{0}</color>: {1}/{2} (Rate: {3})", type, desire.Value.ToString("N2"), desire.MaxValue.ToString("N1"), desire.FillRate.ToString("N2"));
		}
	}
}