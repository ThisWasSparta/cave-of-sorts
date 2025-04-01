using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayInputSystem : MonoBehaviour {
	public delegate void InputEvent(Vector3Int position);
	public static event InputEvent ValidPositionFound;

	[SerializeField] private AnimationCurve returnCurve;
	[SerializeField] private PauseScreenBehaviour pauseScreen;
	[SerializeField] private AnimationCoordinator animationCoordinator;
	[SerializeField] private LayerMask gridTileLayer;
	[SerializeField] private LayerMask chipLayer;
	[SerializeField] private LayerMask bgLayer;
	[SerializeField] private float returnToHandDuration = 0.3f;
	[SerializeField] private float dragRegisterDistance = 2f;
	[SerializeField] private float tapInputRegistrationThreshold = 0.5f;

	private DealerBehaviour dealerBehaviour;
	private ChipStack currentlyHeldStack;
	private ChipStack cachedInputStack;
	private Camera mainCam;
	private Vector3 heldStackHomePosition;
	private Vector3 previousMousePos = Vector3.zero;
	private RaycastHit gridRaycastHit;
	private Vector3Int[] allowedTutorialInputSpots;
	private Vector3Int tutorialPositionBuffer;
	private Vector2 touchBeganPosition;
	private float touchInputEndTime;
	private bool mobileControls;
	private bool pollingInputType;
	private bool tapHolding;
	private bool controlEnabled = true;
	private bool tutorialMode;

	private const float MOVE_THRESHOLD = 0.1f;

	private void Start() {
		mainCam = Camera.main;
		dealerBehaviour = GridManager.Instance.DealerBehaviour;
		mobileControls = Application.isMobilePlatform;
	}

	private void Update() { // this whole class sucks and i hate it >:( (no longer todo, but i still do, even if it works a lot better now)
		if (!controlEnabled) return;
		HandleInput();
	}

	private void HandleInput() {
		Touch touch;
		if (mobileControls) {
			if (Input.touchCount > 0)
				touch = Input.GetTouch(0);
			else return;
		}
		else {
			touch = TranslateMouseInputToTouch();
		}

		if (currentlyHeldStack) {
			if (!tapHolding) {
				currentlyHeldStack.transform.position = BackgroundCast(touch.position);
			}

			if (tapHolding && touch.phase == TouchPhase.Began || !tapHolding && touch.phase == TouchPhase.Ended) {
				var gridTile = GridTileCast(touch.position);
				if (!gridTile) return;
				var tileCoords = gridTile.GridPosition;
				var stack = gridTile.GetComponent<BoardChipStack>();

				if (!stack.IsEmpty || tutorialMode && !allowedTutorialInputSpots.Contains(tileCoords)) {
					if (!tapHolding) ReturnCurrentStackToHand();
					else dealerBehaviour.DisableSelectionShine();
				}
				else {
					gridTile.ChipStack.AddChipsToStack(currentlyHeldStack);
					animationCoordinator.RecordTilePlacement(tileCoords);
					currentlyHeldStack.ClearChips();
					GridManager.Instance.DealerBehaviour.RemoveStackFromHand(currentlyHeldStack.gameObject, tutorialMode);
					if (tutorialMode) {
						tutorialPositionBuffer = tileCoords;
						ValidPositionFound?.Invoke(tileCoords);
					}
				}
				dealerBehaviour.DisableSelectionShine();
				pauseScreen.SetPauseButtonState(true);
				currentlyHeldStack = null;
			}
		}
		else {
			if (touch.phase == TouchPhase.Began) {
				if (cachedInputStack = HandStackCast(touch.position)) {
					touchInputEndTime = Time.time + tapInputRegistrationThreshold;
					touchBeganPosition = touch.position;
					pollingInputType = true;
				}
			}

			if (pollingInputType) {
				if (Time.time > touchInputEndTime || Vector2.Distance(touch.position, touchBeganPosition) > dragRegisterDistance) {
					tapHolding = false;
					CloseInputPoll();
					return;
				}
				else if (touch.phase == TouchPhase.Ended) {
					tapHolding = true;
					CloseInputPoll();
					dealerBehaviour.EnableSelectionShine(currentlyHeldStack.gameObject);
				}
			}
		}
	}

	private void CloseInputPoll() {
		currentlyHeldStack = cachedInputStack;
		cachedInputStack = null;
		pauseScreen.SetPauseButtonState(false);
		pollingInputType = false;
	}

	private ChipStack HandStackCast(Vector3 screenPosition) {
		Ray ray = mainCam.ScreenPointToRay(screenPosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, chipLayer)) {
			ChipStack stack;
			if (hit.collider == null) return null;
			if (hit.collider.gameObject.transform.TryGetComponent(out stack)) {
				heldStackHomePosition = stack.gameObject.transform.position;
				return stack;
			}
		}

		return null;
	}

	private GridBaseTile GridTileCast(Vector3 screenPosition) {
		Ray ray = mainCam.ScreenPointToRay(screenPosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridTileLayer)) {
			GridBaseTile gridTile;
			if (hit.collider == null) return null;
			if (hit.collider.gameObject.transform.TryGetComponent(out gridTile)) {
				return gridTile;
			}
		}

		return null;
	}

	private Vector3 BackgroundCast(Vector3 screenPosition) {
		Ray ray = mainCam.ScreenPointToRay(screenPosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, bgLayer)) {
			if (hit.collider == null) return Vector3.zero;
			return hit.point;
		}

		return Vector3.zero;
	}

	private Touch TranslateMouseInputToTouch() {
		var touch = new Touch();
		touch.position = Input.mousePosition;

		touch.deltaPosition = new Vector2(previousMousePos.x - touch.position.x, previousMousePos.y - touch.position.y);
		if (touch.deltaPosition.sqrMagnitude <= MOVE_THRESHOLD * MOVE_THRESHOLD)
			touch.phase = TouchPhase.Stationary;
		else touch.phase = TouchPhase.Moved;

		if (Input.GetMouseButtonDown(0)) touch.phase = TouchPhase.Began;
		if (Input.GetMouseButtonUp(0)) touch.phase = TouchPhase.Ended;

		previousMousePos = touch.position;
		return touch;
	}

	private void ReturnCurrentStackToHand() {
		controlEnabled = false;

		IEnumerator coroutine = AnimateStackReturn();
		StartCoroutine(coroutine);
	}

	private IEnumerator AnimateStackReturn() {
		var timer = 0f;

		var startPos = currentlyHeldStack.transform.position;

		while (timer <= returnToHandDuration) {
			timer += Time.deltaTime;

			currentlyHeldStack.transform.position = Vector3.Lerp(startPos, heldStackHomePosition, returnCurve.Evaluate(timer / returnToHandDuration));
			yield return null;
		}

		currentlyHeldStack.transform.position = heldStackHomePosition;
		controlEnabled = true;
		currentlyHeldStack = null;
	}

	public void SetupTutorial(Vector3Int[] allowedInputSpots) {
		tutorialMode = true;
		allowedTutorialInputSpots = allowedInputSpots;
	}

	public void RegisterDelayedStackPlacement() {
		GridManager.Instance.DealerBehaviour.ForceHandEmptySignal();
	}

	public void SetControlState(bool setActive) {
		if (currentlyHeldStack) return;
		controlEnabled = setActive;
	}
}
