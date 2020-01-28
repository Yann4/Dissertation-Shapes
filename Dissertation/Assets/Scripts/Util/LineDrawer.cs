using UnityEngine;
using System.Collections.Generic;
using System;

namespace Dissertation.Util
{
	[RequireComponent(typeof(Camera))]
	public class LineDrawer : MonoBehaviour
	{
		// Choose the Unlit/Color shader in the Material Settings
		// You can change that color, to change the color of the connecting lines
		[SerializeField, Tooltip("Works best with Unlit/Color shader")] private Material _lineMat;

		public struct Graph
		{
			public GameObject CentrePoint;
			public GameObject[] Points;
		}

		private List<Graph> _networksToDraw = new List<Graph>();

		public static Action<LineDrawer> OnDrawerAwake;

		private void Awake()
		{
			OnDrawerAwake.InvokeSafe(this);
		}

		public void AddGraph(Graph graph)
		{
			for(int idx = 0; idx < _networksToDraw.Count; idx++)
			{
				if(graph.CentrePoint == _networksToDraw[idx].CentrePoint)
				{
					_networksToDraw[idx] = graph;
					return;
				}
			}

			_networksToDraw.Add(graph);
		}

		public void RemoveGraph(GameObject centrePoint)
		{
			_networksToDraw.RemoveAll(g => g.CentrePoint == centrePoint);
		}

		// Connect all of the `points` to the `mainPoint`
		private void DrawConnectingLines()
		{
			foreach (Graph graph in _networksToDraw)
			{
				if (graph.CentrePoint && graph.Points.Length > 0)
				{
					// Loop through each point to connect to the mainPoint
					foreach (GameObject point in graph.Points)
					{
						Vector3 mainPointPos = graph.CentrePoint.transform.position;
						Vector3 pointPos = point.transform.position;

						GL.Begin(GL.LINES);
						_lineMat.SetPass(0);
						GL.Color(new Color(_lineMat.color.r, _lineMat.color.g, _lineMat.color.b, _lineMat.color.a));
						GL.Vertex3(mainPointPos.x, mainPointPos.y, mainPointPos.z);
						GL.Vertex3(pointPos.x, pointPos.y, pointPos.z);
						GL.End();
					}
				}
			}
		}

		// To show the lines in the game window when it is running
		private void OnPostRender()
		{
			DrawConnectingLines();
		}

		// To show the lines in the editor
		private void OnDrawGizmos()
		{
			DrawConnectingLines();
		}
	}
}