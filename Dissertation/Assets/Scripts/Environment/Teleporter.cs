using Dissertation.UI;
using Dissertation.Util;
using System.Collections;
using UnityEngine;

namespace Dissertation.Environment
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class Teleporter : MonoBehaviour
	{
		[SerializeField] private Teleporter _pairedTeleporter;
		[SerializeField] private Transform _teleportPosition;
		[SerializeField] private ParticleSystem _particleSystem;

		private BoxCollider2D _collider;
		private bool _teleportReady = true;
		private HoverText _hoverText;
		private const string TeleporterPrompt = "/HUD/Teleporter_Prompt";

		private void Start()
		{
			_hoverText = HUD.Instance.CreateMenu<HoverText>();
			_hoverText.SetVisible(false);
			_hoverText.SetText(Util.Localisation.LocManager.GetTranslation(TeleporterPrompt));
			_hoverText.SetTrackedObject(transform, new Vector3(0.0f, 5.0f, 0.0f));
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (_teleportReady && collision.gameObject.layer == Layers.Player)
			{
				_hoverText.SetVisible(true);
			}
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			if (_teleportReady && collision.gameObject.layer == Layers.Player)
			{
				if (Input.InputManager.GetButtonDown(Input.InputAction.Interact))
				{
					StartCoroutine(Teleport(collision.gameObject));
				}
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.gameObject.layer == Layers.Player)
			{
				_hoverText.SetVisible(false);
			}
		}

		private IEnumerator Teleport(GameObject obj)
		{
			_teleportReady = false;

			_particleSystem.Play();

			yield return new WaitForSeconds(2.0f);

			obj.transform.position = _pairedTeleporter._teleportPosition.position;

			yield return new WaitForSeconds(0.5f);

			_particleSystem.Stop();
			_teleportReady = true;
		}
	}
}