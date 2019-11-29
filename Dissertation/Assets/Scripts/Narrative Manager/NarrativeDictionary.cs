using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Narrative
{
	[CreateAssetMenu(fileName = "NarrativeDictionary.asset", menuName = "Dissertation/Scriptables/Narrative/Narrative Dictionary")]
	public class NarrativeDictionary : ScriptableObject
	{
		[SerializeField] private List<WorldProperty> WorldProperties = new List<WorldProperty>();
		[SerializeField] private List<Action> Actions = new List<Action>();

		private static NarrativeDictionary _instance = null;

		public WorldProperty GetWorldProperty(string guid)
		{
			return WorldProperties.Find(property => property.guid == guid);
		}

		public Action GetAction(string guid)
		{
			return Actions.Find(action => action.guid == guid);
		}

		public static NarrativeDictionary GetAsset()
		{
			if(_instance == null)
			{
				_instance = Resources.Load<NarrativeDictionary>("NarrativeDictionary");
			}

			return _instance;
		}
	}
}