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

	void Start () {
		//instance = this;

		// compute layout vectors
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
				board[i,j].Initialize(state);
			}
		}
	}

}
