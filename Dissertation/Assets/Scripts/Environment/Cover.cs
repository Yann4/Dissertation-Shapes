using Dissertation.Util;
using System.Collections;
using UnityEngine;

namespace Dissertation.Environment
{
	[RequireComponent(typeof(Collider2D)), RequireComponent(typeof(SpriteRenderer))]
	public class Cover : MonoBehaviour
	{
		private SpriteRenderer _renderer;
		private Coroutine _lerp;

		private void Start()
		{
			_renderer = GetComponent<SpriteRenderer>();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.layer == Layers.Player)
			{
				DoLerp(false);
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.gameObject.layer == Layers.Player)
			{
				DoLerp(true);
			}
		}

		private void DoLerp(bool lerpOn)
		{
			if (_lerp != null)
			{
				StopCoroutine(_lerp);
				_renderer.color = lerpOn ? new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0.0f) : new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 1.0f);
			}

			_lerp = StartCoroutine(Lerp(lerpOn));
		}

		private IEnumerator Lerp(bool lerpOn)
		{
			float t = 0;
			Color start = _renderer.color;
			Color target = _renderer.color;

			if (lerpOn)
			{
				start.a = 0.0f;
				target.a = 1.0f;
			}
			else
			{
				start.a = 1.0f;
				target.a = 0.0f;
			}

			while (t != 1.0f)
			{
				t += Time.deltaTime;
				_renderer.color = Color.LerpUnclamped(start, target, t);
				yield return null;
			}

			_lerp = null;
		}
	}
}