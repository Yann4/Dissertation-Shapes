using Dissertation.Character.Player;
using Dissertation.Input;
using Dissertation.Util.Localisation;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.UI
{
	public class PauseMenu : MenuBase, IPauser
	{
		[SerializeField] private List<GameObject> _disableInBuild = new List<GameObject>();

		public override void Initialise()
		{
			base.Initialise();
			SetVisible(false);

#if !UNITY_EDITOR
			foreach(GameObject toDisable in _disableInBuild)
			{
				toDisable.SetActive(false);
			}
#endif //!UNITY_EDITOR
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

		public void TryQuit()
		{
			DialogueBox dialogue = HUD.Instance.FindMenu<DialogueBox>();
			dialogue.Show(LocManager.GetTranslation("/Menu/Quit_Header"), LocManager.GetTranslation("/Menu/Quit_Body"), LocManager.GetTranslation("/Menu/Ok"), LocManager.GetTranslation("/Menu/Cancel"),
				() => App.Quit());
		}

		public void UnlockAllShapes()
		{
			PlayerController player = FindObjectOfType<PlayerController>();
			player.UnlockShape(Character.CharacterFaction.Circle);
			player.UnlockShape(Character.CharacterFaction.Triangle);
			player.UnlockShape(Character.CharacterFaction.Square);
		}

		public override void SetVisible(bool visible)
		{
			_canvas.enabled = visible;
		}

		public override bool IsVisible()
		{
			return _canvas.enabled;
		}

		public void RunGenerator()
		{
#if UNITY_EDITOR
			App.Generator.RunGeneration();
#endif //UNITY_EDITOR
		}

		public void DeleteGeneratedAssets()
		{
#if UNITY_EDITOR
			Narrative.Generator.GeneratorUtils.DeleteAllGeneratedAssets();
#endif //UNITY_EDITOR
		}
	}
}