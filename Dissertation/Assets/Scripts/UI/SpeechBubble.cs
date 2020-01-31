using Dissertation.Util;
using System;
using System.Collections;
using System.Text;
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

		//Max default line width
		private const int _characterWidth = 30;
		private Vector3 _heightOffset = Vector3.zero;

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
			_heightOffset = Vector3.zero;

			string toShow = string.Empty;
			if(text.Contains("/n"))
			{
				//If the string has formatting in it, respect that
				toShow = text;
			}
			else
			{
				toShow = WordWrap(text, _characterWidth);
			}

			int newLineCount = text.Length - text.Replace("/n", "").Length;
			_heightOffset.y = newLineCount * 31f; //31 is a magic number that's currently the height of a single line

			_text.text = toShow;
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

			//This is an extra offset to make sure that the centre of the bubble
			//is in roughly the same place no matter how many lines the text has
			_rectTransform.localPosition += _heightOffset;
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
				{
					index++;
				}

				wordCount++;

				// skip whitespace until next word
				while (index < text.Length && char.IsWhiteSpace(text[index]))
				{
					index++;
				}
			}

			return wordCount;
		}

		/*
		 * Word wrap code shamelessly liberated from
		 * https://www.codeproject.com/Articles/51488/Implementing-Word-Wrap-in-C
		 * because word wrap is hard. This will only work for latin languages,
		 * and probably not all of them
		*/

		/// <summary>
		/// Word wraps the given text to fit within the specified width.
		/// </summary>
		/// <param name="text">Text to be word wrapped</param>
		/// <param name="width">Width, in characters, to which the text
		/// should be word wrapped</param>
		/// <returns>The modified text</returns>
		public static string WordWrap(string text, int width)
		{
			// Lucidity check
			if (width < 1 || text.Length < width)
			{
				return text;
			}

			int next;
			StringBuilder sb = new StringBuilder();

			// Parse each line of text
			for (int pos = 0; pos < text.Length; pos = next)
			{
				// Find end of line
				int eol = text.IndexOf(System.Environment.NewLine, pos);
				if (eol == -1)
				{
					eol = text.Length;
					next = eol;
				}
				else
				{
					next = eol + System.Environment.NewLine.Length;
				}

				// Copy this line of text, breaking into smaller lines as needed
				if (eol > pos)
				{
					do
					{
						int len = eol - pos;
						if (len > width)
						{
							len = BreakLine(text, pos, width);
						}

						sb.Append(text, pos, len);
						if (pos + len < text.Length) //Don't add a trailing newline
						{
							sb.Append(System.Environment.NewLine);
						}

						// Trim whitespace following break
						pos += len;
						while (pos < eol && Char.IsWhiteSpace(text[pos]))
						{
							pos++;
						}
					} while (eol > pos);
				}
				else
				{
					sb.Append(System.Environment.NewLine); // Empty line
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Locates position to break the given line so as to avoid
		/// breaking words.
		/// </summary>
		/// <param name="text">String that contains line of text</param>
		/// <param name="pos">Index where line of text starts</param>
		/// <param name="max">Maximum line length</param>
		/// <returns>The modified line length</returns>
		private static int BreakLine(string text, int pos, int max)
		{
			// Find last whitespace in line
			int i = max;
			while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
			{
				i--;
			}

			// If no whitespace found, break at maximum length
			if (i < 0)
			{
				return max;
			}

			// Find start of whitespace
			while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
			{
				i--;
			}

			// Return length of text before whitespace
			return i + 1;
		}
	}
}
 