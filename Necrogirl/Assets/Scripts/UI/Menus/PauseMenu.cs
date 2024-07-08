using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI dialoguesText;
	[SerializeField] private TweenableUIElement tweenable;

	[Header("Animating Text Settings"), Space]
	[SerializeField] private float textSpeed;
	[SerializeField] private bool useTypingSound;
	[SerializeField] private bool persistentSound;
	[SerializeField] private Vector2 pitchRange;
	[SerializeField] private int typingSoundFrequency;

	// Private fields.
	private bool _isPaused;

	private readonly string[] sentences = new string[]
	{
		"Ah, settings! Where the magic happens. Or at least, where you can adjust the volume so your neighbors don't think you're running a rave in your basement.",
		"Back into the fray you go, brave adventurer! Remember, every step you take, every battle you fight, brings you closer to your ultimate destiny. And hey, if you ever need a pep talk, I'm always here in the Settings menu.",
		"Heading back to the main menu, eh? Well, remember, adventurer, every journey has a beginning and an end. But that doesn't mean the story is over. Just another chapter waiting to be written.",
		"Back to the title screen, I see. A moment of reflection before diving back into the adventure? Wise choice, young hero. Sometimes, it's good to pause, take a breath, and remember why we embark on these quests in the first place.",
		"Enjoying the Settings menu, are we? Well, make yourself at home! There's no rush to save the world. Sometimes, it's nice to just relax and let your mind wander. Who knows what brilliant ideas might pop up?",
		"Well, that was a delightful adventure! Until next time, adventurer, may your quests be grand and your rewards be legendary. And remember, the Settings menu is always here, a friendly face in a digital world.",
		"Life is like a box of chocolates. You never know what you're gonna get!",
		"Love is like a potato. It's versatile, it can be cooked in many ways, and it's always there for you.",
		"Yui is my favorite character in K-On!",
		"I'm training to be a Hokage, the strongest ninja in the village! It's my dream to protect everyone.",
		"Sing along and let the music fill your soul!",
		"Remember, a comma can save a life!",
		"The only way to get rid of a temptation is to yield to it."
	};

	private void Update()
	{
		if (InputManager.Instance.GetKeyDown(KeybindingActions.Pause))
			TogglePausing(!_isPaused);
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
		_isPaused = pause;
		Time.timeScale = _isPaused ? 0f : 1f;
		
		tweenable.SetActive(_isPaused);
		StartCoroutine(AnimateText());
	}
	#endregion

	#region Shopkeeper's Dialogue.
	private void PlayTypingSound(int currentVisibleCharIndex)
	{
		if (!useTypingSound)
			return;

		if (currentVisibleCharIndex % typingSoundFrequency == 0)
		{
			if (persistentSound)
			{
				// Create the hash code for the current character.
				int hashCode = dialoguesText.text[currentVisibleCharIndex] + 200000;

				// Predict index for the sound clip.
				int index = hashCode % AudioManager.Instance.GetAudio("Dialogue Typing").ClipsCount;

				// Predict pitch.
				int minPitchInt = (int)(pitchRange.x * 100f);
				int maxPitchInt = (int)(pitchRange.y * 100f);
				int pitchRangeInt = maxPitchInt - minPitchInt;

				if (pitchRangeInt != 0)
				{
					float predictPitch = ((hashCode % pitchRangeInt) + minPitchInt) / 100f;
					AudioManager.Instance.Play("Dialogue Typing", index, predictPitch);
				}
				else
					AudioManager.Instance.Play("Dialogue Typing", index, pitchRange.x);
			}
			else
			{
				AudioManager.Instance.PlayWithRandomPitch("Dialogue Typing", pitchRange.x, pitchRange.y);
			}
		}
	}

	private IEnumerator AnimateText()
	{
		int index = Random.Range(0, sentences.Length);
		string sentence = sentences[index];
		float delay = 1f / textSpeed;

		dialoguesText.text = sentence;
		dialoguesText.maxVisibleCharacters = 0;

		foreach (char _ in sentence)
		{
			if (_isPaused)
			{
				PlayTypingSound(dialoguesText.maxVisibleCharacters);
				dialoguesText.maxVisibleCharacters++;
				yield return new WaitForSecondsRealtime(delay);
			}
		}
	}
	#endregion
}