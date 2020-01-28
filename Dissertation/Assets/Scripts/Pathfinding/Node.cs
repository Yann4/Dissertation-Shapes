#define DRAW_DEBUG
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

#if UNITY_EDITOR && DRAW_DEBUG
		LineDrawer _debugDrawer = null;
		LineDrawer.Graph _graph;
#endif //UNITY_EDITOR && DRAW_DEBUG

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

#if UNITY_EDITOR && DRAW_DEBUG
			_debugDrawer = FindObjectOfType<LineDrawer>();

			if (_debugDrawer != null)
			{
				_graph = new LineDrawer.Graph()
				{
					CentrePoint = gameObject,
					Points = Neighbours.ConvertAll<GameObject>(node => node.gameObject).ToArray()
				};

				_debugDrawer.AddGraph(_graph);
			}
#endif //UNITY_EDITOR && DRAW_DEBUG
		}

		public float DistanceToNeighbour(Node neighbour)
		{
			return 1.0f;
		}
	}
}