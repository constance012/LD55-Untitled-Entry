using System.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using TMPro;

public sealed class ShopkeeperSpeech : MonoBehaviour
{
	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI dialoguesText;

	[Header("Animating Text Settings"), Space]
	[SerializeField, Min(10f)] private float textSpeed;
	[SerializeField] private bool useTypingSound;
	[SerializeField] private bool persistentSound;
	[SerializeField] private Vector2 pitchRange;
	[SerializeField, Min(1)] private int typingSoundFrequency;

	// Private fields.
	private readonly string[] _sentences = new string[]
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
	
	private CancellationTokenSource _cts;

	#region Shopkeeper's Dialogue.
	public async void ToggleAnimatingText(bool state)
	{
		if (state)
		{
			_cts = new CancellationTokenSource();
			await AnimateText();
		}
		else
			_cts?.Cancel();
	}

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

	private async Task AnimateText()
	{
		int index = UnityRandom.Range(0, _sentences.Length);
		string sentence = _sentences[index];
		int delay = (int)(1f / textSpeed * 1000f);

		dialoguesText.text = sentence;
		dialoguesText.maxVisibleCharacters = 0;

		try
		{
			foreach (char _ in sentence)
			{
				_cts.Token.ThrowIfCancellationRequested();

				PlayTypingSound(dialoguesText.maxVisibleCharacters);
				dialoguesText.maxVisibleCharacters++;
				await Task.Delay(delay);
			}
		}
		catch (OperationCanceledException)
		{
			Debug.Log("Shopkeeper text animation canceled.");
		}
	}
	#endregion
}