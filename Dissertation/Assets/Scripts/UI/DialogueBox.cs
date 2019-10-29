using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dissertation.UI
{
	public class DialogueBox : MenuBase, IPauser
	{
		[SerializeField] Text _title;
		[SerializeField] Text _body;

		[SerializeField] Text _yes;
		[SerializeField] Button _yesButton;
		[SerializeField] Text _no;
		[SerializeField] Button _noButton;

		[SerializeField] Text _acknowledge;
		[SerializeField] Button _acknowledgeButton;

		private Action _confirmAction = null;
		private Action _cancelAction = null;

		public void Show(string title, string body, string yes, string no, Action yesAction, Action noAction = null)
		{
			_title.text = title;
			_body.text = body;
			_yes.text = yes;
			_no.text = no;

			Debug.Assert(yesAction != null);

			_yesButton.gameObject.SetActive(true);
			_noButton.gameObject.SetActive(true);

			_confirmAction = yesAction;
			_cancelAction = noAction;

			OpenMenu();
		}

		public void Show(string title, string body, string acknowledge, Action acknowledgeAction = null)
		{
			_title.text = title;
			_body.text = body;

			if (acknowledgeAction != null)
			{
				_acknowledgeButton.onClick.AddListener(() => acknowledgeAction());
			}

			_acknowledgeButton.gameObject.SetActive(true);
			_confirmAction = acknowledgeAction;

			OpenMenu();
		}

		public override void OpenMenu()
		{
			base.OpenMenu();

			_canvas.overrideSorting = true;
		}

		public override void CloseMenu()
		{
			_yesButton.gameObject.SetActive(false);
			_noButton.gameObject.SetActive(false);
			_acknowledgeButton.gameObject.SetActive(false);

			_canvas.overrideSorting = false;

			base.CloseMenu();
		}

		public void Confirm()
		{
			if(_confirmAction != null)
			{
				_confirmAction();
				_confirmAction = null;
			}

			CloseMenu();
		}

		public void Cancel()
		{
			if(_cancelAction != null)
			{
				_cancelAction();
				_cancelAction = null;
			}

			CloseMenu();
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