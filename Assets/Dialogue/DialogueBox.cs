using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour { // class responsible for printing text and awaiting/handling user input, expects an open/visible text field to be present
	[SerializeField] private TextMeshProUGUI dialogueTextField;
	[SerializeField] private float arrowTweenSpeedMult = 1.6f;
	[SerializeField] private float letterWaitDuration = 0.012f;
	[SerializeField] private float commaWaitDuration = 0.07f;
	[SerializeField] private float periodWaitDuration = 0.15f;
	[SerializeField] private float skipWaitDuration = 0.003f;
	[SerializeField] private float arrowBounceHeight = 25f;
	[SerializeField] private float stretchCorrectionValue = 0.8f;
	[SerializeField] private int smearingThreshold = 34; // depends on size of box, generally {characters that fit on a dialogue line} * 0.8

	[SerializeField] private RectTransform dialogueArrow;
	[SerializeField] private AnimationCurve arrowCurve;

	private System.Action<bool> onPrintStart;
	private WaitForSeconds letterWait;
	private WaitForSeconds commaWait;
	private WaitForSeconds periodWait;
	private WaitForSeconds skipWait;

	private float arrowStartWidth;
	private float arrowStartHeight;
	private bool spedUp;
	private bool skipped;
	private bool showArrow;

	private void Start() {
		letterWait = new WaitForSeconds(letterWaitDuration);
		commaWait = new WaitForSeconds(commaWaitDuration);
		periodWait = new WaitForSeconds(periodWaitDuration);
		skipWait = new WaitForSeconds(skipWaitDuration);

		if (dialogueArrow) {
			arrowStartWidth = dialogueArrow.position.x;
			arrowStartHeight = dialogueArrow.position.y;
			showArrow = true;
		}
	}

	private void Update() {
		if (Input.GetMouseButtonDown(0))
			if (!spedUp) spedUp = true;
			else skipped = true;
	}

	public IEnumerator AnimateDialogueLine(string text, System.Action<bool> printStateHandlerFunction) {
		Clear();
		dialogueTextField.text = text;
		dialogueTextField.ForceMeshUpdate(true, true);
		dialogueTextField.havePropertiesChanged = false;
		AdjustAlignmentPerLine(); 

		dialogueTextField.maxVisibleCharacters = 0;
		if (printStateHandlerFunction != null) onPrintStart = printStateHandlerFunction;
		spedUp = false;
		skipped = false;

		var chars = text.ToCharArray();
		onPrintStart?.Invoke(true);
		for (int i = 0; i < chars.Length; i++) {
			dialogueTextField.maxVisibleCharacters = i + 1;
			if (spedUp) {
				if (!skipped) yield return skipWait;
			}
			else {
				switch (chars[i]) {
					case '.':
					case '!':
					case '?':
						yield return periodWait;
						break;
					case ',':
						yield return commaWait;
						break;
					default:
						yield return letterWait;
						break;
				}
			}
			yield return null;
		}

		dialogueTextField.havePropertiesChanged = true;
		onPrintStart?.Invoke(false);

		if (showArrow) {
			yield return StartCoroutine(AnimateArrow());
		}
	}

	private IEnumerator AnimateArrow() {
		dialogueArrow.gameObject.SetActive(true);
		bool input = false;
		float counter = 0;
		while (!input) {
			counter += Time.deltaTime * arrowTweenSpeedMult;
			dialogueArrow.position = new Vector2(arrowStartWidth, arrowStartHeight + arrowCurve.Evaluate(counter) * arrowBounceHeight);
			if (Input.GetMouseButtonDown(0)) {
				input = true;
			}
			yield return null;
		}
		dialogueArrow.position = new Vector2(arrowStartWidth, arrowStartHeight);
		dialogueArrow.gameObject.SetActive(false);
	}

	private void AdjustAlignmentPerLine() { // to avoid text that comes out      printed      like      this      .
		var textInfo = dialogueTextField.textInfo;
		string adjustedText = "";
		bool justificationBroken = false;

		for (int i = 0; i < textInfo.lineCount; i++) {
			var lineInfo = textInfo.lineInfo[i];

			int startChar = lineInfo.firstCharacterIndex;
			int endChar = lineInfo.lastCharacterIndex;
			int charAmount = (endChar - startChar) + 1;

			string lineText = dialogueTextField.text.Substring(startChar, charAmount);

			if (charAmount < smearingThreshold) {
				adjustedText += $"<align=left>{lineText}";
				justificationBroken = true;
			}
			else if (justificationBroken) {
				adjustedText += $"<align=justified>{lineText}";
				justificationBroken = false;
			}
			else {
				adjustedText += lineText;
			}
		}

		dialogueTextField.text = adjustedText;
		dialogueTextField.ForceMeshUpdate();
	}

	public void Clear() {
		dialogueTextField.text = "";
		dialogueTextField.ForceMeshUpdate();
	}
}
