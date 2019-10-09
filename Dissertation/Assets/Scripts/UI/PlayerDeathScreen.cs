using System.Collections;
using UnityEngine;

namespace Dissertation.UI
{
	public class PlayerDeathScreen : MenuBase
	{
		[SerializeField] private float _fadeInDuration = 2.0f;
		[SerializeField] private float _stayDuration = 2.0f;
		[SerializeField] private float _fadeOutDuration = 1.0f;
		[SerializeField] private CanvasGroup _canvasGroup;

		IEnumerator Start()
		{
			float perc = 0.0f;
			while (perc <= 1.0f)
			{
				_canvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, perc);
				perc += Time.deltaTime * _fadeInDuration;
				yield return null;
			}

			yield return new WaitForSeconds(_stayDuration);

			perc = 0.0f;
			while (perc <= 1.0f)
			{
				_canvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, perc);
				perc += Time.deltaTime * _fadeOutDuration;
				yield return null;
			}

			HUD.Instance.DestroyMenu(this);
		}
	}
}