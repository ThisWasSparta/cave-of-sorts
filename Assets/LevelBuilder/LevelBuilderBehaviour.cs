using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelBuilderBehaviour : MonoBehaviour {
	[SerializeField] private GameObject clickTilePrefab;
	[SerializeField] private LevelGridData currentLevelData;
	[SerializeField] private TextMeshProUGUI tileCounter;

	[SerializeField] private Vector2Int xBounds;
	[SerializeField] private Vector2Int yBounds;

	[SerializeField] private LayerMask primaryMask;
	[SerializeField] private LayerMask subMask;

	private Grid grid;
	private Camera mainCam;
	private EventSystem eventSystem;
	private Dictionary<Vector3Int, ClickTileBehaviour> clickTiles;
	private int tilesFilled;

	private void Start() {
		grid = GetComponent<Grid>();
		mainCam = Camera.main;
		eventSystem = EventSystem.current;
		clickTiles = new Dictionary<Vector3Int, ClickTileBehaviour>();
		InstantiateTiles();
	}

	private void Update() {
		if (Input.GetMouseButtonDown(0)) {
			CheckInput(primaryMask, true);
		}

		if (Input.GetMouseButtonDown(1)) {
			CheckInput(subMask, false);
		}
	}

	private void InstantiateTiles() {
		for (int i = xBounds.x; i < xBounds.y; i++) {
			for (int j = yBounds.x; j < yBounds.y; j++) {
				var gridPos = new Vector3Int(i, j);
				var stack = Instantiate(clickTilePrefab, grid.CellToWorld(gridPos), Quaternion.identity);
				clickTiles[gridPos] = stack.GetComponent<ClickTileBehaviour>();
				if (grid.CellToWorld(gridPos) == Vector3.zero) clickTiles[gridPos].AssignLuminaryStatus();
				clickTiles[gridPos].builder = this;
			}
		}
	}

	private void CheckInput(LayerMask mask, bool wasPrimary) {
		if (eventSystem.IsPointerOverGameObject()) return;
		Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
		RaycastHit raycastHit;
		Physics.Raycast(ray, out raycastHit, Mathf.Infinity, mask);

		ClickTileBehaviour clickTile;
		if (raycastHit.collider.transform.parent.TryGetComponent(out clickTile)) {
			clickTile.ChangeTileState(wasPrimary);
		}
	}

	public void CreateLevelDataObject() {
		Vector3Int currentPos = Vector3Int.zero;
		currentLevelData.tilePositions = new List<Vector3Int>();
		currentLevelData.tilesToFill = new List<Vector3Int>();
		currentLevelData.entityPositions = new List<Vector3Int>();
		currentLevelData.blockedTiles = new List<Vector3Int>();
		currentLevelData.specialTilePositions = new List<Vector3Int>();

		for (int i = xBounds.x; i < xBounds.y; i++) {
			for (int j = yBounds.x; j < yBounds.y; j++) {
				currentPos.x = i;
				currentPos.y = j;
				var clickTile = clickTiles[currentPos];
				if (!clickTile.Filled) continue;
				currentLevelData.tilePositions.Add(currentPos);
				switch (clickTile.SubState) {
					case 0:
						// empty, do nothing
						break;
					case 1:
						currentLevelData.tilesToFill.Add(currentPos); // spawn tiles here
						break;
					case 2:
						currentLevelData.entityPositions.Add(currentPos); // entity spawn or exit
						break;
					case 3:
						currentLevelData.blockedTiles.Add(currentPos); // don't spawn tiles here
						break;
					case 4:
						currentLevelData.specialTilePositions.Add(currentPos); // spawn tiles with special mechanics here (ph)
						break;
					default:
						break;
				}
			}
		}
		//EditorUtility.SetDirty(currentLevelData);
		//AssetDatabase.SaveAssets();
	}

	public void ChangeCounter(bool add) {
		if (add) {
			tilesFilled++;
		}
		else {
			tilesFilled--;
		}
		tileCounter.text = tilesFilled.ToString();
	}
}
