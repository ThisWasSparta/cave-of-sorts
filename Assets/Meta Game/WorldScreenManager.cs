using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldScreenManager : MonoBehaviour {
	[Header("Input")]
	[SerializeField] private WorldScreenInputSystem inputSystem;

	[Header("UI")]
	[SerializeField] private TextMeshProUGUI worldHeader;
	[SerializeField] private LevelSelectScreen levelSelectScreen;
	[SerializeField] private UIStarCounter headerStarCounter;
	[SerializeField] private UIStarCounter exitStarCounter;
	[SerializeField] private Button nextWorldButton;
	[SerializeField] private Button previousWorldButton;
	[SerializeField] private GameObject clusterUIPrefab;
	[SerializeField] private RectTransform clusterUiRoot;

	[Header("Data")]
	[SerializeField] private List<WorldLayoutObject> worldData;

	private ScreenTransitionAnimation curtainAnimation;
	private SaveDataManager saveManager;
	private List<LevelClusterUI> uiClusterObjects;
	private int currentWorldIndex;

	public static WorldScreenManager Instance;

	private void Awake() {
		if (Instance != null && Instance != this) {
			Debug.LogWarning($"{this.name} already exists, destroying duplicate instance!");
			Destroy(this);
		}
		else {
			Instance = this;
		}
	}

	private void Start() {
		saveManager = new SaveDataManager();
		uiClusterObjects = new List<LevelClusterUI>();
		curtainAnimation = GameManager.Instance.CurtainAnimation;

		for (int i = 0; i < 4; i++) {
			var uiObj = Instantiate(clusterUIPrefab, clusterUiRoot);
			var uiCluster = uiObj.GetComponent<LevelClusterUI>();
			uiClusterObjects.Add(uiCluster);
		}
		LoadWorld(currentWorldIndex);
	}

	public void LoadWorld(int worldIndex) {
		if (worldIndex > worldData.Count) return;
		worldHeader.text = $"World\n{worldIndex + 1}";
		curtainAnimation.PlayAnimation(false, false);
		var starCount = 0;
		if (GameManager.Instance) starCount = GameManager.Instance.SaveDataManager.CountStarsInWorld(worldIndex);
		headerStarCounter.SetCounterValue(starCount);
		exitStarCounter.SetCounterValue(worldData[currentWorldIndex].RequiredStars);
		headerStarCounter.SetStarImage(worldData[currentWorldIndex].starSprite);
		exitStarCounter.SetStarImage(worldData[currentWorldIndex].starSprite);
		worldData[worldIndex].gameObject.SetActive(true);
		worldData[worldIndex].OnWorldLoad(starCount, uiClusterObjects);
		if (worldIndex == 0) previousWorldButton.interactable = false;
		else previousWorldButton.interactable = true;
		if (worldIndex == worldData.Count) nextWorldButton.interactable = false;
	}

	public void OnLevelSelect(string levelKey, int levelIndex) {
		bool[] starData;
		if (!GameManager.Instance.SaveDataManager.TryGetStars(levelKey, out starData)) starData = new bool[3];
		inputSystem.SetListeningState(false);
		levelSelectScreen.EnableAndPopulate(currentWorldIndex, levelIndex, levelKey, worldData[currentWorldIndex].LevelData[levelIndex], starData, worldData[currentWorldIndex].starSprite);
	}

	public void ReenableControl() {
		inputSystem.SetListeningState(true);
	}

	public void LoadNextWorld() {
		//if (!WorldStarQuotaReached()) return;
		if (currentWorldIndex + 1 > worldData.Count) return;
		IEnumerator coroutine = LoadWorldAnimation(currentWorldIndex + 1, true);
		StartCoroutine(coroutine);
	}

	public void LoadPreviousWorld() {
		if (currentWorldIndex == 0) return;
		var coroutine = LoadWorldAnimation(currentWorldIndex - 1, false);
		StartCoroutine(coroutine);
	}

	public void OnBackButton() {
		GameManager.Instance.BackToTitle();
	}

	private IEnumerator LoadWorldAnimation(int nextWorldIndex, bool movingForward) {
		yield return curtainAnimation.PlayAnimation(movingForward, true);

		worldData[currentWorldIndex].gameObject.SetActive(false);
		currentWorldIndex = nextWorldIndex;
		LoadWorld(currentWorldIndex);

		yield return curtainAnimation.PlayAnimation(movingForward, false);
	}

	public bool WorldStarQuotaReached() {
		var data = worldData[currentWorldIndex];
		if (data.CurrentStarCount >= data.RequiredStars) {
			return true;
		}

		return false;
	}
}
