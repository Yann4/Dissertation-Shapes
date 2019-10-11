using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Util
{
	public class PrefabPool
	{
		private GameObject _prefab = null;

		private Transform _poolRoot = null;
		private bool _reparentOnReturn = false;

		private List<GameObject> _free;
		private HashSet<GameObject> _used;

		public PrefabPool(GameObject prefab, int initialPoolSize, bool reparentOnReturn = false, Transform poolParent = null)
		{
			Debug.Assert(prefab != null);
			Debug.Assert(initialPoolSize > 0);

			_prefab = prefab;
			_poolRoot = poolParent;
			_reparentOnReturn = reparentOnReturn;

			_free = new List<GameObject>(initialPoolSize);
			_used = new HashSet<GameObject>();

			for(int idx = 0; idx < initialPoolSize; idx++)
			{
				_free.Add( CreateInstance() );
			}
		}

		public GameObject GetInstance(Transform parent)
		{
			if(_free.Count == 0)
			{
				_free.Add(CreateInstance(true));
			}

			int lastIndex = _free.Count - 1;

			GameObject instance = _free[lastIndex];
			_free.RemoveAt(lastIndex);
			_used.Add(instance);

			instance.transform.SetParent(parent);
			SetActive(instance, true);

			return instance;
		}

		public T GetInstance<T>(Transform parent)
		{
			return GetInstance(parent).GetComponent<T>();
		}

		public void ReturnInstance(GameObject instance)
		{
			if(!_used.Contains(instance))
			{
				GameObject.Destroy(instance);
			}
			else
			{
				Debug.Assert(!_free.Contains(instance));
				_free.Add(instance);
				_used.Remove(instance);

				SetActive(instance, false);
				if(_reparentOnReturn)
				{
					instance.transform.SetParent(_poolRoot);
				}
			}
		}

		private GameObject CreateInstance(bool instantiateActivated = false)
		{
			GameObject instance = GameObject.Instantiate(_prefab, _poolRoot);
			SetActive(instance, instantiateActivated);

			return instance;
		}

		private void SetActive(GameObject obj, bool active)
		{
			if(obj.activeInHierarchy != active)
			{
				obj.SetActive(active);
			}
		}
	}
}
