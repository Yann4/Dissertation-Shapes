using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Narrative
{
	[CreateAssetMenu(fileName = "NarrativeDictionary.asset", menuName = "Dissertation/Scriptables/Narrative/Narrative Dictionary")]
	public class NarrativeDictionary : ScriptableObject
	{
		[SerializeField] private List<WorldPropertyScriptable> WorldProperties = new List<WorldPropertyScriptable>();
		[SerializeField] private List<Action> Actions = new List<Action>();

		private static NarrativeDictionary _instance = null;

		public WorldPropertyScriptable GetWorldProperty(string guid)
		{
			WorldPropertyScriptable foundProperty =  WorldProperties.Find(property => property.guid == guid);

			UnityEngine.Debug.Assert(foundProperty != null, "Couldn't find property with guid " + guid);

			return foundProperty;
		}

		public Action GetAction(string guid)
		{
			Action foundAction = Actions.Find(action => action.guid == guid);

			UnityEngine.Debug.Assert(foundAction != null, "Couldn't find action with guid " + guid);

			return foundAction;
		}

#if UNITY_EDITOR
		public void AddWorldProperty(WorldPropertyScriptable worldProperty)
		{
			if(!WorldProperties.Contains(worldProperty))
			{
				WorldProperties.Add(worldProperty);
				UnityEditor.EditorUtility.SetDirty(this);
			}
		}

		public void AddAction(Action action)
		{
			if (!Actions.Contains(action))
			{
				Actions.Add(action);
				UnityEditor.EditorUtility.SetDirty(this);
			}
		}

		public void Remove<T>(T asset) where T : ScriptableObject
		{
			WorldPropertyScriptable property = asset as WorldPropertyScriptable;
			if (property != null)
			{
				if (WorldProperties.Remove(property))
				{
					UnityEditor.EditorUtility.SetDirty(this);
				}
			}
			else
			{
				Action action = asset as Action;
				if (Actions.Remove(action))
				{
					UnityEditor.EditorUtility.SetDirty(this);
				}
			}
		}
#endif //UNITY_EDITOR

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