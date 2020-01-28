using UnityEngine;

/*
 * Used to find a node object that should be linked up 
 * to one in a different scene to connect up the 
 * "navmesh". There's definitely a better way to do this,
 * I'm just wanting a quick solution.
 */
namespace Dissertation.Pathfinding
{
	[RequireComponent(typeof(Node))]
	public class OffSceneLink : MonoBehaviour
	{
		public Node LinkNode { get; private set; }

		private void Awake()
		{
			LinkNode = GetComponent<Node>();
		}
	}
}