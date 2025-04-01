using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StorySceneManager : MonoBehaviour {
	[SerializeField] private GameDialogueController dialogueController;
	[SerializeField] private TextMeshProUGUI dialogueTextField;
	[SerializeField] private List<DialogueBox> dialogueBoxes;
	[SerializeField] private GameObject storyBg;
	[SerializeField] private GameObject skipButton;
	[SerializeField] private Image bgImage;
	[SerializeField] private float pacingDelayDuration;
	[SerializeField] private float alphaFade = 0.8f;

	private WaitForSeconds pacingDelay;
	private string[] introDialogue;
	private bool finalDialogueFinished;

	private void Start() {
		ClearDialogueBoxes();
		int firstTimeLaunching = 1;// PlayerPrefs.GetInt("FirstTimeLaunching", 1);
		if (firstTimeLaunching != 0) {
			PlayerPrefs.SetInt("FirstTimeLaunching", 0);
			StartIntro();
		}
		else {
			storyBg.SetActive(false);
		}

		ClearDialogueBoxes();

		pacingDelay = new WaitForSeconds(pacingDelayDuration);
		introDialogue = new string[7]; // yeah yeah, this is not the best way to do this, i'm not going to make an entire scriptable object for text i'm not changing/localizing anyway
		introDialogue[0] = "Not long ago, word arose of a marvelous treasure said to be hidden in a cave of some sort...";
		introDialogue[1] = "It further warned of a fierce dragon, said to be guarding this treasure at all costs.";
		introDialogue[2] = "The allure of gold and gemstones proved too great to resist.\n\nThe decision to set out was swiftly made.";
		introDialogue[3] = "'Cross the fields, 'cross the mountains you ventured until the unassuming entrance was found.";
		introDialogue[4] = "You balled your fists and took a deep breath.\n\nWith lit torch in hand, you stepped inside.";
		introDialogue[5] = "As soon as the cave had you in its grasp, however rocks tumbled down behind, trapping you within!";
		introDialogue[6] = "With no way back you pressed on into the dark.\n\nDeeper into the...";

		introDialogue = new string[5];
		introDialogue[0] = "Why, hello there chum! Seems you picked a bad time to get lost.";
		introDialogue[1] = "Hmmm? \"Fierce dragon\", you say? Oh don't you worry, chum. Those days are long since behind me.";
		introDialogue[2] = "I take it you're here for the treasure, hmm? Well, I'd let you take some but the thing is I can't.";
		introDialogue[3] = "No, you see, the treasure is cursed, binding it down here. Although I suppose you could help with that.";
		introDialogue[4] = "Follow me and I'll show you how.";
	}

	private void StartIntro() {
		IEnumerator coroutine = TellIntroStory();
		StartCoroutine(coroutine);
	}

	private IEnumerator TellIntroStory() {
		yield return new WaitForSeconds(1f);

		int dialogueIndex = 0;
		for (int i = 0; i < introDialogue.Length - 1; i++) {
			yield return dialogueBoxes[dialogueIndex % dialogueBoxes.Count].AnimateDialogueLine(introDialogue[i], null);
			dialogueIndex++;
			if (dialogueIndex % 3 == 0) {
				ClearDialogueBoxes();
			} else {
				yield return pacingDelay;
			}
		}

		yield return dialogueBoxes[1].AnimateDialogueLine(introDialogue[6], null);
		yield return pacingDelay;

		yield return CloseScene();

		yield return pacingDelay;

		yield return LetHimCook();

	}

	private IEnumerator CloseScene() {
		ClearDialogueBoxes();
		skipButton.SetActive(false);
		yield return FadeBackground();
	}

	private IEnumerator FadeBackground() {
		float alphaCounter = 1;
		while (alphaCounter > 0) {
			alphaCounter -= Time.deltaTime * alphaFade;
			bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, alphaCounter);
			yield return null;
		}
		gameObject.SetActive(false);
	}

	private IEnumerator LetHimCook() {
		//dialogueController.
		yield return null;
	}

	private void ClearDialogueBoxes() {
		foreach (var box in dialogueBoxes) {
			box.Clear();
		}
	}

	public void ResetFirstTimeFlag() {
		PlayerPrefs.SetInt("FirstTimeLaunching", 1);
	}

	public void SkipIntro() {
		StopAllCoroutines();
		StartCoroutine(CloseScene());
	}
}