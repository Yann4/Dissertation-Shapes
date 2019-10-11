using Dissertation.Character.Player;
using Dissertation.Input;

namespace Dissertation.UI
{
	public class PauseMenu : MenuBase, IPauser
	{
		public override void Initialise()
		{
			base.Initialise();
			SetVisible(false);
		}

		protected override void Update()
		{
			if(InputManager.GetButtonDown(InputAction.TogglePause))
			{
				if (!IsOpen)
				{
					OpenMenu();
				}
				else
				{
					CloseMenu();
				}
			}
		}

		public void ToggleAgentDebugUI()
		{
			foreach(AgentDebugUI menu in HUD.Instance.FindMenus<AgentDebugUI>())
			{
				if(menu.IsVisible())
				{
					menu.CloseMenu();
				}
				else
				{
					menu.OpenMenu();
				}
			}
		}

		public void FullyHealPlayer()
		{
			PlayerController player = FindObjectOfType<PlayerController>();
			player.Health.Heal((uint)player.Config.MaxHealth);
		}

		protected override void SetVisible(bool visible)
		{
			_canvas.enabled = visible;
		}

		public override bool IsVisible()
		{
			return _canvas.enabled;
		}
	}
}