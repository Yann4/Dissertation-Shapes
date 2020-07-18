using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dissertation.Narrative
{
	public class NarrativeObject : MonoBehaviour
	{
		[SerializeField] Generator.Tags _narrativeTag;

		private static NarrativeObject[] _allObjects;

		private void Start()
		{
			if(_allObjects == null)
			{
				_allObjects = FindObjectsOfType<NarrativeObject>();
			}
		}

		//An object has to match all of the tags to pass the comparison
		public static IEnumerable<NarrativeObject> GetObjectsAND(Generator.Tags tags)
		{
			return _allObjects.Where(obj => (obj._narrativeTag & tags) == tags);
		}

		//An object has to match any of the tags to pass the comparison
		public static IEnumerable<NarrativeObject> GetObjectsOR(Generator.Tags tags)
		{
			return _allObjects.Where(obj => (obj._narrativeTag & tags) != 0);
		}

		public static NarrativeObject GetObjectAND(Generator.Tags tags, out bool reused, HashSet<NarrativeObject> existingObjects = null)
		{
			NarrativeObject obj;
			if(existingObjects != null)
			{
				obj = existingObjects.FirstOrDefault(o => (o._narrativeTag & tags) == tags);
				if(obj != null)
				{
					reused = true;
					return obj;
				}
			}

			reused = false;
			return _allObjects.FirstOrDefault(o => (o._narrativeTag & tags) == tags);
		}

		public static NarrativeObject GetObjectOR(Generator.Tags tags, out bool reused, HashSet<NarrativeObject> existingObjects = null)
		{
			if (existingObjects != null)
			{
				NarrativeObject obj = existingObjects.FirstOrDefault(o => (o._narrativeTag & tags) != 0);
				if (obj != null)
				{
					reused = true;
					return obj;
				}
			}

			reused = false;
			return _allObjects.FirstOrDefault(o => (o._narrativeTag & tags) != 0);
		}
	}
}