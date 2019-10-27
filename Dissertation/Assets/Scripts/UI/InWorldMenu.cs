using UnityEngine;

namespace Dissertation.UI
{
	public class InWorldMenu : MenuBase
	{
		[SerializeField] private Vector2 _offset;

		private Transform _trackedObject = null;

		private Vector2 _canvasOffset;
		private RectTransform _canvasTransform;

		protected virtual void Start()
		{
			_canvasTransform = _canvas.transform as RectTransform;
			_canvasOffset = new Vector2(_canvasTransform.sizeDelta.x / 2f, _canvasTransform.sizeDelta.y / 2f);
		}

		protected override void Update()
		{
			base.Update();

			if(_trackedObject != null)
			{
				_rectTransform.localPosition = TransformPosition(_trackedObject.position);
			}
		}

		protected Vector3 TransformPosition(Vector3 position)
		{
			Vector2 viewportPosition = Camera.main.WorldToViewportPoint(position);

			Vector2 proportionalPosition = new Vector2(viewportPosition.x * _canvasTransform.sizeDelta.x, viewportPosition.y * _canvasTransform.sizeDelta.y);

			return proportionalPosition - _canvasOffset + _offset;
		}

		protected void TrackObject(Transform toTrack)
		{
			_trackedObject = toTrack;
		}

		//Overrides any object tracking
		protected void SetPosition(Vector3 worldPosition)
		{
			_trackedObject = null;
			TransformPosition(worldPosition);
		}
	}
}