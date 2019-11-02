using Dissertation.Util;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Dissertation.UI
{
	public class SpeechBubble : InWorldMenu
	{
		[SerializeField] private Text _text;
		[SerializeField] private CanvasGroup _group;

		[SerializeField] private float _fadeInTime = 0.2f;
		[SerializeField] private float _fadeOutTime = 0.2f;
		private float _fadeInSpeed;
		private float _fadeOutSpeed;

		private float _showDuration = 5.0f;
		private const float _wordsPerSecond = 60.0f / 100.0f; //Google says average adult reads 200-250 wpm, but that seems a bit quick so slow it down

		private Func<bool> _shouldCloseEarly;

		private Coroutine _present = null;
		private bool _isFadingOut = false;

		public Action OnClose;

		public void Show( Transform actor, string text, Func<bool> shouldCloseEarly = null )
		{
			float showDuration = _wordsPerSecond * WordCount(text);
			Show(actor, text, showDuration, shouldCloseEarly);
		}

		public void Show( Transform actor, string text, float showDuration, Func<bool> shouldCloseEarly = null )
		{
			_text.text = text;
			TrackObject(actor);

			_showDuration = showDuration;

			_fadeInSpeed = 1.0f / _fadeInTime;
			_fadeOutSpeed = 1.0f / _fadeOutTime;

			_shouldCloseEarly = shouldCloseEarly;

			_present = StartCoroutine(Present());
		}

		private IEnumerator Present()
		{
			float t = 0.0f;
			while (t <= 1.0f)
			{
				_group.alpha = Mathf.Lerp(0.0f, 1.0f, t);
				yield return null;
				t += _fadeInSpeed * Time.deltaTime;
			}

			yield return new WaitForSeconds(_showDuration);

			yield return FadeOut();
		}

		private IEnumerator FadeOut()
		{
			_isFadingOut = true;

			float t = 0.0f;
			while (t <= 1.0f)
			{
				_group.alpha = Mathf.Lerp(1.0f, 0.0f, t);
				yield return null;
				t += _fadeOutSpeed * Time.deltaTime;
			}

			CloseMenu();
		}

		protected override void Update()
		{
			base.Update();

			if(!_isFadingOut && _shouldCloseEarly != null && _shouldCloseEarly())
			{
				StopCoroutine(_present);
				StartCoroutine(FadeOut());
			}
		}

		public override void CloseMenu()
		{
			base.CloseMenu();

			OnClose.InvokeSafe();
		}

		private int WordCount(string text)
		{
			int wordCount = 0, index = 0;

			// skip whitespace until first word
			while (index < text.Length && char.IsWhiteSpace(text[index]))
			{
				index++;
			}

			while (index < text.Length)
			{
				// check if current char is part of a word
				while (index < text.Length && !char.IsWhiteSpace(text[index]))
					index++;

				wordCount++;

				// skip whitespace until next word
				while (index < text.Length && char.IsWhiteSpace(text[index]))
					index++;
			}

			return wordCount;
		}
	}
}