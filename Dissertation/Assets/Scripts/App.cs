using Dissertation.Character.AI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Dissertation.Util.Localisation;
using Dissertation.Character;
using Dissertation.Util;
using System;
using System.Collections;

namespace Dissertation
{
	public class App : MonoBehaviour
	{
		[SerializeField] private string _HUDSceneName = "HUD";
		[SerializeField] private string _whiteboxScene = "Whitebox";

		[SerializeField] private LocalisationScriptable _localisation;
		[SerializeField] private ConversationData _conversations;

		public static bool Paused { get { return _pause > 0; } }
		private static int _pause = 0;
		private static float _timeScale;

		public static Blackboard AIBlackboard { get; private set; }

		public static Action OnLevelLoaded;

		private void Start()
		{
			new LocManager(_localisation); //Need to create the instance, but don't need to worry about holding a reference

			AIBlackboard = new Blackboard( _conversations );

			LoadScene(_HUDSceneName);
			LoadScene(_whiteboxScene);

			StartCoroutine(InvokeLevelLoaded());

			_timeScale = Time.timeScale;
		}

		private IEnumerator InvokeLevelLoaded()
		{
			yield return null;
			OnLevelLoaded.InvokeSafe();
		}

		private void LoadScene(string sceneName)
		{
			Scene scene = SceneManager.GetSceneByName(sceneName);
			if (scene != null && !scene.isLoaded)
			{
				SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
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
	}
}