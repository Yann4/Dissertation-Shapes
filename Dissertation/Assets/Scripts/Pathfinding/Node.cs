#define DRAW_DEBUG
using Dissertation.Util;
using System;
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
		private static bool _registeredToLevelLoadedEvent = false;

#if UNITY_EDITOR && DRAW_DEBUG
		LineDrawer _debugDrawer = null;
		LineDrawer.Graph _graph;
#endif //UNITY_EDITOR && DRAW_DEBUG

		private void Awake()
		{
			if(!_registeredToLevelLoadedEvent)
			{
				_registeredToLevelLoadedEvent = true;
				App.OnLevelLoaded += OnLevelLoaded;
			}

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

			if(_debugDrawer == null)
			{
				LineDrawer.OnDrawerAwake += OnDrawerAwake;
			}

			GenerateDebugGraph();
#endif //UNITY_EDITOR && DRAW_DEBUG
		}

		private void OnLevelLoaded()
		{
			List<OffSceneLink> toLink = new List<OffSceneLink>(FindObjectsOfType<OffSceneLink>());

			for(int inspecting = toLink.Count - 1; inspecting >= 0 && toLink.Count > 0; inspecting--)
			{
				int closestLink = -1;
				float closestDistance = float.MaxValue;
				for(int idx = 0; idx < inspecting; idx++)
				{
					float distance = Vector3.Distance(toLink[inspecting].transform.position, toLink[idx].transform.position);
					if(distance < closestDistance)
					{
						closestLink = idx;
						closestDistance = distance;
					}
				}

				toLink[inspecting].LinkNode.AddNeighbour(toLink[closestLink].LinkNode);
				toLink.RemoveAt(inspecting);
				toLink.RemoveAt(closestLink);
			}

			App.OnLevelLoaded -= OnLevelLoaded;
		}

		public void AddNeighbour(Node neighbour)
		{
			if(Neighbours.Contains(neighbour))
			{
				return;
			}

			Neighbours.Add(neighbour);
			GenerateDebugGraph();

			neighbour.AddNeighbour(this);
		}

		public float DistanceToNeighbour(Node neighbour)
		{
			return 1.0f;
		}

		private void GenerateDebugGraph()
		{
#if UNITY_EDITOR && DRAW_DEBUG
			if (_debugDrawer != null)
			{
				_debugDrawer.RemoveGraph(gameObject);

				_graph = new LineDrawer.Graph()
				{
					CentrePoint = gameObject,
					Points = Neighbours.ConvertAll<GameObject>(node => node.gameObject).ToArray()
				};

				_debugDrawer.AddGraph(_graph);
			}
#endif //UNITY_EDITOR && DRAW_DEBUG
		}

#if UNITY_EDITOR && DRAW_DEBUG
		private void OnDrawerAwake(LineDrawer drawer)
		{
			LineDrawer.OnDrawerAwake -= OnDrawerAwake;

			_debugDrawer = drawer;

			GenerateDebugGraph();
		}
#endif //UNITY_EDITOR && DRAW_DEBUG
	}
}