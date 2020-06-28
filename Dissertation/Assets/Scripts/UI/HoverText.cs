using UnityEngine;
using UnityEngine.UI;

namespace Dissertation.UI
{
	public class HoverText : InWorldMenu
	{
		[SerializeField] private Text _text;

		internal void SetText(string text)
		{
			_text.text = text;
		}

		internal void SetTrackedObject(Transform obj, Vector3 offset = default(Vector3))
		{
			_trackedObject = obj;
			_offset = offset;
		}
	}
}
