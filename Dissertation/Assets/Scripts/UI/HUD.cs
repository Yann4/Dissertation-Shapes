using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.UI
{
	public class HUD : MonoBehaviour
	{
		public static HUD Instance { get; private set; } = null;

		[SerializeField] private RectTransform _root;
		[SerializeField] private MenuPrefabs _prefabs;
		private Dictionary<Type, MenuBase> _prefabMap = new Dictionary<Type, MenuBase>();

		private List<MenuBase> _instantiatedMenus = new List<MenuBase>();

		private void Awake()
		{
			Debug.Assert(Instance == null);
			Instance = this;

			foreach(MenuBase menu in _prefabs.Prefabs)
			{
				Type type = menu.GetType();
				if(_prefabMap.ContainsKey(type))
				{
					Debug.Assert(false, "Duplicate typed menu in MenuPrefabs.asset: " + type);
				}

				_prefabMap[type] = menu;
			}
		}

		private void Start()
		{
			CreateMenu<PauseMenu>();
			CreateMenu<Respawner>();
		}

		private void OnDestroy()
		{
			DestroyMenu<PauseMenu>();
			DestroyMenu<Respawner>();
		}

		private T FindPrefab<T>() where T : MenuBase
		{
			if(_prefabMap.TryGetValue(typeof(T), out MenuBase prefab))
			{
				return prefab as T;
			}

			return null;
		}

		public T CreateMenu<T>() where T : MenuBase
		{
			T prefab = FindPrefab<T>();
			Debug.Assert(prefab != null, "Menu " + typeof(T) + " is not in MenuPrefabs scriptable");

			T menu = Instantiate(prefab, _root);
			_instantiatedMenus.Add(menu);
			menu.Initialise();

			return menu;
		}

		//Returns first menu of type. Don't use if you're expecting there to be
		//multiple menus of the same type
		public T FindMenu<T>(bool allowCreation = true) where T : MenuBase
		{
			Type type = typeof(T);
			foreach (MenuBase menu in _instantiatedMenus)
			{
				if(menu.GetType() == type)
				{
					return menu as T;
				}
			}

			if(allowCreation)
			{
				return CreateMenu<T>();
			}

			return null;
		}

		public List<T> FindMenus<T>() where T : MenuBase
		{
			List<T> toReturn = new List<T>();

			Type type = typeof(T);
			foreach(MenuBase menu in _instantiatedMenus)
			{
				if(menu.GetType() == type)
				{
					toReturn.Add(menu as T);
				}
			}

			return toReturn;
		}

		public void DestroyMenu(MenuBase menu)
		{
			if (menu != null)
			{
				_instantiatedMenus.Remove(menu);
				Destroy(menu.gameObject);
			}
			else
			{
				Debug.LogError("Can't destroy null menu");
			}
		}

		public void DestroyMenu<T>() where T : MenuBase
		{
			DestroyMenu(FindMenu<T>());
		}
	}
}