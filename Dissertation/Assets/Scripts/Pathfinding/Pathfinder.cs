using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Pathfinding
{
	public static class Pathfinder
	{
		public static Node NearestNode(Vector3 position)
		{
			float shortestDistance = float.MaxValue;
			Node nearest = null;

			foreach(Node node in Node.AllNodes)
			{
				float magnitudeSqr = Vector3.SqrMagnitude(node.Position - position);
				if (magnitudeSqr < shortestDistance)
				{
					shortestDistance = magnitudeSqr;
					nearest = node;
				}
			}

			return nearest;
		}

		public static List<Node> GetPath(Vector3 start, Vector3 destination)
		{
			Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
			HashSet<Node> openSet = new HashSet<Node>();
			HashSet<Node> closedSet = new HashSet<Node>();
			Dictionary<Node, float> gScore = new Dictionary<Node, float>();
			Dictionary<Node, float> fScore = new Dictionary<Node, float>();

			Node startNode = NearestNode(start);
			Node destinationNode = NearestNode(destination);
			openSet.Add(startNode);

			gScore[startNode] = 0.0f;
			fScore[startNode] = Heuristic(startNode, destinationNode);

			while(openSet.Count != 0)
			{
				Node current = LowestScore(fScore);
				if(current == destinationNode)
				{
					List<Node> path = new List<Node>();
					path.Add(current);
					while(cameFrom.ContainsKey(current))
					{
						current = cameFrom[current];
						path.Insert(0, current);
					}

					return path;
				}

				if(!openSet.Remove(current))
				{
					Debug.LogError("Tried to remove node that wasn't in the open set");
					return null;
				}

				closedSet.Add(current);

				foreach(Node neighbour in current.Neighbours)
				{
					if(closedSet.Contains(neighbour))
					{
						continue;
					}

					float tentativeGScore = gScore[current] + current.DistanceToNeighbour(neighbour);
					float currentNeighbourG = float.MaxValue;
					if(!gScore.TryGetValue(neighbour, out currentNeighbourG) || tentativeGScore < currentNeighbourG)
					{
						cameFrom[neighbour] = current;
						gScore[neighbour] = tentativeGScore;
						fScore[neighbour] = tentativeGScore + Heuristic(neighbour, destinationNode);
						openSet.Add(neighbour);
					}
				}
			}

			return null;
		}

		private static float Heuristic(Node considering, Node destination)
		{
			return Vector3.SqrMagnitude(destination.Position - considering.Position);
		}

		private static Node LowestScore(Dictionary<Node, float> scores)
		{
			float lowestScore = float.MaxValue;
			Node lowest = null;

			foreach(KeyValuePair<Node, float> pair in scores)
			{
				if(pair.Value < lowestScore)
				{
					lowestScore = pair.Value;
					lowest = pair.Key;
				}
			}

			return lowest;
		}
	}
}