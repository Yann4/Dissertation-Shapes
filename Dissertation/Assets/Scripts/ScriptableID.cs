using Dissertation.Narrative;
using UnityEngine;

namespace Dissertation
{
	[CreateAssetMenu(fileName = "EntityID.asset", menuName = "Dissertation/Scriptables/EntityID")]
	public class ScriptableID : ScriptableObject
	{
		[SerializeField] public ObjectClass Class = ObjectClass.ANY;
		[SerializeField] public int ID = -1;
	}
}