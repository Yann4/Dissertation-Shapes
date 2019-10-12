using Dissertation.Character.AI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dissertation
{
	public class App : MonoBehaviour
	{
		[SerializeField] private string _HUDSceneName = "HUD";
		[SerializeField] private string _whiteboxScene = "Whitebox";

		public static bool Paused { get { return _pause > 0; } }
		private static int _pause = 0;
		private static float _timeScale;

		public static Blackboard AIBlackboard { get; private set; }

		private void Start()
		{
			AIBlackboard = new Blackboard();

			LoadScene(_HUDSceneName);
			LoadScene(_whiteboxScene);

			_timeScale = Time.timeScale;
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
	}
}