using UnityEngine;
using System;
using UnityEditor;
using Dissertation.Util;
using System.IO;

namespace Dissertation.Editor
{
	public class Node
	{
		public Rect NodeRect;
		private string Title = "Hello";
		public GUID Guid;

		private GUIStyle _style;
		private GUIStyle _defaultNodeStyle;
		private GUIStyle _selectedNodeStyle;

		private bool _isHeld = false;
		private bool _isSelected = false;

		public ConnectionPoint InPoint { get; private set; }
		public ConnectionPoint OutPoint { get; private set; }

		private Action<Node> _onRemoveNode;

		public Node(Vector2 position, Vector2 size, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode, GUID? guid = null)
		{
			NodeRect = new Rect(position.x, position.y, size.x, size.y);
			_style = nodeStyle;
			_defaultNodeStyle = nodeStyle;
			_selectedNodeStyle = selectedStyle;

			InPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, onClickInPoint);
			OutPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, onClickOutPoint);
			_onRemoveNode = onRemoveNode;

			if (!guid.HasValue)
			{
				Guid = GUID.Generate();
			}
			else
			{
				Guid = guid.Value;
			}
		}

		public void Drag(Vector2 delta)
		{
			NodeRect.position += delta;
		}

		public void Draw()
		{
			InPoint.Draw();
			OutPoint.Draw();
			GUI.Box(NodeRect, Title, _style);
		}

		public bool ProcessEvents(Event e)
		{
			switch(e.type)
			{
				case EventType.MouseDown:
					if(e.button == 0)
					{
						if(NodeRect.Contains(e.mousePosition))
						{
							_isHeld = true;
							_isSelected = true;
							_style = _selectedNodeStyle;
						}
						else
						{
							_isSelected = false;
							_style = _defaultNodeStyle;
						}

						GUI.changed = true;
					}
					else if(e.button == 1 && _isSelected && NodeRect.Contains(e.mousePosition))
					{
						ProcessContextMenu();
						e.Use();
					}
					break;
				case EventType.MouseUp:
					_isHeld = false;
					break;
				case EventType.MouseDrag:
					if(e.button == 0 && _isHeld)
					{
						Drag(e.delta);
						e.Use();
						return true;
					}
					break;
			}
			return false;
		}

		private void ProcessContextMenu()
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Remove node"), false, () => _onRemoveNode.InvokeSafe(this));
			menu.ShowAsContext();
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(Guid.ToString());
			writer.Write(NodeRect.x);
			writer.Write(NodeRect.y);
			writer.Write(NodeRect.width);
			writer.Write(NodeRect.height);
			writer.Write(Title);
		}

		public static Node Deserialize(BinaryReader reader, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode)
		{
			GUID id;
			GUID.TryParse(reader.ReadString(), out id);
			Rect nodeRect = new Rect(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			string title = reader.ReadString();
			return new Node(nodeRect.position, nodeRect.size, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onRemoveNode, id);
		}
	}
}