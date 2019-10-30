﻿using Dissertation.Util;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dissertation.Editor
{
	public class Connection
	{
		public ConnectionPoint InPoint { get; private set; }
		public ConnectionPoint OutPoint { get; private set; }
		private Action<Connection> _onClick;

		public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> onClick)
		{
			InPoint = inPoint;
			OutPoint = outPoint;
			_onClick = onClick;
		}

		public void Draw()
		{
			Handles.DrawBezier(InPoint.Rect.center, OutPoint.Rect.center,
				InPoint.Rect.center + (Vector2.left * 50.0f),
				OutPoint.Rect.center - (Vector2.left * 50.0f),
				Color.white, null, 2.0f);
#pragma warning disable 0618 //This is obsolete
			if(Handles.Button((InPoint.Rect.center + OutPoint.Rect.center) * 0.5f, Quaternion.identity, 4.0f, 8.0f, Handles.RectangleCap))
			{
				_onClick.InvokeSafe(this);
			}
#pragma warning restore
		}

		public void Serialise(BinaryWriter writer)
		{
			writer.Write(InPoint.Node.Guid.ToString());
			writer.Write(OutPoint.Node.Guid.ToString());
		}

		public static Connection Deserialize(BinaryReader reader, List<Node> nodes, Action<Connection> onClickRemoveConnection)
		{
			GUID nodeGuid;
			GUID.TryParse(reader.ReadString(), out nodeGuid);
			Node inNode = nodes.Find(node => node.Guid == nodeGuid);

			GUID.TryParse(reader.ReadString(), out nodeGuid);
			Node outNode = nodes.Find(node => node.Guid == nodeGuid);

			return new Connection(inNode.InPoint, outNode.OutPoint, onClickRemoveConnection);
		}
	}
}