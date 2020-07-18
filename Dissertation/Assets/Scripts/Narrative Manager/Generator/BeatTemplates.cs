using System.Linq;
using UnityEngine;

namespace Dissertation.Narrative.Generator
{
	[CreateAssetMenu(fileName = "RuleTemplates.asset", menuName = "Dissertation/Scriptables/Narrative/Rule Templates")]
	public class BeatTemplates : ScriptableObject
	{
		[System.Serializable]
		public class NamedGraph
		{
			public string Name;
			public TextAsset Graph;
		}

		public NamedGraph[] Beats;

		public TextAsset GetTemplate(string name)
		{
			NamedGraph template = Beats.FirstOrDefault(x => x.Name == name);
			return template == null ? null : template.Graph;
		}
	}
}