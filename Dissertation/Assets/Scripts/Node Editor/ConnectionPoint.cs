using UnityEngine;
using System;
using Dissertation.Util;

namespace Dissertation.NodeGraph
{
	public enum ConnectionPointType
	{
		In,
		Out
	}

	public class ConnectionPoint
	{
		public Rect Rect;
		private GUIStyle _style;
		private ConnectionPointType _type;
		private Action<ConnectionPoint> _onClickConnectionPoint;
		public Node Node { get; private set; }

		private const float _xOffset = 8.0f;

		public ConnectionPoint(Node node, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> onClick)
		{
			Node = node;
			_type = type;
			_style = style;
			_onClickConnectionPoint = onClick;
			Rect = new Rect(0, 0, 10, 20);
		}

		public ConnectionPoint(Node node, ConnectionPointType type)
		{
			Node = node;
			_type = type;
		}

		public void Draw()
		{
			Rect.y = Node.NodeRect.y + (Node.NodeRect.height * 0.5f) - (Rect.height * 0.5f);

			switch (_type)
			{
				case ConnectionPointType.In:
					Rect.x = Node.NodeRect.x - Rect.width + _xOffset;
					break;
				case ConnectionPointType.Out:
					Rect.x = Node.NodeRect.x + Node.NodeRect.width - _xOffset;
					break;
			}

			if(GUI.Button(Rect, "", _style))
			{
				_onClickConnectionPoint.InvokeSafe(this);
			}
		}
	}
}