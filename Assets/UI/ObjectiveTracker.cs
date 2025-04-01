using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTracker : MonoBehaviour {
	[SerializeField] private GameObject objectiveCardPrefab;
	[SerializeField] private MatchResolver resolverScript;
	[SerializeField] private StarDisplay starDisplay;
	[SerializeField] private FillBar comboFillBar;
	[SerializeField] private RectTransform bgGraphicTransform;
	[SerializeField] private Sprite chipBaseSprite;
	[SerializeField] private List<Color> colours;
	[SerializeField] private Sprite fallbackStarSprite;

	private Dictionary<ChipType, ObjectiveCard> trackedObjectives;
	private bool[] starData;
	private bool comboFilled;
	private bool doneMatching = false;
	private int numberOfTrackedObjectives;
	private bool tutorialMode;

	private static int OBJECTED_CARD_INIT_OFFSET = -170;
	private static int OBJECTIVE_CARD_OFFSET = 200;
	private static int BG_WIDTH = 80;
	private static int BG_HEIGHT_PER_CARD = 80;
	private static int COMBO_SCORE_REQUIREMENT = 8;

	public bool[] StarData { get { return starData; } }
	public Vector3 ObjectiveCardBgPos { get { return bgGraphicTransform.position; } }

	private void Awake() {
		trackedObjectives = new Dictionary<ChipType, ObjectiveCard>();
	}

	public void Setup(List<TrackedObjective> objectivesToAdd, Sprite starImage, bool[] starData) {
		foreach (var objective in objectivesToAdd) {
			switch (objective.objectiveType) {
				case ObjectiveType.ColouredChipQuota:
					CreateChipQuotaTracker(objective.chipType, objective.requiredAmount);
					break;

				case ObjectiveType.EntityOnBoard:
					CreateEntityTracker();
					break;

				default:
					break;
			}
		}

		this.starData = starData;
		starDisplay.SetImage(starImage ? starImage : fallbackStarSprite);
		starDisplay.UpdateDisplay(starData);
		comboFillBar.Setup(COMBO_SCORE_REQUIREMENT);
	}

	private void CreateChipQuotaTracker(ChipType type, int amount) {
		if (!trackedObjectives.ContainsKey(type)) {
			var obj = Instantiate(objectiveCardPrefab, this.transform);
			var rect = obj.GetComponent<RectTransform>();
			var card = obj.GetComponent<ObjectiveCard>();
			rect.localPosition = new Vector3(0, OBJECTED_CARD_INIT_OFFSET - OBJECTIVE_CARD_OFFSET * numberOfTrackedObjectives, 0);
			bgGraphicTransform.sizeDelta = new Vector2(BG_WIDTH, BG_HEIGHT_PER_CARD + BG_HEIGHT_PER_CARD * numberOfTrackedObjectives);

			card.SetImage(chipBaseSprite, colours[(int)type - 1]);
			card.SetCounterGoal(amount);
			card.AddToCounterAndUpdateText(0);

			trackedObjectives.Add(type, card);
			numberOfTrackedObjectives++;
		}
	}

	private void CreateEntityTracker() {
		// create ui element
		// create entity and add it
	}

	public void RegisterMatch(ChipType type, int amount) {
		if (!trackedObjectives.ContainsKey(type)) return;
		trackedObjectives[type].AddToCounterAndUpdateText(amount);
		if (!tutorialMode) CheckObjectiveCompletion();
	}

	private void CheckObjectiveCompletion() {
		foreach (var objective in trackedObjectives) {
			if (!objective.Value.IsCompleted) {
				return;
			}
		}

		AwardObjectiveStar();
		if (resolverScript.Matching) {
			IEnumerator coroutine = HoldCompletionUntilMatched();
			StartCoroutine(coroutine);
		}
		else {
			GameManager.Instance.EndLevel(true);
		}
	}

	private IEnumerator HoldCompletionUntilMatched() {
		while (!doneMatching) {
			yield return null;
		}

		yield return new WaitForSeconds(0.5f);
		GameManager.Instance.EndLevel(true);
	}

	private void SetCompletionFlag(bool matchesFound) {
		if (!matchesFound) doneMatching = true;
	}

	public void AddToCombo(int amount) {
		if (comboFilled) return;
		comboFillBar.AddIncrements((int)Mathf.Pow(amount, 2), out comboFilled);
		if (comboFilled) AwardComboStar();
	}

	private void AwardCurseStar() {
		print("curse star awarded");
		starData[0] = true; 
		starDisplay.UpdateDisplay(starData);
	}

	private void AwardObjectiveStar() {
		starData[1] = true;
		starDisplay.UpdateDisplay(starData);
	}

	private void AwardComboStar() {
		starData[2] = true;
		starDisplay.UpdateDisplay(starData);
	}

	public void SetTutorialFlag() {
		tutorialMode = true;
	}

	private void OnEnable() {
		GridManager.OnCurseClear += AwardCurseStar;
		ComboSystem.onComboPayout += AddToCombo;
		MatchResolver.BoardEvaluated += SetCompletionFlag;
	}

	private void OnDisable() {
		GridManager.OnCurseClear -= AwardCurseStar;
		ComboSystem.onComboPayout -= AddToCombo;
		MatchResolver.BoardEvaluated -= SetCompletionFlag;
	}
}

[System.Serializable]
public struct TrackedObjective {
	public ObjectiveType objectiveType;
	public ChipType chipType;
	public int requiredAmount;
	public int currentAmount;

	public TrackedObjective(ObjectiveType objectiveType, ChipType chipType, int amount) {
		this.objectiveType = objectiveType;
		this.chipType = chipType;
		requiredAmount = amount;
		currentAmount = 0;
	}
}

public enum ObjectiveType {
	ColouredChipQuota,
	EntityOnBoard
}
