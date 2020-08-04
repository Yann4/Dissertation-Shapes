using Dissertation.Narrative;
using Dissertation.Util;
using System;
using UnityEngine;

namespace Dissertation
{
	public class EntityID : MonoBehaviour
	{
		[SerializeField] private ScriptableID _scriptID;
		[SerializeField] private ObjectClass _class = ObjectClass.ANY;
		[SerializeField] private int _id = INVALID_ID;

		public ObjectClass Class { get { return _class; } }
		public long ObjectID { get; private set; }

		private const int INVALID_ID = -1;
		public static Action<EntityID> OnSpawnEntity;

		public void SetID(ScriptableID id)
		{
			_scriptID = id;

			_class = _scriptID.Class;
			_id = _scriptID.ID;

			ObjectID = GetObjectID(Class, _id);
			if (ObjectID != INVALID_ID && !App.WorldState.Entities.HasObject(this))
			{
				OnSpawnEntity.InvokeSafe(this);
			}
		}

		private void Awake()
		{
			if (_scriptID != null)
			{
				_class = _scriptID.Class;
				_id = _scriptID.ID;
			}

			ObjectID = GetObjectID(Class, _id);
			if (ObjectID != INVALID_ID && !App.WorldState.Entities.HasObject(this))
			{
				OnSpawnEntity.InvokeSafe(this);
			}
		}

		private long GetObjectID(ObjectClass type, int index)
		{
			return ((long)type << 32) | ((long)index);
		}
	}
}