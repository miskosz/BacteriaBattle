using UnityEngine;
using System.Collections;

public class BoardBuilder : MonoBehaviour {

	public GameObject boardCellPrefab;
	public float cellDistance = 1;

	// initial board setup
	// 0 - out of board
	// 1 - empty
	// 2 - blue
	// 3 - orange
	int[,] boardSetup = {
		{1,1,1,2,0,0,0},
		{1,1,1,1,1,0,0},
		{1,1,1,1,1,1,0},
		{1,1,1,1,1,1,1},
		{0,1,1,1,1,1,1},
		{0,0,1,1,1,1,1},
		{0,0,0,3,1,1,1}
	};

	// game board
	BoardCell[,] board;

	// whose turn is it (0 or 1), switched after start
	int playerOnTurn = 0;
	BoardCellState[] playerState = {BoardCellState.Blue, BoardCellState.Orange};

	void Start () {
		//instance = this;

		// compute layout vectors
		// i - rows, j - columns
		Vector3 iVector = new Vector3 (0, -cellDistance, 0);
		Vector3 jVector = new Vector3(Mathf.Cos(-Mathf.PI/6) * cellDistance, -Mathf.Sin(-Mathf.PI/6) * cellDistance, 0);
		int iCenter = 3;
		int jCenter = 3;

		// init the board
		board = new BoardCell[7,7];

		for (int i = 0; i < board.GetLength(0); i++) {
			for (int j = 0; j < board.GetLength(1); j++) {

				// skip if positions is out of bounds
				if (boardSetup[i,j] == 0)
					continue;

				// determine cell state
				BoardCellState state;
				switch (boardSetup[i,j]) {
				case 1: state = BoardCellState.Empty; break;
				case 2: state = BoardCellState.Blue; break;
				case 3: state = BoardCellState.Orange; break;
				default: throw new UnityException("Invalid board setup cell state.");
				}

				// create board cell
				//Debug.Log("Initializing " + i + " " + j);
				board[i,j] = ((GameObject) Instantiate(boardCellPrefab)).GetComponent<BoardCell>();
				board[i,j].transform.position = (i-iCenter)*iVector + (j-jCenter)*jVector;
				board[i,j].Initialize(this, state, i, j);
			}
		}

		nextTurn ();
	}

	void nextTurn() {
		// switch player
		playerOnTurn = 1 - playerOnTurn;

		// detect closed areas
		// TODO

		// set highlighted places
		// iterate over board cells
		bool hasMove = false;

		for (int i = 0; i < board.GetLength(0); i++) {
			for (int j = 0; j < board.GetLength(1); j++) {
				if (onBoard(i,j)) {
					bool highlight = false;
					if (board[i,j].getState() == BoardCellState.Empty) {
						// iterate over cell neighbours
						for (int d = 0; d < di.Length; d++) {
							// get neighbour coordinates
							int ii = i + di[d], jj = j + dj[d];
							if (onBoard(ii, jj) && board[ii,jj].getState() == playerState[playerOnTurn]) {
								highlight = true;
								hasMove = true;
								break;
							}
						}
					}
					board[i,j].setHighlighted(highlight);
				}
			}
		}

		// detect game end
		if (!hasMove) {
			Debug.Log("GAME OVER");

			// all remaining cells are opponent's
			for (int i = 0; i < board.GetLength(0); i++) {
				for (int j = 0; j < board.GetLength(1); j++) {
					if (onBoard(i,j) && board[i,j].getState() == BoardCellState.Empty) {
						board[i,j].setState(playerState[1-playerOnTurn]);
					}
				}
			}

			// TODO

		}
	}

	public void playerSelected(int i, int j) {

		//Debug.Log ("Player selected " + i + " " + j);

		// only moves to highlighted empty cells are valid
		if (! board[i,j].getHighlighted() || board[i,j].getState() != BoardCellState.Empty)
			return;

		//Debug.Log ("It is a valid move.");

		// new bacteria here, please!
		board[i,j].setState(playerState[playerOnTurn]);

		// convert neighbours
		for (int d = 0; d < di.Length; d++) {
			// get neighbour coordinates
			int ii = i + di[d], jj = j + dj[d];
			if (onBoard(ii,jj) && board[ii,jj].getState() == playerState[1-playerOnTurn]) {
				board[ii,jj].setState(playerState[playerOnTurn]);
			}
		}

		nextTurn ();
	}

	///
	/// Board utilities
	///

	// neighbourhood vectors di, dj
	// 1 2
	// 0 . 3
	//   5 4
	int [] di = {0, -1, -1, 0, 1, 1};
	int [] dj = {-1, -1, 0, 1, 1, 0};

	bool onBoard(int i, int j) {
		return (
			0 <= i && i < board.GetLength(0) &&
			0 <= j && j < board.GetLength(1) &&
			boardSetup[i,j] != 0
		);
	}
}
