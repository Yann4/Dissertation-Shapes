using System;
using System.Collections.Generic;
using UnityEngine;

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
			[Tooltip("Additional above the base in units/second")] public float FillRate;
		}

		public float Value { get; private set; }
		public float MaxValue { get; private set; }
		public float FillRate { get; private set; }

		private float _baseValue = 0.0f;
		private float _baseMaxValue = 10.0f;
		private float _baseFillRate = 0.5f / 60.0f;

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

		public void Update()
		{
			Value = Mathf.Min(MaxValue, Value + (FillRate * Time.deltaTime));
		}

		public void Reset()
		{
			Value = 0;
		}
	}
}