#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using static Dissertation.Narrative.ActionFunctionLibrary;
using System.Collections.Generic;

namespace Dissertation.Narrative.Generator
{
	public static class GeneratorUtils
	{
		private const string GeneratedAssetsPath = "Assets/Data/Narrative/Generated";
		private const string WorldPropertyPath = GeneratedAssetsPath + "/Properties";
		private const string ActionPath = GeneratedAssetsPath + "/Actions";

		public static bool CreateWorldPropertyAsset(WorldProperty property, string fileName)
		{
			if(!fileName.EndsWith(".asset"))
			{
				fileName += ".asset";
			}

			if(DoesAssetExist(fileName, WorldPropertyPath))
			{
				Debug.LogErrorFormat("Already created asset with name {0}", fileName);
				return false;
			}

			WorldPropertyScriptable scriptable = ScriptableObject.CreateInstance<WorldPropertyScriptable>();
			scriptable.FromRuntimeProperty(property, true);

			AssetDatabase.CreateAsset(scriptable, string.Format("{0}/{1}", WorldPropertyPath, fileName));

			NarrativeDictionary.GetAsset().AddWorldProperty(scriptable);
			return true;
		}

		public static bool CreateActionAsset(List<WorldPropertyScriptable> preconditions, List<WorldPropertyScriptable> postConditions,
			List<WorldPropertyScriptable> exitConditions, Actions perform, 
			string fileName)
		{
			if (!fileName.EndsWith(".asset"))
			{
				fileName += ".asset";
			}

			if (DoesAssetExist(fileName, WorldPropertyPath))
			{
				Debug.LogErrorFormat("Already created asset with name {0}", fileName);
				return false;
			}

			Action action = ScriptableObject.CreateInstance<Action>();
			action.Preconditions = preconditions;
			action.Postconditions = postConditions;
			action.ExitConditions = exitConditions;
			action.PerformFunction = perform;
			action.guid = GUID.Generate().ToString();

			AssetDatabase.CreateAsset(action, string.Format("{0}/{1}", ActionPath, fileName));
			NarrativeDictionary.GetAsset().AddAction(action);
			return true;
		}

		public static void DeleteAllGeneratedAssets()
		{
			DeleteAllAssets<WorldPropertyScriptable>(WorldPropertyPath);
			DeleteAllAssets<Action>(ActionPath);
		}

		private static void DeleteAllAssets<T>(string assetLocation) where T : ScriptableObject
		{
			NarrativeDictionary dictionary = NarrativeDictionary.GetAsset();
			string[] assets = AssetDatabase.FindAssets("t:ScriptableObject", new string[] { assetLocation });

			foreach (string guid in assets)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guid);
				T scriptable = AssetDatabase.LoadAssetAtPath<T>(assetPath);
				dictionary.Remove(scriptable);
				AssetDatabase.DeleteAsset(assetPath);
			}
		}

		private static bool DoesAssetExist(string fileName, string path)
		{
			string[] assets = AssetDatabase.FindAssets(fileName + " t:ScriptableObject", new string[] { path });

			return Array.Find(assets, guid => AssetDatabase.GUIDToAssetPath(guid).EndsWith(fileName)) != null;
		}
	}
}
#endif //UNITY_EDITOR