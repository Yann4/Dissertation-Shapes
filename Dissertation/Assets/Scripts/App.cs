using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dissertation
{
	public class App : MonoBehaviour
	{
		[SerializeField] private string _HUDSceneName = "HUD";
		[SerializeField] private string _whiteboxScene = "Whitebox";

		private void Start()
		{
			LoadScene(_HUDSceneName);
			LoadScene(_whiteboxScene);
		}

		private void LoadScene(string scene)
		{
			if (SceneManager.GetSceneByName(scene) != null)
			{
				SceneManager.LoadScene(scene, LoadSceneMode.Additive);
			}
		}

		private void UnloadScene(string scene)
		{
			SceneManager.UnloadSceneAsync(scene);
		}
	}
}