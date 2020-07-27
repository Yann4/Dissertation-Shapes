using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Environment
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class KillVolume : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D collision)
		{
			HandleKilling(collision.gameObject);
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			HandleKilling(collision.gameObject);
		}

		private void HandleKilling(GameObject obj)
		{
			Character.BaseCharacterController character = obj.GetComponent<Character.BaseCharacterController>();
			if (character != null)
			{
				if (!character.IsDashAttacking)
				{
					character.Health.Kill();
				}
			}
		}
	}
}