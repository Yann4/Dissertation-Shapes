using UnityEngine;

namespace Dissertation.Util
{
	public static class Layers
	{
		public static readonly int Default = LayerMask.NameToLayer("Default");
		public static readonly int DefaultMask = LayerMask.GetMask("Default");
		public static readonly int Water = LayerMask.NameToLayer("Water");
		public static readonly int WaterMask = LayerMask.GetMask("Water");
		public static readonly int UI = LayerMask.NameToLayer("UI");
		public static readonly int UIMask = LayerMask.GetMask("UI");
		public static readonly int Ground = LayerMask.NameToLayer("Ground");
		public static readonly int GroundMask = LayerMask.GetMask("Ground");
		public static readonly int Player = LayerMask.NameToLayer("Player");
		public static readonly int PlayerMask = LayerMask.GetMask("Player");
		public static readonly int Agent = LayerMask.NameToLayer("Agent");
		public static readonly int AgentMask = LayerMask.GetMask("Agent");
		public static readonly int DamageSource = LayerMask.NameToLayer("Damage Source");
		public static readonly int DamageSourceMask = LayerMask.GetMask("Damage Source");
		public static readonly int Interactable = LayerMask.NameToLayer("Interactable");
		public static readonly int InteractableMask = LayerMask.GetMask("Interactable");
	}
}
