using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour { // script that contains all the necessary references for starting/playing through the tutorial
	[SerializeField] private List<TutorialScenario> scenarios;
	[SerializeField] private GameDialogueController dialogueManager;
	[SerializeField] private ControlInstructionAnimator instructionAnimator;
	[SerializeField] private DialogueBox dialogueBox;

	private TutorialScenario currentScenario;
	private ScreenTransitionAnimation curtain;
	private IEnumerator tutorialCoroutine;
	private IEnumerator gloveAnimationRoutine;
	private string sceneToReturnTo;

	public void StartTutorial(string sceneToReturnTo) {
		curtain = GameManager.Instance.CurtainAnimation;
		this.sceneToReturnTo = sceneToReturnTo;
		tutorialCoroutine = ProcessScenarios();
		StartCoroutine(tutorialCoroutine);
	}

	private IEnumerator ProcessScenarios() {
		foreach (var scenario in scenarios) {
			currentScenario = scenario;
			yield return currentScenario.ProcessScenario(this);
		}

		GameManager.Instance.LoadSceneByName(sceneToReturnTo);
	}

	public void PointGloveAnimation(int pos) {
		instructionAnimator.PlayPointAnimation(pos);
	}

	public void DragGloveAnimation(Vector3 from, Vector3 to) {
		instructionAnimator.PlayDragAnimation(from, to);
	}

	public void StopGloveAnimation() {
		instructionAnimator.StopAnimating();
	}

	public IEnumerator DrawOrCloseCurtain(bool bottomToTop, bool animateInward) {
		yield return curtain.PlayAnimation(bottomToTop, animateInward);
	}

	public void ChangeScreenCoverState(bool open) {
		curtain.CoverScreen(open);
	}

	public IEnumerator StartDialogue(string[] dialogue) {
		yield return dialogueManager.AnimateTextCoroutine(dialogue);
	}

	public ObjectiveTracker RetrieveObjectiveTracker() {
		return GridManager.Instance.ObjectiveTracker;
	}
}
