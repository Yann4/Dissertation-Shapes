using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character.AI
{
	[CreateAssetMenu(fileName = "Trait.asset", menuName = "Dissertation/Scriptables/Character Config/Trait")]
	public class Trait : ScriptableObject
	{
		[SerializeField] private string _name;
		public string Name { get { return _name; } }

		[SerializeField] private List<Desire.Modifier> _desireModifiers = new List<Desire.Modifier>();
		public IEnumerable<Desire.Modifier> DesireModifiers { get { return _desireModifiers; } }

		[SerializeField] private SpecialistStates _specialBehaviours;
		public SpecialistStates SpecialBehaviours { get { return _specialBehaviours; } }
	}
}