using UnityEngine;

namespace Dissertation.UI
{
	public class MenuBase : MonoBehaviour
	{
		protected RectTransform _rectTransform { get; private set; }
		protected Canvas _canvas { get; private set; }

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			_canvas = GetComponentInParent<Canvas>();
		}

		public virtual void Initialise()
		{ }

		protected virtual void Update()
		{ }
	}
}