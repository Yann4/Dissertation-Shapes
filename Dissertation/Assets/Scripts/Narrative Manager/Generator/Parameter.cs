﻿
using System;
using UnityEngine;

namespace Dissertation.Narrative.Generator
{
	public class Parameter
	{
		public Tags Tag;
		public int Label;

		public Parameter(string tag, string label)
		{
			if(!Enum.TryParse<Tags>(tag, true, out Tag))
			{
				Debug.LogErrorFormat("%s is not a valid Tags enum value", tag);
			}

			if(!int.TryParse(label, out Label))
			{
				Debug.LogErrorFormat("%s is not an integer", label);
			}
		}

		public override string ToString()
		{
			return Tag + ":" + Label;
		}

		public override bool Equals(object obj)
		{
			Parameter rhs = obj as Parameter;
			if(rhs != null)
			{
				return Tag == rhs.Tag && Label == rhs.Label;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return Tag.GetHashCode() ^ Label.GetHashCode() ^ base.GetHashCode();
		}
	}
}