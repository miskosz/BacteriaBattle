using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BoardCellState {Player1 = 0, Player2 = 1, Empty = 3, OutOfBoard = 4};
public enum ActionType {Spawn, Split, Convert};

static class BoardCellStateExtensions {
	public static BoardCellState Opponent(this BoardCellState player) {
		return (BoardCellState)(1 - (int)player);
	}
}

// simple struct to store position
public struct IntPair {
	public IntPair(int i, int j) {
		this.i = i;
		this.j = j;
	}
	public int i, j;
};


public struct Action {
	public ActionType type;
	public IntPair cell, splitCell;
	public Action(ActionType type, IntPair cell) {
		this.type = type;
		this.cell = cell;
		this.splitCell = new IntPair(0,0);
	}
}

/**
 * Class Board stores game position and handles game logic.
 * Also implements the AI logic.
 **/
public class Board {

	// the game position
	public BoardCellState[,] board;

	// whose turn is it
	public BoardCellState playerOnTurn;

	// constructor
	public Board(int[,] boardSetup, BoardCellState playerOnTurn = BoardCellState.Player1) {

		this.playerOnTurn = playerOnTurn;

		// init the board from boardSetup
		board = new BoardCellState[boardSetup.GetLength(0),boardSetup.GetLength(1)];
		for (int i = 0; i < board.GetLength(0); i++) {
			for (int j = 0; j < board.GetLength(1); j++) {
				board[i,j] = (BoardCellState) boardSetup[i,j];
			}
		}
	}

	///
	/// Game Logic
	///

	// Return a board where the player on turn moved to (i,j).
	// Return null if the move is not to an empty cell.
	// Does not check if the move is otherwise invalid.
	Board SimulateMove(int i, int j) {

		// check if the cell is empty
		if (board[i,j] != BoardCellState.Empty)
			return null;

		// create a new board and place the player on (i,j)
		Board nextBoard = (Board)this.MemberwiseClone();
		nextBoard.board[i,j] = playerOnTurn;
		nextBoard.playerOnTurn = playerOnTurn.Opponent();

		// convert neighbours
		ForEachCellNeighbour(i, j, (int ii, int jj) => {
			if (board[ii,jj] == playerOnTurn.Opponent()) {
				board[ii,jj] = playerOnTurn;
			}
		});

		// fill closed spaces
		nextBoard.FillClosedAreas();

		return nextBoard;
	}

	public List<Action> MakeMove(int i, int j) {

		// simulate the move
		Board nextBoard = SimulateMove(i,j);

		// prepare action messages
		List<Action> actions = new List<Action>();

		ForEachBoardCell((int ii, int jj) => {
			if (board[ii,jj] != nextBoard.board[ii,jj]) {
				if (board[ii,jj] == BoardCellState.Empty)
					actions.Add(new Action(ActionType.Spawn, new IntPair(ii, jj)));
				else
					actions.Add(new Action(ActionType.Convert, new IntPair(ii, jj)));
			}
		});

		// update the board
		// just to be sure that I do not have a memory leak copy manually
		playerOnTurn = nextBoard.playerOnTurn;
		ForEachBoardCell((int ii, int jj) => {
			board[ii,jj] = nextBoard.board[ii,jj];
		});

		return actions;
	}
	
	
	// Let us say that an area of empty is WEAKLY CONNECTED if cells are
	// separated by at most one bacteria. A weakly connected area should
	// be filled if all of its boundary bacteria are of the same type.
	public void FillClosedAreas() {
		
		// detect weakly connected areas by a depth-first search
		
		int[,] visited = new int[board.GetLength(0),board.GetLength(1)];
		int areaId = 0;
		
		ForEachBoardCell((int i, int j) => {
			// find empty cell which was not visited
			if (board[i,j] == BoardCellState.Empty && visited[i,j] == 0) {
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
					
					ForEachCellNeighbour(pos.i, pos.j, (int ii, int jj) => {
						if (board[pos.i, pos.j] == BoardCellState.Empty || board[ii, jj] == BoardCellState.Empty)
							dfs.Push(new IntPair(ii, jj));
					});
					
					// store if we have seen bacteria
					if (board[pos.i, pos.j] != BoardCellState.Empty) {
						if (boundary == BoardCellState.Empty)
							boundary = board[pos.i, pos.j];
						else if (boundary != board[pos.i, pos.j])
							isUniform = false;
					}
				}
				
				// if boundary is uniform, fill the cells from area areaId
				if (isUniform) {
					ForEachBoardCell((int ni, int nj) => {
						if (visited[ni,nj] == areaId) {
							board[ni,nj] = boundary;
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
	
	bool OnBoard(int i, int j) {
		return (
			0 <= i && i < board.GetLength(0) &&
			0 <= j && j < board.GetLength(1) &&
			board[i,j] != BoardCellState.OutOfBoard
			);
	}
	
	// board iterator
	// usage:
	//     forEachBoardCell((i,j) => {
	//         Debug.Log("boardCell " + i + " " + j);
	//     });
	public delegate void CellDelegate(int i, int j);
	public void ForEachBoardCell(CellDelegate method)
	{
		for (int i = 0; i < board.GetLength(0); i++) {
			for (int j = 0; j < board.GetLength(1); j++) {
				if (OnBoard(i,j)) {
					method(i,j);
				}
			}
		}
	}
	
	// cell neighbourhood iterator
	public void ForEachCellNeighbour(int i, int j, CellDelegate method)
	{
		for (int d = 0; d < di.Length; d++) {
			// get neighbour coordinates
			int ii = i + di[d], jj = j + dj[d];
			if (OnBoard(ii, jj)) {
				method(ii, jj);
			}
		}
	}

}
