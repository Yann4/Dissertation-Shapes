using Dissertation.Character.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Dissertation.UI
{
	public class PlayerInventory : MenuBase, IPauser
	{
		[SerializeField] Text _currencyAmount;

		private PlayerController _player;

		public void Setup(PlayerController owner)
		{
			_player = owner;
		}

		public void Toggle()
		{
			bool visible = IsVisible();

			if (!visible)
			{
				//Update contents of screen if we're about to show it
				_currencyAmount.text = _player.Inventory.Contents.Currency.ToString();
				OpenMenu();
			}
			else
			{
				CloseMenu();
			}
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