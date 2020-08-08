using UnityEngine;

namespace Dissertation.Util.Localisation
{
	public class LocManager
	{
		private static LocManager Instance = null;

		private LocalisationScriptable _data;
		public LocManager(LocalisationScriptable data)
		{
			Debug.Assert(data != null);
			_data = data;
			_data.Setup();
			Debug.Assert(Instance == null);
			Instance = this;
		}

		private string GetTranslation_Internal(string key)
		{
			key = key.Trim();
			string translation = _data.GetTranslation(key);
			if(translation == null)
			{
				Debug.LogErrorFormat("Couldn't find translation matching key '{0}'", key);
				return key;
			}

			return translation;
		}

		public static string GetTranslation(string key)
		{
			return Instance.GetTranslation_Internal(key);
		}
	}
}