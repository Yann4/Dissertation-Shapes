using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dissertation.NodeGraph
{
	public static class NodeUtils
	{
		public static readonly string Extension = "bytes";
		public static readonly string ExtensionWithDot = ".bytes";

		public static void SaveGraph(string path, List<Node> nodes, List<Connection> connections)
		{
			BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate), System.Text.Encoding.UTF8);
			writer.Write(nodes.Count);
			foreach (Node node in nodes)
			{
				node.Serialize(writer);
			}

			writer.Write(connections.Count);
			foreach (Connection connection in connections)
			{
				connection.Serialise(writer);
			}

			writer.Close();
		}

		public static Node LoadGraph(string path, Func<BinaryReader, Node> createNode, List<Node> nodes = null, List<Connection> connections = null, Action<Connection> onClickRemoveConnection = null )
		{
			if (!File.Exists(path))
			{
				Debug.LogError("Couldn't find node graph at path " + path);
				return null;
			}

			FileStream stream = new FileStream(path, FileMode.Open);
			byte[] data = new byte[stream.Length];
			stream.Read(data, 0, (int)stream.Length);
			stream.Close();

			return LoadGraph(data, createNode, nodes, connections, onClickRemoveConnection);
		}

		public static Node LoadGraph(byte[] data, Func<BinaryReader, Node> createNode, List<Node> nodes = null, List<Connection> connections = null, Action<Connection> onClickRemoveConnection = null )
		{
			if (nodes == null)
			{
				nodes = new List<Node>();
			}
			else
			{
				nodes.Clear();
			}

			if (connections == null)
			{
				connections = new List<Connection>();
			}
			else
			{
				connections.Clear();
			}

			Node startNode = null;
			BinaryReader reader = new BinaryReader(new MemoryStream(data), System.Text.Encoding.UTF8);
			int numNodes = reader.ReadInt32();
			for (int idx = 0; idx < numNodes; idx++)
			{
				Node node = createNode(reader);
				nodes.Add(node);

				if(node.PreviousOption == -1)
				{
					Debug.Assert(startNode == null, "Can't have multiple start nodes (ones with PreviousOption == -1)");
					startNode = node;
				}
			}

			numNodes = reader.ReadInt32();
			for (int idx = 0; idx < numNodes; idx++)
			{
				if (onClickRemoveConnection != null)
				{
					connections.Add(new Connection(reader, nodes, onClickRemoveConnection));
				}
				else
				{
					connections.Add(new Connection(reader, nodes));
				}
			}

			reader.Close();

			ConnectNodes(nodes, connections);

			return startNode;
		}

		private static void ConnectNodes(List<Node> nodes, List<Connection> connections)
		{
			List<Connection> unlinkedConnections = new List<Connection>(connections);

			foreach (Node node in nodes)
			{
				List<Node> nodeConnections = new List<Node>();
				for (int idx = unlinkedConnections.Count - 1; idx >= 0; idx--)
				{
					if (unlinkedConnections[idx].OutPoint.Node == node)
					{
						nodeConnections.Add(unlinkedConnections[idx].InPoint.Node);
						unlinkedConnections.RemoveAt(idx);
					}
				}

				node.Connect(nodeConnections);
			}

			Debug.Assert(unlinkedConnections.Count == 0, "There are connections that aren't accounted for");
		}

		public static Node CreateNode<T>(BinaryReader reader) where T : Node
		{
			if (typeof(T) == typeof(ConversationNode))
			{
				return new ConversationNode(reader);
			}

			return new Node(reader);
		}
	}
}