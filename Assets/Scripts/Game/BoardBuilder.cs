using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardBuilder : MonoBehaviour {
	
	public GameObject boardCellPrefab;
	public float cellDistance = 1;

	public AudioClip divisionAudio;
	public AudioClip gameOverAudio;

	public PauseMenu pauseMenu;
	public WinnerLoser winnerLoser;

	int[] scoreCount = {0, 0};
	bool inputEnabled = false;
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
		//StartCoroutine(nextTurn());
	}

	/*IEnumerator nextTurn() {
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
			MusicManagerSingleton.Instance.playSound(gameOverAudio, audio);

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
			if (boardCells[i,j].isEmpty()) {
				// iterate over cell neighbours
				forEachCellNeighbour(i, j, (int ii, int jj) => {
					if (boardCells[ii,jj].getState() == player) {
						highlight = true;
						hasMove = true;
					}
				});
			}
			boardCells[i,j].setHighlighted(highlight, player); 
			
		});

		return hasMove;
	}*/

	void HighlightOff() {
		board.ForEachBoardCell((int i, int j) => {
			if (boardCells[i,j].getHighlighted()) {
				boardCells[i,j].setHighlighted(false); 
			}
		});
	}
	
	public IEnumerator playerSelected(int i, int j) {
		
		//Debug.Log ("Player selected " + i + " " + j);
		
		// only moves to highlighted empty cells are valid
		if (inputEnabled /*&& boardCells[i,j].getHighlighted()*/ && boardCells[i,j].isEmpty()) {
		
			//Debug.Log ("It is a valid move.");

			//disableInput();
			//HighlightOff();

			// TODO: playerOnTurn is switched by this!
			List<Action> actions = board.MakeMove(i,j);

			// Process the actions by type, in the order split - convert - spawn.
			// Doing foreach 3x does not kill us.

			// split
			foreach (Action action in actions) {
				if (action.type == ActionType.Split) {
					boardCells[action.splitCell.i,action.splitCell.j].Split(boardCells[action.cell.i,action.cell.j]);
					yield return StartCoroutine(GlobalAnimationTimer.WaitForAnimationEnd());
					boardCells[action.cell.i,action.cell.j].Appear(board.playerOnTurn.Opponent());
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
					boardCells[action.cell.i,action.cell.j].Spawn(board.playerOnTurn.Opponent());
				}
			}
			yield return StartCoroutine(GlobalAnimationTimer.WaitForAnimationEnd());
		}

		yield break;
	}

	// score counting
	public void updateScore() {
		int[] tempScore = {0, 0};

		board.ForEachBoardCell((int i, int j) => {
			if (!boardCells[i,j].isEmpty())
				tempScore[(int)boardCells[i,j].getState()]++;
		});
		
		scoreCount = tempScore;
	}

	/*IEnumerator MakeAIMove() {
		Debug.Log("MakeAIMove");

		disableInput(); // TODO
		yield return new WaitForSeconds(0.5f);

		List<IntPair> possibleMoves = new List<IntPair>();

		forEachBoardCell((int i, int j) => {
			if (boardCells[i,j].getHighlighted())
				possibleMoves.Add(new IntPair(i,j));
		});

		if (possibleMoves.Count > 0) {
			int r = Random.Range(0, possibleMoves.Count);
			enableInput(); // TODO
			StartCoroutine(playerSelected(possibleMoves[r].i, possibleMoves[r].j));

			Debug.Log("MakeAIMove " + possibleMoves[r].i + " " + possibleMoves[r].j);
		}

		yield break;
	}*/

	public int getScore(BoardCellState player) {
		return scoreCount[(int)player];
	}

	public void disableInput() {
		inputEnabled = false;
	}

	public void enableInput() {
		inputEnabled = true;
	}
	

}
