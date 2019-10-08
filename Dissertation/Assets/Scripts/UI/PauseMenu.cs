using Dissertation.Character.Player;
using Dissertation.Input;
using UnityEngine;

namespace Dissertation.UI
{
	public class PauseMenu : MenuBase
	{
		public bool Paused { get; private set; } = false;
		private float _timeScale;

		public override void Initialise()
		{
			base.Initialise();
			SetVisible(false);
			_timeScale = Time.timeScale;
		}

		protected override void Update()
		{
			if(InputManager.GetButtonDown(InputAction.TogglePause))
			{
				SetVisible(!IsVisible());

				TogglePause();
			}
		}

		private void TogglePause()
		{
			if (!Paused)
			{
				_timeScale = Time.timeScale;
				Time.timeScale = 0;
			}
			else
			{
				Time.timeScale = _timeScale;
			}

			Paused = !Paused;
		}

		public void ToggleAgentDebugUI()
		{
			foreach(AgentDebugUI menu in HUD.Instance.FindMenus<AgentDebugUI>())
			{
				menu.SetVisible(!menu.IsVisible());
			}
		}

		public void FullyHealPlayer()
		{
			PlayerController player = FindObjectOfType<PlayerController>();
			player.Health.ModifyHealth(player.Config.MaxHealth);
		}

		public override void SetVisible(bool visible)
		{
			_canvas.enabled = visible;
		}

		public override bool IsVisible()
		{
			return _canvas.enabled;
		}
	}
}