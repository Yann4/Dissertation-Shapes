using Dissertation.Narrative;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation
{
	public class EntityManager
	{
		private List<EntityID> _entities = new List<EntityID>();

		public EntityManager()
		{
			EntityID.OnSpawnEntity += OnEntitySpawned;
		}

		private void OnEntitySpawned(EntityID spawned)
		{
			if(_entities.Contains(spawned))
			{
				Debug.LogError("Duplicate entity id " + spawned.ObjectID, spawned.gameObject);
			}
			else
			{
				_entities.Add(spawned);
			}
		}

		public GameObject GetObject(long id)
		{
			EntityID entity = _entities.Find(ent => ent.ObjectID == id);
			if(entity != null)
			{
				return entity.gameObject;
			}

			Debug.LogErrorFormat("Entity with ID {0} not found", id);
			return null;
		}

		public GameObject[] GetObjects(ObjectClass objectClass)
		{
			List<EntityID> entities = _entities.FindAll(ent => ent.Class == objectClass);
			GameObject[] objects = new GameObject[entities.Count];
			for(int idx = 0; idx < entities.Count; idx++)
			{
				objects[idx] = entities[idx].gameObject;
			}

			return objects;
		}

		public bool HasObject(EntityID objectID)
		{
			return _entities.Contains(objectID);
		}
	}
}