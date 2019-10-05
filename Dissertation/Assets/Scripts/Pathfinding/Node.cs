using Dissertation.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Pathfinding
{
	public class Node : MonoBehaviour
	{
		[SerializeField] public List<Node> Neighbours;

		public Vector3 Position { get; private set; }
		public Transform Platform { get; private set; }

		public static List<Node> AllNodes = new List<Node>();

		private void Awake()
		{
			Position = transform.position;
			Platform = Positional.GetPlatform(Position);
			Debug.Assert(Platform != null);

			Neighbours.RemoveAll(x => x == null);

			//Don't add orphans to the graph
			if (Neighbours.Count > 0)
			{
				Debug.Assert(!AllNodes.Contains(this));
				AllNodes.Add(this);
			}
		}

		public float DistanceToNeighbour(Node neighbour)
		{
			return 1.0f;
		}
	}
}