using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	[Header("UI References"), Space]
	[SerializeField] private HealthBar playerHealthBar;
	[SerializeField] private GameObject summaryScreen;
	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private TextMeshProUGUI coinCollectedText;
	[SerializeField] private TextMeshProUGUI enemyCountText;

	[Header("Containers"), Space]
	[SerializeField] private Transform enemyContainer;

	// Properties.
	public bool GameFinished { get; private set; }

	// Private fields.
	private int _maxEnemies;

	private IEnumerator Start()
	{
		yield return new WaitForSecondsRealtime(.1f);
		_maxEnemies = enemyContainer.childCount;
	}

	private void Update()
	{
		if (enemyContainer.childCount == 0)
		{
			ShowVictoryScreen();
			return;
		}

		if (InputManager.Instance.GetKeyDown(KeybindingActions.Pause))
			Pause();
	}

	private void LateUpdate()
	{
		enemyCountText.text = $"{enemyContainer.childCount} / {_maxEnemies}";
	}

	public void UpdateCurrentHealth(float currentHP)
	{
		playerHealthBar.SetCurrentHealth(currentHP);
	}

	public void InitializeHealthBar(float initialHP)
	{
		playerHealthBar.SetMaxHealth(initialHP);
	}

	/// <summary>
	/// Callback method for the retry button.
	/// </summary>
	public void RestartGame()
	{
		GameFinished = false;

		SceneManager.LoadSceneAsync("Scenes/Main Game");
	}

	public void Pause()
	{
		Time.timeScale = 0f;
		pauseMenu.SetActive(true);
	}

	public void ShowGameOverScreen()
	{
		GameFinished = true;

		summaryScreen.SetActive(true);
		summaryScreen.transform.Find("Panel/Game Over Text").gameObject.SetActive(true);
		summaryScreen.transform.Find("Panel/Victory Text").gameObject.SetActive(false);

		coinCollectedText.text = $"You've collected <color=#C39F4C>{ItemsManager.Instance.Coins}";
	}

	public void ShowVictoryScreen()
	{
		GameFinished = true;

		summaryScreen.SetActive(true);
		summaryScreen.transform.Find("Panel/Game Over Text").gameObject.SetActive(false);
		summaryScreen.transform.Find("Panel/Victory Text").gameObject.SetActive(true);

		coinCollectedText.text = $"You've collected <color=#C39F4C>{ItemsManager.Instance.Coins}";
	}
}