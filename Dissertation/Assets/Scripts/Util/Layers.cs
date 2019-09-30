using UnityEngine;

namespace Dissertation.Util
{
	public static class Layers
	{
		public static readonly int Default = LayerMask.NameToLayer("Default");
		public static readonly int Water = LayerMask.NameToLayer("Water");
		public static readonly int UI = LayerMask.NameToLayer("UI");
		public static readonly int Ground = LayerMask.NameToLayer("Ground");
	}
}
