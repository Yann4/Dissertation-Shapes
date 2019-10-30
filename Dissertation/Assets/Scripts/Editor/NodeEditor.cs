using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Dissertation.Editor
{
	public class NodeEditor : EditorWindow
	{
		protected string Title = "Node Editor";
		private GUIStyle _nodeStyle;
		private GUIStyle _selectedNodeStyle;
		private GUIStyle _inStyle;
		private GUIStyle _outStyle;
		private Vector2 _defaultNodeSize = new Vector2(200.0f, 50.0f);

		private List<Node> _nodes = new List<Node>();
		private List<Connection> _connections = new List<Connection>();

		private ConnectionPoint _selectedInPoint = null;
		private ConnectionPoint _selectedOutPoint = null;

		private Vector2 _drag;
		private Vector2 _offset;

		private string _path;

		[MenuItem("Window/Node Editor")]
		private static void OpenWindow()
		{
			NodeEditor window = GetWindow < NodeEditor >();
			window.titleContent = new GUIContent(window.Title);
		}

		private void OnEnable()
		{
			_nodeStyle = new GUIStyle();
			_nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
			_nodeStyle.border = new RectOffset(12, 12, 12, 12);

			_selectedNodeStyle = new GUIStyle();
			_selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
			_selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

			_inStyle = new GUIStyle();
			_inStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
			_inStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
			_inStyle.border = new RectOffset(4, 4, 12, 12);

			_outStyle = new GUIStyle();
			_outStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
			_outStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
			_outStyle.border = new RectOffset(4, 4, 12, 12);
		}

		private void OnGUI()
		{
			DrawToolbar();

			DrawGrid(20, 0.2f, Color.gray);
			DrawGrid(100, 0.4f, Color.gray);

			DrawNodes();
			DrawConnections();
			DrawConnectionLine(Event.current);

			ProcessNodeEvents(Event.current);
			ProcessEvents(Event.current);

			if(GUI.changed)
			{
				Repaint();
			}
		}

		private void DrawNodes()
		{
			for(int idx = 0; idx < _nodes.Count; idx++)
			{
				_nodes[idx].Draw();
			}
		}

		private void DrawConnections()
		{
			for(int idx = 0; idx < _connections.Count; idx++)
			{
				_connections[idx].Draw();
			}
		}

		private void DrawConnectionLine(Event e)
		{
			if(_selectedInPoint != null && _selectedOutPoint == null)
			{
				Handles.DrawBezier(_selectedInPoint.Rect.center, e.mousePosition,
					_selectedInPoint.Rect.center + (Vector2.left * 50.0f),
					e.mousePosition - (Vector2.left * 50.0f),
					Color.gray, null, 2.0f);

				GUI.changed = true;
			}

			if (_selectedOutPoint != null && _selectedInPoint == null)
			{
				Handles.DrawBezier(_selectedOutPoint.Rect.center, e.mousePosition,
					_selectedOutPoint.Rect.center - (Vector2.left * 50.0f),
					e.mousePosition - (Vector2.left * 50.0f),
					Color.gray, null, 2.0f);

				GUI.changed = true;
			}
		}

		private void DrawGrid(float spacing, float opacity, Color colour)
		{
			int widthDivs = Mathf.CeilToInt(position.width / spacing);
			int heightDivs = Mathf.CeilToInt(position.height / spacing);

			Handles.BeginGUI();
			Color existingColour = Handles.color;
			Handles.color = new Color(colour.r, colour.g, colour.b, opacity);

			_offset += _drag * 0.5f;
			Vector3 newOffset = new Vector3(_offset.x % spacing, _offset.y % spacing);

			for(int x = 0; x < widthDivs; x++)
			{
				Handles.DrawLine(new Vector3(spacing * x, -spacing) + newOffset, new Vector3(spacing * x, position.height) + newOffset);
			}

			for (int y = 0; y < heightDivs; y++)
			{
				Handles.DrawLine(new Vector3(-spacing, spacing * y) + newOffset, new Vector3(position.width, spacing * y) + newOffset);
			}

			Handles.color = existingColour;
			Handles.EndGUI();
		}

		private void DrawToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			if(GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton))
			{
				SaveGraph();
			}

			if (GUILayout.Button(new GUIContent("Load"), EditorStyles.toolbarButton))
			{
				LoadGraph();
			}

			GUILayout.EndHorizontal();
		}

		private void ProcessEvents(Event e)
		{
			_drag = Vector2.zero;

			switch(e.type)
			{
				case EventType.MouseDown:
				{
					if(e.button == 1)
					{
						ProcessContextMenu(e.mousePosition);
					}
					break;
				}
				case EventType.MouseDrag:
					if(e.button == 0)
					{
						OnDrag(e.delta);
					}
					break;
			}
		}

		private void ProcessNodeEvents(Event e)
		{
			for(int idx = _nodes.Count - 1; idx >= 0; idx--)
			{
				bool guiChanged = _nodes[idx].ProcessEvents(e);

				if(guiChanged)
				{
					GUI.changed = true;
				}
			}
		}

		private void ProcessContextMenu(Vector2 mousePosition)
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
			menu.ShowAsContext();
		}

		private void SaveGraph()
		{
			_path = EditorUtility.SaveFilePanel("Save to", Application.dataPath, "graph", "nodegraph");
			if (string.IsNullOrEmpty(_path))
			{
				return;
			}

			BinaryWriter writer = new BinaryWriter(new FileStream(_path, FileMode.OpenOrCreate), System.Text.Encoding.UTF32);
			writer.Write(_nodes.Count);
			foreach(Node node in _nodes)
			{
				node.Serialize(writer);
			}

			writer.Write(_connections.Count);
			foreach(Connection connection in _connections)
			{
				connection.Serialise(writer);
			}

			writer.Close();
		}

		private void LoadGraph()
		{
			_nodes.Clear();
			_connections.Clear();

			_path = EditorUtility.OpenFilePanel("Load from", Application.dataPath, "nodegraph");
			if(!File.Exists(_path))
			{
				return;
			}

			BinaryReader reader = new BinaryReader(new FileStream(_path, FileMode.Open), System.Text.Encoding.UTF32);
			int numNodes = reader.Read();
			for(int idx = 0; idx < numNodes; idx++)
			{
				_nodes.Add(Node.Deserialize(reader, _nodeStyle, _selectedNodeStyle, _inStyle, _outStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
			}

			numNodes = reader.Read();
			for (int idx = 0; idx < numNodes; idx++)
			{
				_connections.Add(Connection.Deserialize(reader, _nodes, OnClickRemoveConnection));
			}

			reader.Close();

			GUI.changed = true;
		}

		private void OnClickAddNode(Vector2 mousePosition)
		{
			_nodes.Add(new Node(mousePosition, _defaultNodeSize, _nodeStyle, _selectedNodeStyle, _inStyle, _outStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
		}

		private void OnClickInPoint(ConnectionPoint point)
		{
			_selectedInPoint = point;

			if(_selectedOutPoint != null)
			{
				if(_selectedOutPoint.Node != _selectedInPoint.Node)
				{
					CreateConnection();
				}

				ClearConnectionSelection();
			}
		}

		private void OnClickOutPoint(ConnectionPoint point)
		{
			_selectedOutPoint = point;

			if (_selectedInPoint != null)
			{
				if (_selectedOutPoint.Node != _selectedInPoint.Node)
				{
					CreateConnection();
				}

				ClearConnectionSelection();
			}
		}

		private void OnClickRemoveConnection(Connection connection)
		{
			_connections.Remove(connection);
		}

		private void OnClickRemoveNode(Node node)
		{
			for(int idx = _connections.Count - 1; idx >= 0; idx--)
			{
				if(_connections[idx].InPoint == node.InPoint || _connections[idx].OutPoint == node.OutPoint)
				{
					_connections.RemoveAt(idx);
				}
			}

			_nodes.Remove(node);
		}

		private void OnDrag(Vector2 delta)
		{
			_drag = delta;

			foreach(Node node in _nodes)
			{
				node.Drag(delta);
			}

			GUI.changed = true;
		}

		private void CreateConnection()
		{
			_connections.Add(new Connection(_selectedInPoint, _selectedOutPoint, OnClickRemoveConnection));
		}

		private void ClearConnectionSelection()
		{
			_selectedInPoint = null;
			_selectedOutPoint = null;
		}
	}
}