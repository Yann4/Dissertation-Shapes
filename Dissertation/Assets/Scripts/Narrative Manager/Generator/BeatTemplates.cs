using UnityEngine;

namespace Dissertation.Narrative.Generator
{
	[CreateAssetMenu(fileName = "BeatTemplates.asset", menuName = "Dissertation/Scriptables/Narrative/Beat Templates")]
	public class BeatTemplates : ScriptableObject
	{
		public Beat[] Templates = new Beat[0];
	}
}