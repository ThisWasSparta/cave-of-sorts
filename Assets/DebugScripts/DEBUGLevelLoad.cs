using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DEBUGLevelLoad : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI indexTextObject;
	[SerializeField] private LevelContentData contentData;
	[SerializeField] private LevelGridData gridData;

	[SerializeField] private TMP_Dropdown dealerLevelDropdown;
	[SerializeField] private TMP_Dropdown levelSizeDropdown;

	[SerializeField] private Sprite starImage;

	[SerializeField] private AnimationCurve entranceAnimationCurve;

	[SerializeField] private float entranceAnimationDuration = 1.5f;

	private Camera mainCam;

	private const float GOAL_FOV = 1;

	private void Start() {
		mainCam = Camera.main;
	}

	public void OnClick() {
		GameManager.Instance.TransitionToAndLoadLevel(contentData, gridData, "6969", dealerLevelDropdown.value, new bool[3], starImage);
	}

	public void OnClickStart() {
		IEnumerator coroutine = PlayStartAnimation();
		StartCoroutine(coroutine);
	}

	private IEnumerator PlayStartAnimation() {
		yield return new WaitForSeconds(0.4f);

		float timer = 0;
		float currentFov = mainCam.fieldOfView;

		while (timer < entranceAnimationDuration) {
			timer += Time.deltaTime;
			float animationProgress = timer / entranceAnimationDuration;

			mainCam.fieldOfView = Mathf.Lerp(currentFov, GOAL_FOV, entranceAnimationCurve.Evaluate(animationProgress));
			yield return null;
		}
		GameManager.Instance.TransitionToWorld(0);
	}

	public void NukeSaveFile() {
		File.Delete("SaveFile/save.bin");
		print("i am become death, the destroyer of worlds...");
	}

	public void LoadLevel() {
		GameManager.Instance.TransitionToAndLoadLevel(contentData, gridData, "", 1, new bool[3], starImage);
	}

	public void StartTutorial() {
		GameManager.Instance.TransitionToAndBeginTutorial("MainMenuScene");
	}
}
