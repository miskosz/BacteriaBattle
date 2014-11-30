using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardBuilder : MonoBehaviour {

	public GameObject boardCellPrefab;
	public float cellDistance = 1;

	public AudioClip divisionAudio;
	public AudioClip gameOverAudio;

	int[] scoreCount = {0, 0};
	

	// initial board setup
	// 0 - out of board
	// 1 - empty
	// 2 - blue
	// 3 - orange
	/*int[,] boardSetup = {
		{1,1,1,2,0,0,0},
		{1,1,1,1,1,0,0},
		{1,1,1,1,1,1,0},
		{1,1,1,1,1,1,1},
		{0,1,1,1,1,1,1},
		{0,0,1,1,1,1,1},
		{0,0,0,3,1,1,1}
	};*/
	int[,] boardSetup = {
		{1,1,2,0,0,},
		{1,1,1,1,0,},
		{1,1,1,1,1,},
		{1,1,1,1,1,},
		{1,1,1,1,1,},
		{0,1,1,1,1,},
		{0,0,3,1,1,}
	};
		/*int[,] boardSetup = {
		{3,1,1,0,0},
		{1,1,1,1,0},
		{1,1,1,1,2},
		{0,1,1,1,1},
		{1,1,1,1,1},
		{1,1,1,1,0},
		{2,1,1,1,1},
		{0,1,1,1,1},
		{0,0,1,1,3}
	};*/

	// game board
	BoardCell[,] board;

	// whose turn is it (0 or 1), switched after start
	public int playerOnTurn = 0; // TODO: make safe maybe
	BoardCellState[] playerState = {BoardCellState.Blue, BoardCellState.Orange};

	void Start () {

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
		Debug.Log (scaleFactor);

		// init the board
		board = new BoardCell[boardSetup.GetLength(0),boardSetup.GetLength(1)];

		forEachBoardCell((int i, int j) => {

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
			board[i,j].transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
			board[i,j].transform.position = (i-iCenter)*iVector + (j-jCenter)*jVector;
			board[i,j].transform.position *= scaleFactor;
			board[i,j].Initialize(this, state, i, j);
		});

		// initialize highlighted cells & stuff
		nextTurn ();
	}

	void nextTurn() {
		// switch player
		playerOnTurn = 1 - playerOnTurn;

		// fill closed areas
		fillClosedAreas();

		// set highlighted places
		// iterate over board cells
		bool hasMove = false;

		forEachBoardCell((int i, int j) => {
				
			bool highlight = false;
			if (board[i,j].isEmpty()) {
				// iterate over cell neighbours
				forEachCellNeighbour(i, j, (int ii, int jj) => {
					if (board[ii,jj].getState() == playerState[playerOnTurn]) {
						highlight = true;
						hasMove = true;
					}
				});
			}
			board[i,j].setHighlighted(highlight); 


		});

		updateScore();

		// detect game end
		if (!hasMove) {
			Debug.Log("GAME OVER");

			// all remaining cells are opponent's
			forEachBoardCell((int i, int j) => {
				if (board[i,j].isEmpty()) {
						board[i,j].setState(playerState[1-playerOnTurn]);
				}
			});

			// TODO

			// play audio
			audio.clip = gameOverAudio;
			audio.Play();

		}
	}

	public void playerSelected(int i, int j) {
		
		//Debug.Log ("Player selected " + i + " " + j);
		
		// only moves to highlighted empty cells are valid
		if (! board[i,j].getHighlighted() || ! board[i,j].isEmpty())
			return;
		
		//Debug.Log ("It is a valid move.");
		
		// new bacteria here, please!
		board[i,j].setState(playerState[playerOnTurn]);
		
		// convert neighbours
		forEachCellNeighbour(i, j, (int ii, int jj) => {
			if (board[ii,jj].getState() == playerState[1-playerOnTurn]) {
				board[ii,jj].setState(playerState[playerOnTurn]);
			}
		});

		// play dividing sound
		audio.clip = divisionAudio;
		audio.Play();

		nextTurn();
	}

	// score counting
	public void updateScore() {
		int[] tempScore = {0, 0};

		forEachBoardCell((int i, int j) => {
			if (!board[i,j].isEmpty())
				tempScore[(int)board[i,j].getState()]++;
		});
		
		scoreCount = tempScore;
	}

	public int getScore(BoardCellState player) {
		return scoreCount[(int)player];
	}

	// simple struct for fillClosedAreas needs, do not use elsewhere if not nesessary
	struct IntPair {
		public IntPair(int i, int j) {
			this.i = i;
			this.j = j;
		}
		public int i, j;
	};

	// Let us say that an area of empty is WEAKLY CONNECTED if cells are
	// separated by at most one bacteria. A weakly connected area should
	// be filled if all of its boundary bacteria are of the same type.
	void fillClosedAreas() {

		// detect weakly connected areas by a depth-first search

		int[,] visited = new int[boardSetup.GetLength(0),boardSetup.GetLength(1)];
		int areaId = 0;

		forEachBoardCell((int i, int j) => {
			// find empty cell which was not visited
			if (board[i,j].isEmpty() && visited[i,j] == 0) {
				// fill this area
				areaId++;

				// store boundary type and if uniform
				BoardCellState boundary = BoardCellState.Empty;
				bool isUniform = true;

				// store positions to visit in a stack for dfs
				Stack<IntPair> dfs = new Stack<IntPair>();

				// kick off the search
				dfs.Push(new IntPair(i,j));
				while (dfs.Count != 0) {
					IntPair pos = dfs.Pop();
					if (visited[pos.i, pos.j] != 0)
						continue;

					// this cell has not been visited yet

					// mark as visited
					// add all neighbours to the queue if empty
					// add empty neighbours if not empty
					visited[pos.i, pos.j] = areaId;

					forEachCellNeighbour(pos.i, pos.j, (int ii, int jj) => {
						if (board[pos.i, pos.j].isEmpty() || board[ii, jj].isEmpty())
							dfs.Push(new IntPair(ii, jj));
					});

					// store if we have seen bacteria
					if (! board[pos.i, pos.j].isEmpty()) {
						if (boundary == BoardCellState.Empty)
							boundary = board[pos.i, pos.j].getState();
						else if (boundary != board[pos.i, pos.j].getState())
							isUniform = false;
					}
				}

				// if boundary is uniform, fill the cells from area areaId
				if (isUniform) {
					forEachBoardCell((int ni, int nj) => {
						if (visited[ni,nj] == areaId) {
							board[ni,nj].setState(boundary);
						}
					});
				}
			}
		});
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
			0 <= i && i < boardSetup.GetLength(0) &&
			0 <= j && j < boardSetup.GetLength(1) &&
			boardSetup[i,j] != 0
		);
	}

	// board iterator
	// usage:
	//     forEachBoardCell((i,j) => {
	//         Debug.Log("boardCell " + i + " " + j);
	//     });
	public delegate void CellDelegate(int i, int j);
	public void forEachBoardCell(CellDelegate method)
	{
		for (int i = 0; i < boardSetup.GetLength(0); i++) {
			for (int j = 0; j < boardSetup.GetLength(1); j++) {
				if (onBoard(i,j)) {
					method(i,j);
				}
			}
		}
	}

	// cell neighbourhood iterator
	public void forEachCellNeighbour(int i, int j, CellDelegate method)
	{
		for (int d = 0; d < di.Length; d++) {
			// get neighbour coordinates
			int ii = i + di[d], jj = j + dj[d];
			if (onBoard(ii, jj)) {
				method(ii, jj);
			}
		}
	}


}
