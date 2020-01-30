using Dissertation.Character.AI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Dissertation.Util.Localisation;
using Dissertation.Character;
using Dissertation.Util;
using System.Collections;
using Dissertation.Narrative;
using Dissertation.Narrative.Generator;
using Dissertation.Environment;

namespace Dissertation
{
	public class App : MonoBehaviour
	{
		[SerializeField] private string _HUDSceneName = "HUD";
		[SerializeField] private string _whiteboxScene = "Whitebox";

		[SerializeField] private LocalisationScriptable _localisation;
		[SerializeField] private ConversationData _conversations;
		[SerializeField] private TextAsset _beatGraph;
		[SerializeField] private BeatTemplates _templates;

		[SerializeField] private WorldConfig _worldConfig;
		[SerializeField] private bool _loadWhitebox = false;

		public static bool Paused { get { return _pause > 0; } }
		private static int _pause = 0;
		private static float _timeScale;

		public static Blackboard AIBlackboard { get; private set; }

		public static WorldStateManager WorldState { get; private set; }
		private static NarrativePlanner Planner;

#if UNITY_EDITOR
		public static NarrativeGenerator Generator { get; private set; }
#endif //UNITY_EDITOR

		public static System.Action OnLevelLoaded;

		private IEnumerator Start()
		{
			new LocManager(_localisation); //Need to create the instance, but don't need to worry about holding a reference

			AIBlackboard = new Blackboard( _conversations );

			WorldState = new WorldStateManager();
			Planner = new NarrativePlanner(WorldState, this, _beatGraph);

#if UNITY_EDITOR
			Generator = new NarrativeGenerator(_beatGraph, _templates, WorldState);
#endif //UNITY_EDITOR

			yield return LoadScene(_HUDSceneName);

			if (_loadWhitebox || _worldConfig == null)
			{
				yield return LoadScene(_whiteboxScene);
			}
			else
			{
				foreach(WorldConfig.SceneLocation scene in _worldConfig.Scenes)
				{
					yield return LoadScene(scene.WorldSceneName);

					Scene loadedScene = SceneManager.GetSceneByName(scene.WorldSceneName);
					foreach(GameObject sceneRoot in loadedScene.GetRootGameObjects())
					{
						sceneRoot.transform.position += scene.Offset;
					}
				}
			}

			StartCoroutine(InvokeLevelLoaded());

			_timeScale = Time.timeScale;
		}

		private IEnumerator InvokeLevelLoaded()
		{
			yield return null;
			OnLevelLoaded.InvokeSafe();

			Planner.Enable();
		}

		private IEnumerator LoadScene(string sceneName)
		{
			Scene scene = SceneManager.GetSceneByName(sceneName);
			if (scene != null && !scene.isLoaded)
			{
				yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			}
		}

		private void UnloadScene(string scene)
		{
			SceneManager.UnloadSceneAsync(scene);
		}

		public static void Pause()
		{
			if (!Paused && _pause + 1 > 0)
			{
				_timeScale = Time.timeScale;
				Time.timeScale = 0;
			}

			_pause++;
		}

		public static void Resume()
		{
			if(Paused && _pause - 1 == 0)
			{
				Time.timeScale = _timeScale;
			}

			_pause--;
			Debug.Assert(_pause >= 0);
		}

		public static void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		public void Update()
		{
			Planner.Update();
		}

		public static void LogPlan()
		{
			Debug.Log(Planner.CurrentPlan());
		}
	}
}