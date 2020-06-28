using Dissertation.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Teleporter : MonoBehaviour
{
	[SerializeField] private Teleporter _pairedTeleporter;
	[SerializeField] private Transform _teleportPosition;
	[SerializeField] private ParticleSystem _particleSystem;

	private BoxCollider2D _collider;
	private bool _teleportReady = true;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(_teleportReady && collision.gameObject.layer == Layers.Player)
		{
			StartCoroutine(Teleport(collision.gameObject));
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == Layers.Player)
		{
			_teleportReady = true;
		}
	}

	private IEnumerator Teleport(GameObject obj)
	{
		_pairedTeleporter._teleportReady = false;
		_particleSystem.Play();

		yield return new WaitForSeconds(2.0f);

		obj.transform.position = _pairedTeleporter._teleportPosition.position;

		yield return new WaitForSeconds(0.5f);

		_particleSystem.Stop();
		_teleportReady = true;
	}
}
