using System;
using System.Collections.Generic;

namespace Dissertation.Character.AI
{
	public enum DesireType
	{
		Money,
		Safety,
		Power,
		COUNT
	}

	public class Desire
	{
		[Serializable]
		public struct Modifier
		{
			public DesireType ToModify;
			public float MaximumValue;
			public float FillRate;
		}

		public float Value { get; private set; }
		public float MaxValue { get; private set; }
		public float FillRate { get; private set; }

		private float _baseValue = 0.0f;
		private float _baseMaxValue = 10.0f;
		private float _baseFillRate = 0.1f;

		public DesireType Type { get; private set; }

		private List<Modifier> _modifiers = new List<Modifier>();

		public Desire(DesireType type)
		{
			Type = type;

			Recalculate();
		}

		public void ApplyModifier(Modifier modifier)
		{
			_modifiers.Add(modifier);
			Recalculate();
		}

		public void RemoveModifier(Modifier modifier)
		{
			if(_modifiers.Remove(modifier))
			{
				Recalculate();
			}
		}

		private void Recalculate()
		{
			Value = _baseValue;
			FillRate = _baseFillRate;
			MaxValue = _baseMaxValue;

			foreach(Modifier modifier in _modifiers)
			{
				FillRate += modifier.FillRate;
				MaxValue += modifier.MaximumValue;
			}
		}
	}
}