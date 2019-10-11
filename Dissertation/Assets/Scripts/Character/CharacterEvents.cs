using System;
using UnityEngine;

namespace Dissertation.Character
{
	public struct CharacterEvents
	{
		public Action<RaycastHit2D> OnControllerCollidedEvent;
		public Action<Collider2D> OnTriggerEnterEvent;
		public Action<Collider2D> OnTriggerStayEvent;
		public Action<Collider2D> OnTriggerExitEvent;

		//Combat events
		public Action OnMeleeAttackBegin;
		public Action OnMeleeAttackEnd;
		public Action<BaseCharacterController /*CharacterHit*/> OnMeleeAttackConnect;
	}
}
