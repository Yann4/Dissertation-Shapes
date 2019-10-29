using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Util.Localisation
{
	[CreateAssetMenu(fileName = "Locstrings.asset", menuName = "Dissertation/Scriptables/Localisation")]
	public class LocalisationScriptable : ScriptableObject
	{
		[Serializable]
		public struct Locstring
		{
			[SerializeField] private string _key;
			[SerializeField] private string _english;

			public string Key { get { return _key; } }
			public string Translation { get { return _english; } }

			public override int GetHashCode()
			{
				return Key.GetHashCode();
			}
		}

		[SerializeField] private List<Locstring> _locstrings;

		private Dictionary<int, Locstring> _runTime = new Dictionary<int, Locstring>();
		public void Setup()
		{
			foreach(Locstring locstring in _locstrings)
			{
				Debug.Assert(!_runTime.ContainsKey(locstring.GetHashCode()), "Can't have duplicate keys - " + locstring.Key);
				_runTime.Add(locstring.GetHashCode(), locstring);
			}
		}

		public string GetTranslation(string Key)
		{
			if(_runTime.TryGetValue(Key.GetHashCode(), out Locstring value))
			{
				return value.Translation;
			}
			else
			{
				return null;
			}
		}
	}
}