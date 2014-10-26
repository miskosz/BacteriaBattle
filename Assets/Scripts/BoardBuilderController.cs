using UnityEngine;
using System.Collections;

public class BoardBuilderController : MonoBehaviour {

	// game board (TODO: state structure)
	int boardSize = 4;
	int[,] board;
	Vector3 iVector = new Vector3(Mathf.Cos(-Mathf.PI/6),Mathf.Sin(-Mathf.PI/6),0);
	Vector3 jVector = new Vector3(0,1,0);

	public Transform boardCellPrefab;

	void Start () {
		// init the board
		board = new int[2*boardSize-1, 2*boardSize-1];

		for (int i = 0; i < 2*boardSize-1; i++) {
			for (int j = 0; j < 2*boardSize-1; j++) {

				// cut off corners
				if (i-j >= boardSize || j-i >= boardSize)
					board[i,j] = 0; // set empty
				else
					board[i,j] = 1; // set cell of type 1
			}
		}

		// instantiate board cells
		Debug.Log("here" + jVector);
		
		for (int i = 0; i < 2*boardSize-1; i++) {
			for (int j = 0; j < 2*boardSize-1; j++) {
				if (board[i,j] == 1) {
					Vector3 position = (i-boardSize+1)*iVector + (j-boardSize+1)*jVector;
					Instantiate(boardCellPrefab, position, Quaternion.identity);
				}
			}
		}
	}

}
