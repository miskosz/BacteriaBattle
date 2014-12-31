using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardBuilder : MonoBehaviour {
	
	public GameObject boardCellPrefab;
	public float cellDistance = 1;
	
	public PauseMenu pauseMenu;
	public WinnerLoser winnerLoser;

	public bool inputEnabled = false;
	bool playerVsAI;

	// initial board setup
	// see Board.cs for legend
	int[,] boardSetup;
	int[,] board0 = {
		{1,2,2,3,3,},
		{2,2,2,2,3,},
		{2,2,2,2,0,},
		{2,2,2,2,2,},
		{0,2,2,2,2,},
		{3,2,2,2,2,},
		{3,3,2,2,1,}
	};
	int[,] board1 = {
		{3,1,2,3,3,3,3},
		{3,2,2,2,2,3,3},
		{2,2,2,3,2,0,3},
		{2,2,2,2,2,2,3},
		{3,2,2,2,2,2,3},
		{3,2,2,2,2,2,2},
		{3,0,2,3,2,2,2},
		{3,3,2,2,2,2,3},
		{3,3,3,3,2,1,3}
	};
	int[,] board2 = {
		{1,2,2,2,3,3,3},
		{2,2,2,2,2,3,3},
		{2,2,2,2,2,2,3},
		{0,2,2,2,2,2,0},
		{3,2,2,2,2,2,2},
		{3,3,2,2,2,2,2},
		{3,3,3,2,2,2,1}
	};

	// game board
	Board board;
	BoardCell[,] boardCells;
	
	void Start () {

		// apply settings
		switch (GlobalState.gameMode) {
		case 0: playerVsAI = false; break;
		case 1: playerVsAI = true; break;
		default: throw new UnityException("Invalid game mode.");
		}

		switch (GlobalState.board) {
		case 0: boardSetup = board0; break;
		case 1: boardSetup = board1; break;
		case 2: boardSetup = board2; break;
		default: throw new UnityException("Invalid board.");
		}

		board = new Board(boardSetup, GlobalState.GetStartingPlayer());

		// compute layout vectors
		// i - rows, j - columns
		Vector3 iVector = new Vector3 (0, -cellDistance, 0);
		Vector3 jVector = new Vector3(Mathf.Cos(-Mathf.PI/6) * cellDistance, -Mathf.Sin(-Mathf.PI/6) * cellDistance, 0);

		// get center coordinates
		float iCenter = (boardSetup.GetLength(0)-1)/2.0f;
		float jCenter = (boardSetup.GetLength(1)-1)/2.0f;

		// compute scale factor
		int standardWidth = 7;
		float scaleFactor = standardWidth / (float)boardSetup.GetLength(1);
		//Debug.Log(scaleFactor);

		// init the board
		boardCells = new BoardCell[boardSetup.GetLength(0),boardSetup.GetLength(1)];

		board.ForEachBoardCell((int i, int j) => {

			// determine cell state
			BoardCellState state = board.board[i,j];

			// create board cell
			//Debug.Log("Initializing " + i + " " + j);
			boardCells[i,j] = ((GameObject) Instantiate(boardCellPrefab)).GetComponent<BoardCell>();
			boardCells[i,j].transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
			boardCells[i,j].transform.position = (i-iCenter)*iVector + (j-jCenter)*jVector;
			boardCells[i,j].transform.position *= scaleFactor;
			boardCells[i,j].Initialize(this, i, j);
			if (state != BoardCellState.Empty)
				boardCells[i,j].Spawn(state);
		});

		// initialize highlighted cells & stuff
		StartCoroutine(nextTurn());
	}

	public IEnumerator playerSelected(int i, int j) {
		
		// Assert: The move is valid.

		DisableInput();
		HighlightOff();
		
		// NOTE: playerOnTurn is switched by this!
		List<Action> actions = board.MakeMove(i,j);

		yield return StartCoroutine(AnimateActions(actions));
		yield return StartCoroutine(nextTurn());
	}

	IEnumerator nextTurn() {

		// set highlighted places
		bool hasMove = HighlightOn();

		// detect game end
		if (!hasMove) {
			Debug.Log("GAME OVER");

			// draw winner and loser
			if (board.scoreCount[0] < board.scoreCount[1])
				winnerLoser.spin();
			winnerLoser.setVisible();
			yield return new WaitForSeconds(0.5f);

			// show pause menu
			pauseMenu.Toggle();
		}
		else {
			if (IsAITurn())
				StartCoroutine(MakeAIMove());
			else
				EnableInput();
		}

		yield break;
	}

	// Set highlighted cells.
	// Returns true if player has a move.
	bool HighlightOn() {

		HighlightOff();

		List<IntPair> moves = board.GetPossibleMoves();
		foreach (IntPair cell in moves)
			boardCells[cell.i,cell.j].setHighlighted(true, board.playerOnTurn); 

		return (moves.Count != 0);
	}

	void HighlightOff() {
		board.ForEachBoardCell((int i, int j) => {
			boardCells[i,j].setHighlighted(false); 
		});
	}

	IEnumerator AnimateActions(List<Action> actions) {
		// Process the actions by type, in the order split - convert - spawn.
		// Doing foreach 3x does not kill us.
		
		// split
		foreach (Action action in actions) {
			if (action.type == ActionType.Split) {
				boardCells[action.splitCell.i,action.splitCell.j].Split(boardCells[action.cell.i,action.cell.j]);
				yield return StartCoroutine(GlobalAnimationTimer.WaitForAnimationEnd());
				boardCells[action.cell.i,action.cell.j].Appear(board.playerOnTurn.Opponent()); // playerOnTurn already switched
			}
		}
		
		// convert
		foreach (Action action in actions) {
			if (action.type == ActionType.Convert) {
				boardCells[action.cell.i,action.cell.j].Convert();
			}
		}
		yield return StartCoroutine(GlobalAnimationTimer.WaitForAnimationEnd());
		
		// spawn
		foreach (Action action in actions) {
			if (action.type == ActionType.Spawn) {
				boardCells[action.cell.i,action.cell.j].Spawn(board.playerOnTurn.Opponent()); // playerOnTurn already switched
			}
		}
		yield return StartCoroutine(GlobalAnimationTimer.WaitForAnimationEnd());	
	}

	bool IsAITurn() {
		return playerVsAI && board.playerOnTurn == BoardCellState.Player1;
	}

	IEnumerator MakeAIMove() {
		yield return new WaitForSeconds(0.4f);
		IntPair move = board.GetAIMove();
		StartCoroutine(playerSelected(move.i, move.j));
	}

	public int GetScore(BoardCellState player) {
		return board.scoreCount[(int)player];
	}

	public void DisableInput() {
		inputEnabled = false;
	}

	public void EnableInput() {
		inputEnabled = true;
	}

}
