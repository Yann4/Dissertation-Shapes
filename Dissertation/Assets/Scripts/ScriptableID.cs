using Dissertation.Narrative;
using UnityEngine;

namespace Dissertation
{
	[CreateAssetMenu(fileName = "EntityID.asset", menuName = "Dissertation/Scriptables/EntityID")]
	public class ScriptableID : ScriptableObject
	{
		public ObjectClass Class = ObjectClass.ANY;
		public int ID = -1;
	}
}