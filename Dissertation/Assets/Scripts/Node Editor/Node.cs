using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Dissertation.Util;
using System.Collections.Generic;

namespace Dissertation.NodeGraph
{
	public class Node
	{
		public Rect NodeRect;
		protected string Title = "Hello";
#if UNITY_EDITOR
		private GUID Guid;
#endif //UNITY_EDITOR
		public int UID { get; private set; }

		private GUIStyle _style;
		private GUIStyle _defaultNodeStyle;
		private GUIStyle _selectedNodeStyle;

#if UNITY_EDITOR
		private bool _isHeld = false;
		protected bool _isSelected = false;

		protected Vector2 _selectedSize;
		protected Vector2 _unselectedSize;
#endif //UNITY_EDITOR

		public ConnectionPoint InPoint { get; private set; }
		public ConnectionPoint OutPoint { get; private set; }

		private Action<Node> _onRemoveNode;

		public Node[] ConnectedTo;
		public int PreviousOption;
		private string _previousOptionField = "-1";

#if UNITY_EDITOR
		public Node(Vector2 position, Vector2 size, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode, GUID? guid = null)
		{
			_unselectedSize = size;
			_selectedSize = size;

			NodeRect = new Rect(position.x, position.y, size.x, size.y);
			_style = nodeStyle;
			_defaultNodeStyle = nodeStyle;
			_selectedNodeStyle = selectedStyle;
			PreviousOption = -1;

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

			UID = Guid.GetHashCode();
		}
#endif //UNITY_EDITOR

		public Node(BinaryReader reader, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode)
		{
			Deserialise(reader);

			_style = nodeStyle;
			_defaultNodeStyle = nodeStyle;
			_selectedNodeStyle = selectedStyle;

			InPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, onClickInPoint);
			OutPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, onClickOutPoint);
			_onRemoveNode = onRemoveNode;
		}

		public Node(BinaryReader reader)
		{
			Deserialise(reader);

			InPoint = new ConnectionPoint(this, ConnectionPointType.In);
			OutPoint = new ConnectionPoint(this, ConnectionPointType.Out);
		}

		//Pass in the nodes that are connected to the out point only
		public virtual void Connect(List<Node> connectsTo)
		{
			ConnectedTo = new Node[connectsTo.Count];

			foreach (Node node in connectsTo)
			{
				ConnectedTo[node.PreviousOption] = node;
			}
		}

		public virtual void Serialize(BinaryWriter writer)
		{
			writer.Write(UID);

			if (_isSelected)
			{
				NodeRect.size = _unselectedSize;
			}

			writer.Write(NodeRect.x);
			writer.Write(NodeRect.y);
			writer.Write(NodeRect.width);
			writer.Write(NodeRect.height);

			if (_isSelected)
			{
				NodeRect.size = _selectedSize;
			}

			writer.Write(_unselectedSize.x);
			writer.Write(_unselectedSize.y);
			writer.Write(_selectedSize.x);
			writer.Write(_selectedSize.y);

			writer.Write(Title);

			if (!int.TryParse(_previousOptionField, out int val))
			{
				Debug.LogErrorFormat("Couldn't parse input on previous option index - index found is '{1}'", _previousOptionField);
			}

			writer.Write(PreviousOption);
		}

		protected virtual void Deserialise(BinaryReader reader)
		{
			UID = reader.ReadInt32();

			NodeRect = new Rect(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			Title = reader.ReadString();
			PreviousOption = reader.ReadInt32();
			_previousOptionField = PreviousOption.ToString();
		}

#if UNITY_EDITOR
		public void Drag(Vector2 delta)
		{
			NodeRect.position += delta;
		}

		public void Draw()
		{
			InPoint.Draw();
			OutPoint.Draw();
			GUI.Box(NodeRect, Title, _style);

			EditorGUILayout.BeginVertical();

			Rect currentContentRect = new Rect(NodeRect.x + 10, NodeRect.y + 30, NodeRect.width - 20, 20);
			DrawContent(ref currentContentRect);

			EditorGUILayout.EndVertical();
		}

		protected virtual void DrawContent(ref Rect currentContentRect)
		{
			GUI.Label(currentContentRect, new GUIContent("Previous option index"));
			currentContentRect.y += currentContentRect.height + 5;

			int value = PreviousOption;
			_previousOptionField = GUI.TextField(currentContentRect, _previousOptionField);
			currentContentRect.y += currentContentRect.height + 5;
			if (int.TryParse(_previousOptionField, out value))
			{
				PreviousOption = value;
			}
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
					OnSelect();
				}
				else
				{
					_isSelected = false;
					_style = _defaultNodeStyle;
					OnDeselect();
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

		protected virtual void OnSelect()
		{ }

		protected virtual void OnDeselect()
		{ }
#endif //UNITY_EDITOR
	}
}