using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardBuilder : MonoBehaviour {

	public bool playerVsAI = false;

	public GameObject boardCellPrefab;
	public float cellDistance = 1;

	// this is bad and you should feel bad
	public float splittingAnimDuration = 0.5f;

	public AudioClip divisionAudio;
	public AudioClip gameOverAudio;

	public PauseMenu pauseMenu;
	public WinnerLoser winnerLoser;

	int[] scoreCount = {0, 0};

	bool inputEnabled = false;

	// initial board setup
	// 0 - out of board
	// 1 - empty
	// 2 - blue
	// 3 - orange
	int[,] boardSetup;
	int[,] board0 = {
		{2,1,1,0,0,},
		{1,1,1,1,0,},
		{1,1,1,1,3,}//,
//		{1,1,1,1,1,},
//		{3,1,1,1,1,},
//		{0,1,1,1,1,},
//		{0,0,1,1,2,}
	};
	int[,] board1 = {
		{0,1,1,0,0,0,0},
		{0,1,1,1,1,0,0},
		{1,1,1,0,1,2,0},
		{1,1,1,1,1,1,0},
		{0,1,1,1,1,1,0},
		{0,1,1,1,1,1,1},
		{0,3,1,0,1,1,1},
		{0,0,1,1,1,1,0},
		{0,0,0,0,1,1,0}
	};
	int[,] board2 = {
		{2,1,1,1,0,0,0},
		{1,1,1,1,1,0,0},
		{1,1,1,1,1,1,0},
		{3,1,1,1,1,1,3},
		{0,1,1,1,1,1,1},
		{0,0,1,1,1,1,1},
		{0,0,0,1,1,1,2}
	};

	// game board
	BoardCell[,] board;

	// whose turn is it (0 or 1)
	int playerOnTurn;
	BoardCellState[] playerState = {BoardCellState.Player1, BoardCellState.Player2};

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
		board = new BoardCell[boardSetup.GetLength(0),boardSetup.GetLength(1)];

		forEachBoardCell((int i, int j) => {

			// determine cell state
			BoardCellState state;
			switch (boardSetup[i,j]) {
			case 1: state = BoardCellState.Empty; break;
			case 2: state = BoardCellState.Player1; break;
			case 3: state = BoardCellState.Player2; break;
			default: throw new UnityException("Invalid board setup cell state.");
			}

			// create board cell
			//Debug.Log("Initializing " + i + " " + j);
			board[i,j] = ((GameObject) Instantiate(boardCellPrefab)).GetComponent<BoardCell>();
			board[i,j].transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
			board[i,j].transform.position = (i-iCenter)*iVector + (j-jCenter)*jVector;
			board[i,j].transform.position *= scaleFactor;
			board[i,j].Initialize(this, i, j);
			if (state != BoardCellState.Empty)
				board[i,j].Spawn(state);
		});

		// initialize highlighted cells & stuff
		playerOnTurn = 1 - GlobalState.GetStartingPlayer();
		StartCoroutine(nextTurn());
	}

	IEnumerator nextTurn() {
		// switch player
		playerOnTurn = 1 - playerOnTurn;

		// fill closed areas
		fillClosedAreas();
		yield return StartCoroutine(GlobalAnimationTimer.WaitForAnimationEnd());

		// set highlighted places
		bool hasMove = HighlightOn(playerState[playerOnTurn]);

		updateScore();

		// detect game end
		if (!hasMove) {
			Debug.Log("GAME OVER");

			// all remaining cells are opponent's
			forEachBoardCell((int i, int j) => {
				if (board[i,j].isEmpty()) {
						board[i,j].Spawn(playerState[1-playerOnTurn]);
				}
			});

			// play audio
			if (MusicManagerSingleton.Instance.soundsOn()) {
				audio.clip = gameOverAudio;
				audio.Play();
			}

			// draw winner and loser
			if (scoreCount[0] < scoreCount[1])
				winnerLoser.spin();
			winnerLoser.setVisible();
			yield return new WaitForSeconds(0.5f);

			// show pause menu
			pauseMenu.Toggle();
		}

		if (playerVsAI && playerOnTurn == 0) {
			//disableInput(); TODO!!!!!
			StartCoroutine(MakeAIMove());
		}
		else {
			enableInput();
		}

		yield break;
	}

	// set highlighted cells
	// returns if layer has move
	bool HighlightOn(BoardCellState player) {
		bool hasMove = false;
		
		forEachBoardCell((int i, int j) => {
			
			bool highlight = false;
			if (board[i,j].isEmpty()) {
				// iterate over cell neighbours
				forEachCellNeighbour(i, j, (int ii, int jj) => {
					if (board[ii,jj].getState() == player) {
						highlight = true;
						hasMove = true;
					}
				});
			}
			board[i,j].setHighlighted(highlight, player); 
			
		});

		return hasMove;
	}

	void HighlightOff() {
		forEachBoardCell((int i, int j) => {
			if (board[i,j].getHighlighted()) {
				board[i,j].setHighlighted(false); 
			}
		});
	}
	
	public IEnumerator playerSelected(int i, int j) {
		
		//Debug.Log ("Player selected " + i + " " + j);
		
		// only moves to highlighted empty cells are valid
		if (inputEnabled && board[i,j].getHighlighted() && board[i,j].isEmpty()) {
		
			//Debug.Log ("It is a valid move.");

			disableInput();
			HighlightOff();

			// animate the originating cell
			BoardCell origin = null;
			forEachCellNeighbour(i, j, (int ii, int jj) => {
				if (board[ii,jj].getState() == playerState[playerOnTurn]) {
					origin = board[ii,jj];
				}
			});

			if (origin) {

				// play dividing sound
				if (MusicManagerSingleton.Instance.soundsOn()) {
					audio.clip = divisionAudio;
					audio.Play();
				}

				origin.Split(board[i,j]);
				yield return StartCoroutine(GlobalAnimationTimer.WaitForAnimationEnd());
			}

			// new bacteria here, please!
			board[i,j].Appear(playerState[playerOnTurn]);
			
			// convert neighbours
			forEachCellNeighbour(i, j, (int ii, int jj) => {
				if (board[ii,jj].getState() == playerState[1-playerOnTurn]) {
					board[ii,jj].Convert();
				}
			});
			yield return StartCoroutine(GlobalAnimationTimer.WaitForAnimationEnd());

			yield return StartCoroutine(nextTurn());

		}

		yield break;
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

	IEnumerator MakeAIMove() {
		Debug.Log("MakeAIMove");

		disableInput(); // TODO
		yield return new WaitForSeconds(0.5f);

		List<IntPair> possibleMoves = new List<IntPair>();

		forEachBoardCell((int i, int j) => {
			if (board[i,j].getHighlighted())
				possibleMoves.Add(new IntPair(i,j));
		});

		if (possibleMoves.Count > 0) {
			int r = Random.Range(0, possibleMoves.Count);
			enableInput(); // TODO
			StartCoroutine(playerSelected(possibleMoves[r].i, possibleMoves[r].j));

			Debug.Log("MakeAIMove " + possibleMoves[r].i + " " + possibleMoves[r].j);
		}

		yield break;
	}

	public int getScore(BoardCellState player) {
		return scoreCount[(int)player];
	}

	public void disableInput() {
		inputEnabled = false;
	}

	public void enableInput() {
		inputEnabled = true;
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
							board[ni,nj].Spawn(boundary);
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
