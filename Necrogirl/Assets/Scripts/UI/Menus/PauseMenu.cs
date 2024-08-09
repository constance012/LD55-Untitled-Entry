using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	[Header("Tweening Effect"), Space]
	[SerializeField] private TweenableUIElement tweenable;

	[Header("Shopkeeper Speech"), Space]
	[SerializeField] private ShopkeeperSpeech shopkeeper;

	// Properties.
	public static bool IsPaused { get; private set; }

	private void Update()
	{
		if (LegacyInputManager.Instance.GetKeyDown(KeybindingActions.Pause))
			TogglePausing(!IsPaused);
	}

	#region Callback Methods for UI.
	public void ReturnToMenu()
	{
		TogglePausing(false);
		SceneManager.LoadSceneAsync("Scenes/Menu");
		AudioManager.Instance.Stop("Ambience Noise");
	}

	public void TogglePausing(bool pause)
	{
		IsPaused = pause;
		Inventory.Instance.ToggleActive(false);
		Time.timeScale = IsPaused ? 0f : 1f;
		
		tweenable.SetActive(IsPaused);
		shopkeeper.ToggleAnimatingText(IsPaused);
	}
	#endregion
}