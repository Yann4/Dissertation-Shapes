using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Dissertation.Util;

namespace Dissertation.Editor
{
	public class Node
	{
		public Rect NodeRect;
		protected string Title = "Hello";
		public GUID Guid;

		private GUIStyle _style;
		private GUIStyle _defaultNodeStyle;
		private GUIStyle _selectedNodeStyle;

		private bool _isHeld = false;
		protected bool _isSelected = false;

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

		public Node(BinaryReader reader, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode)
		{
			GUID.TryParse(reader.ReadString(), out Guid);
			NodeRect = new Rect(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			Title = reader.ReadString();

			_isSelected = reader.ReadBoolean();

			_style = nodeStyle;
			_defaultNodeStyle = nodeStyle;
			_selectedNodeStyle = selectedStyle;

			InPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, onClickInPoint);
			OutPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, onClickOutPoint);
			_onRemoveNode = onRemoveNode;
		}

		public void Drag(Vector2 delta)
		{
			NodeRect.position += delta;
		}

		public virtual void Draw()
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
					HandleMouseDown(e);
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
				case EventType.KeyDown:
					if(e.keyCode == KeyCode.Delete && _isSelected)
					{
						_onRemoveNode.InvokeSafe(this);
					}
					break;
			}

			return false;
		}

		private void HandleMouseDown(Event e)
		{
			if (e.button == 0)
			{
				if (NodeRect.Contains(e.mousePosition))
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
			else if (e.button == 1 && _isSelected && NodeRect.Contains(e.mousePosition))
			{
				ProcessContextMenu();
				e.Use();
			}
		}

		private void ProcessContextMenu()
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Remove node"), false, () => _onRemoveNode.InvokeSafe(this));
			menu.ShowAsContext();
		}

		public virtual void Serialize(BinaryWriter writer)
		{
			writer.Write(Guid.ToString());

			writer.Write(NodeRect.x);
			writer.Write(NodeRect.y);
			writer.Write(NodeRect.width);
			writer.Write(NodeRect.height);

			writer.Write(Title);

			writer.Write(_isSelected);
		}
	}
}