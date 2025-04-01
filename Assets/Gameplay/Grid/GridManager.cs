using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour {
	public delegate void GridManagerEvent();
	public static event GridManagerEvent OnCurseClear;

	[SerializeField] private LevelContentData phLevelDataContainer;
	[SerializeField] private GameObject tileBasePrefab;
	[SerializeField] private ObjectiveTracker objectiveTracker;
	[SerializeField] private DealerBehaviour dealerComponent;
	[SerializeField] private GameEndScreenBehaviour endScreen;
	[SerializeField] private GameplayInputSystem inputSystem;
	[SerializeField] private Vector2Int xBounds;
	[SerializeField] private Vector2Int yBounds;

	private Dictionary<Vector3Int, GridBaseTile> gridTiles;
	private HashSet<BoardChipStack> cursedTiles;
	private int activeCursedTiles;

	public Grid grid;

	[HideInInspector] public Vector3Int[] hexagonDirections;
	[HideInInspector] public Vector3Int[] hexagonDirectionsOffset;

	public GameplayInputSystem InputSystem { get { return inputSystem; } }
	public DealerBehaviour DealerBehaviour { get { return dealerComponent; } }
	public ObjectiveTracker ObjectiveTracker { get { return objectiveTracker; } }
	public GameEndScreenBehaviour EndScreen { get { return endScreen; } }

	public static GridManager Instance;

	static float RANDOM_FILL_CHANCE = 0.2f;

	private void Awake() {
		if (Instance != null && Instance != this) {
			Debug.LogWarning($"{this.name} already exists, destroying duplicate instance!");
			Destroy(this);
		}
		else {
			Instance = this;
		}

		gridTiles = new Dictionary<Vector3Int, GridBaseTile>();
		grid = GetComponent<Grid>();

		hexagonDirections = new Vector3Int[] // neighbour coords on an even y-coord (start top right clockwise)
		{
			new Vector3Int(0, 1),
			new Vector3Int(1, 0),
			new Vector3Int(0, -1),
			new Vector3Int(-1, -1),
			new Vector3Int(-1, 0),
			new Vector3Int(-1, 1),
		};

		hexagonDirectionsOffset = new Vector3Int[] // neighbour coords on an uneven y-coord (start top right clockwise)
		{
			new Vector3Int(1, 1),
			new Vector3Int(1, 0),
			new Vector3Int(1, -1),
			new Vector3Int(0, -1),
			new Vector3Int(-1, 0),
			new Vector3Int(0, 1),
		};
	}

	public void InstantiateLevel(LevelContentData contentData, LevelGridData gridData, Sprite starImage, bool[] starData) {
		cursedTiles = new HashSet<BoardChipStack>();
		InstantiateGrid(contentData, gridData);

		objectiveTracker.Setup(contentData.levelObjectives, starImage, starData);
		AnimationCoordinator.Instance.PlayBoardSetupAnimation(cursedTiles);
	}

	public void InstantiateGrid(LevelContentData contentData, LevelGridData gridData) {
		ClearGrid();
		var amountGenerator = dealerComponent.ChipAmountGenerator;
		var colourAmountGenerator = dealerComponent.ColourAmountGenerator;

		foreach (var tilePos in gridData.tilePositions) {
			var tileObj = Instantiate(tileBasePrefab, grid.CellToWorld(tilePos), Quaternion.identity);
			var tileBase = tileObj.GetComponent<GridBaseTile>();
			var chipStack = tileObj.GetComponent<BoardChipStack>();
			tileBase.SetGridPosition(tilePos);
			gridTiles.Add(tilePos, tileBase);
			if (GameManager.Instance.CurrentlyInTutorialMode) continue;

			if (gridData.tilesToFill.Contains(tilePos)) {
				chipStack.AddTilesRandom(contentData.tilesPresentInLevel, amountGenerator.GenerateValue(), colourAmountGenerator.GenerateValue());
				cursedTiles.Add(chipStack);
				chipStack.ApplyCurse();
				continue;
			}

			if (!gridData.blockedTiles.Contains(tilePos)) {
				if (Random.value <= RANDOM_FILL_CHANCE) {
					chipStack.AddTilesRandom(contentData.tilesPresentInLevel, amountGenerator.GenerateValue(), colourAmountGenerator.GenerateValue());
				}
			}
		}
	}

	public int EmptyGridTileCount() {
		int count = 0;
		foreach (var gridTile in gridTiles.Values) {
			if (gridTile.ChipStack.IsEmpty)
				count++;
		}
		return count;
	}

	private void CheckIfGridIsFull(bool _) {
		foreach (var gridTile in gridTiles.Values) {
			if (gridTile.ChipStack.IsEmpty) {
				return;
			}
		}

		GameManager.Instance.EndLevel(false);
	}

	public void RegisterChainedTile() {
		activeCursedTiles++;
	}

	public void RegisterChainRelease() {
		activeCursedTiles--;
		if (activeCursedTiles <= 0) {
			OnCurseClear?.Invoke();
		}
	}

	public bool IsTileBlockedOrEmpty(Vector3Int gridPosition) {
		GridBaseTile tile;

		if (gridTiles.TryGetValue(WorldToGridPos(gridPosition), out tile)) {
			if (tile.IsBlocked) return true;
			return false;
		}
		else return true;
	}

	public Vector3Int WorldToGridPos(Vector3 worldPosition) {
		return grid.WorldToCell(worldPosition);
	}

	public Vector3 GridToWorldPos(Vector3Int gridPos) {
		return grid.CellToWorld(gridPos);
	}

	public void CreateSpecificStack(Vector3Int tilePos, ChipType type, int amount) {
		var chipStack = RequestTileUnsafe(tilePos).ChipStack;
		chipStack.AddSpecificTiles(type, amount);
		chipStack.UpdateCounter();
	}

	public void CreateSpecificStack(Vector3Int tilePos, Tuple<ChipType, int>[] types) {
		var chipStack = RequestTileUnsafe(tilePos).ChipStack;
		foreach (var tuple in types) {
			chipStack.AddSpecificTiles(tuple.Item1, tuple.Item2);
		}
		chipStack.UpdateCounter();
	}

	public void RegisterMatch(ChipType type, int amount) {
		objectiveTracker.RegisterMatch(type, amount);
	}

	public bool TryRequestTile(Vector3Int position, out GridBaseTile baseTile) {
		return gridTiles.TryGetValue(position, out baseTile);
	}

	public GridBaseTile RequestTileUnsafe(Vector3Int position) {
		GridBaseTile tile;
		gridTiles.TryGetValue(position, out tile);
		return tile;
	}

	public Vector3Int GetTilePos(Vector3 position) {
		return grid.WorldToCell(position);
	}

	public List<ChipType> GetTileTypes() {
		return phLevelDataContainer.tilesPresentInLevel;
	}

	public void ClearGrid() {
		foreach (var tile in gridTiles) {
			Destroy(tile.Value);
		}
		gridTiles.Clear();
	}

	private void OnEnable() {
		MatchResolver.BoardEvaluated += CheckIfGridIsFull;
	}

	private void OnDisable() {
		MatchResolver.BoardEvaluated -= CheckIfGridIsFull;
	}
}
