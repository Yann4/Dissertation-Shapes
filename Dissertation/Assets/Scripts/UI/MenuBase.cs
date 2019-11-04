using UnityEngine;
using UnityEngine.UI;

namespace Dissertation.UI
{
	public class MenuBase : MonoBehaviour
	{
		[SerializeField] private bool _destroyOnClose = true;
		[SerializeField] private Selectable _defaultSelectable = null;

		protected RectTransform _rectTransform { get; private set; }
		protected Canvas _canvas { get; private set; }

		public bool IsOpen { get; private set; }

		protected Selectable _currentSelectable = null;

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			_canvas = GetComponentInParent<Canvas>();
		}

		public virtual void Initialise()
		{ }

		protected virtual void Update()
		{ }

		public virtual void OpenMenu()
		{
			IsOpen = true;
			SetVisible(true);

			if(this is IPauser)
			{
				App.Pause();
			}

			if(_defaultSelectable != null)
			{
				_currentSelectable = _defaultSelectable;
				_currentSelectable.Select();
			}
		}

		public virtual void CloseMenu()
		{
			IsOpen = false;

			if (this is IPauser)
			{
				App.Resume();
			}

			if (_destroyOnClose)
			{
				Destroy(gameObject);
				return;
			}

			SetVisible(false);
		}

		public virtual void SetVisible(bool visible)
		{
			if(gameObject.activeSelf != visible)
			{
				gameObject.SetActive(visible);
			}
		}

		public virtual bool IsVisible()
		{
			return gameObject.activeSelf;
		}
	}
}