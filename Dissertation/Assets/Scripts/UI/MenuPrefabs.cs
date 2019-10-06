using UnityEngine;

namespace Dissertation.UI
{
	[CreateAssetMenu(fileName = "MenuPrefabs.asset", menuName = "Dissertation/Scriptables/UI/Menu Prefabs")]
	public class MenuPrefabs : ScriptableObject
	{
		[SerializeField] public MenuBase[] Prefabs;
	}
}