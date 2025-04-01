using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	private Camera mainCam;
	private GameEndScreenBehaviour endScreen;
	private LevelContentData currentContentData;
	private LevelGridData currentGridData;
	private SaveDataManager saveDataManager;
	private Sprite currentStarImage;
	private TutorialManager tutorialManager;
	private ScreenTransitionAnimation curtainAnimation;

	private string currentLevelKey;
	private string tutorialReturnCall;
	private int currentDealerLevel;
	private int currentWorldIndex;
	private bool[] currentStarData;
	private bool currentlyInTutorial;
	private bool currentlyPlayingGame;
	
	public static GameManager Instance;
	public SaveDataManager SaveDataManager { get { return saveDataManager; } }
	public Camera MainCam { get { return mainCam; } }
	public ScreenTransitionAnimation CurtainAnimation { get { return curtainAnimation; } }
	public bool CurrentlyInTutorialMode { get { return currentlyInTutorial; } }

	private void Awake() {
		if (Instance != null && Instance != this) {
			Destroy(this.gameObject);
			Debug.LogWarning($"{this.name} instance already exists! Destroying duplicate.");
		}
		else {
			Instance = this;
		}

		DontDestroyOnLoad(this);
		saveDataManager = new SaveDataManager();
		saveDataManager.LoadOrCreateData();
	}

	private void Start() {
		mainCam = Camera.main;
		curtainAnimation = FindAnyObjectByType<ScreenTransitionAnimation>();
		DontDestroyOnLoad(curtainAnimation);
	}

	public void TransitionToWorld(int worldIndex) {
		IEnumerator coroutine = WorldLoadRoutine(worldIndex);
		StartCoroutine(coroutine);
	}

	private IEnumerator WorldLoadRoutine(int worldIndex) {
		currentWorldIndex = worldIndex;
		yield return SceneManager.LoadSceneAsync("WorldMap");
	}

	public void TransitionToAndBeginTutorial(string sceneToReturnTo) { // "WorldMap" or "MainMenuScene" (i should really stop naming scenes with "scene" on the end of it like this, it's so spectacularly redundant)
		IEnumerator coroutine = LoadTutorial(sceneToReturnTo);
		StartCoroutine(coroutine);
	}

	public void TransitionToAndLoadLevel(LevelContentData content, LevelGridData grid, string levelKey, int dealerLevel, bool[] starData, Sprite starImage) {
		currentContentData = content;
		currentGridData = grid;
		currentLevelKey = levelKey;
		currentDealerLevel = dealerLevel;
		currentStarData = starData;
		currentStarImage = starImage;
		IEnumerator coroutine = LoadLevelCoroutine();
		StartCoroutine(coroutine);
		currentlyPlayingGame = true;
	}

	private IEnumerator LoadTutorial(string sceneToReturnTo) {
		yield return curtainAnimation.PlayAnimation(true, true);
		yield return new WaitForSeconds(0.3f);
		yield return LoadSceneAsyncByName("GameplayScene");
		currentlyInTutorial = true;
		tutorialManager = FindAnyObjectByType<TutorialManager>();
		tutorialManager.StartTutorial(sceneToReturnTo);
	}

	public IEnumerator LoadLevelCoroutine() {
		yield return LoadSceneAsyncByName("GameplayScene");
		GridManager.Instance.DealerBehaviour.InitializeDealer(currentDealerLevel);
		yield return new WaitForEndOfFrame();
		GridManager.Instance.InstantiateLevel(currentContentData, currentGridData, currentStarImage, currentStarData);
	}

	public void EndLevel(bool playerWon) {
		if (playerWon) {
			currentStarData = GridManager.Instance.ObjectiveTracker.StarData;
			saveDataManager.RegisterLevelScore(currentLevelKey, currentStarData);
		}
		ShowEndScreen(playerWon);
	}

	public void StartNextLevel() {
		// TODO: figure out how to do this
		// probably best to decouple level storing/loading logic from the world screen scene

		// take current level id, add one to it, request if that level exists, if not, world is done, return to map and play animation
	}

	public void BackToWorldMap() {
		IEnumerator coroutine = BackToMapScreen();
		StartCoroutine(coroutine);
	}

	public void LoadSceneByName(string sceneName) {
		IEnumerator coroutine = LoadSceneAsyncByName(sceneName);
		StartCoroutine(coroutine);
	}

	public IEnumerator LoadSceneAsyncByName(string sceneName) {
		AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName);
		while (!sceneLoad.isDone) yield return null;
		mainCam = Camera.main;
	}

	public void BackToTitle() {
		LoadSceneByName("MainMenuScene");
	}

	private IEnumerator BackToMapScreen() {
		yield return LoadSceneAsyncByName("WorldMap");
		currentlyPlayingGame = false;
		// WorldScreenManager.Instance.LoadWorld(currentWorldIndex);
		yield return null;
	}



	public void RestartLevel() {
		TransitionToAndLoadLevel(currentContentData, currentGridData, currentLevelKey, currentDealerLevel, currentStarData, currentStarImage);
	}

	private void ShowEndScreen(bool playerWon) {
		var endScreen = GridManager.Instance.EndScreen;
		endScreen.ShowScreen();
		endScreen.StartEndAnimation(playerWon, currentStarData, currentStarImage);
	}
}
