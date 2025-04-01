using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GameDialogueController : MonoBehaviour {
	[SerializeField] private DialogueBox dialogueBox;
	[SerializeField] private Transform characterModel;
	[SerializeField] private TextMeshProUGUI dialogueText;
	[SerializeField] private AnimationCurve appearTweenCurve;
	[SerializeField] private RectTransform dialogueBackground;
	[SerializeField] private List<Button> buttonsToDisable;
	[SerializeField] private float modelTweenDuration = 0.35f;
	[SerializeField] private float modelInX;
	[SerializeField] private float modelOutX;

	private Animator characterController;
	private bool visible;
	private float bgStartWidth = 70f;
	private float bgEndWidth;
	private float bgStartHeight = 70f;
	private float bgEndHeight;
	private float bgTweenDuration = 0.5f;

	private string[] helpDialogue;

	private void Start() {
		characterController = characterModel.GetComponent<Animator>();
		bgEndWidth = dialogueBackground.sizeDelta.x;
		bgEndHeight = dialogueBackground.sizeDelta.y;

		helpDialogue = new string[7];
		helpDialogue[0] = "My my, having trouble there, chum? Let me give you the rundown.";
		helpDialogue[1] = "These gemstones here are the treasure that needs sorting. I've handed you a stack of them that you can place anywhere you like, given the tile is empty!";
		helpDialogue[2] = "Placing gems of the same colour next to each other will make them sort themselves.";
		helpDialogue[3] = "Careful, however, if there's no more space on the board left to place the gems, you lose, and you'll have to start over!";
		helpDialogue[4] = "Stacks of gems bound by cursed chains cannot move on their own! They can only be freed by putting similarly coloured gems next to it. Doing so dispells the curse and allows them to move freely.";
		helpDialogue[5] = "Lastly, the top right shows the amount of gems you'll need to clear from the board to finish the level!";
		helpDialogue[6] = "Good luck, chum.";
	}

	private void SetSpeakingBool(bool value) {
		characterController.SetBool("speaking", value);
	}

	public void AnimateText(string[] paragraphText) {
		foreach (var button in buttonsToDisable) {
			button.interactable = false;
		}
		var coroutine = AnimateTextCoroutine(paragraphText);
		StartCoroutine(coroutine);
	}

	public void PlayDebugTutorialDialogue() {
		AnimateText(helpDialogue);
	}

	private IEnumerator TweenIn() {
		if (visible) yield break;
		StartCoroutine(TweenModel(true));
		StartCoroutine(TweenDialogueBox(true));
		visible = true;
	}

	private IEnumerator TweenOut() {
		if (!visible) yield break;
		StartCoroutine(TweenModel(false));
		StartCoroutine(TweenDialogueBox(false));
		yield return null;

		visible = false;
		foreach (var button in buttonsToDisable) {
			button.interactable = true;
		}
	}

	private IEnumerator TweenModel(bool tweenIn) {
		Vector3 modelStart = new Vector3(tweenIn ? modelOutX : modelInX, characterModel.position.y, characterModel.position.z);
		Vector3 modelGoal = new Vector3(tweenIn ? modelInX : modelOutX, characterModel.position.y, characterModel.position.z);
		float counter = 0;
		while (counter < modelTweenDuration) {
			characterModel.transform.position = Vector3.Lerp(modelStart, modelGoal, appearTweenCurve.Evaluate(counter / modelTweenDuration));
			counter += Time.deltaTime;
			yield return null;
		}

		characterModel.transform.position = modelGoal;
	}

	private IEnumerator TweenDialogueBox(bool tweenIn) {
		float counter = 0;
		Vector2 boxSizeStart = new Vector2(tweenIn ? bgStartWidth : bgEndWidth, tweenIn ? bgStartHeight : bgEndHeight);
		Vector2 boxSizeEnd = new Vector2(tweenIn ? bgEndWidth : bgStartWidth, tweenIn ? bgEndHeight : bgStartHeight);
		var rect = dialogueBackground.gameObject.GetComponent<RectTransform>();
		rect.sizeDelta = boxSizeStart;

		if (tweenIn) {
			dialogueBackground.gameObject.SetActive(true);
		} else {
			dialogueText.gameObject.SetActive(false);
		}

		while (counter < bgTweenDuration) {
			rect.sizeDelta = Vector2.Lerp(boxSizeStart, boxSizeEnd, appearTweenCurve.Evaluate(counter / bgTweenDuration));
			counter += Time.deltaTime;
			yield return null;
		}

		if (tweenIn) {
			dialogueText.gameObject.SetActive(true);
		} else {
			dialogueBackground.gameObject.SetActive(false);
		}

		yield return null;
	}

	public IEnumerator AnimateTextCoroutine(string[] paragraphText) {
		yield return TweenIn();
		for (int i = 0; i < paragraphText.Length; i++) {
			yield return dialogueBox.AnimateDialogueLine(paragraphText[i], SetSpeakingBool);
		}

		StartCoroutine(TweenOut());
	}
}
