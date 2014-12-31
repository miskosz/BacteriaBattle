using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum BoardCellState {Player1 = 0, Player2 = 1, Empty = 2, OutOfBoard = 3};
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
	public Action(ActionType type, IntPair cell) : this(type, cell, new IntPair (-1, -1)) {}
	public Action(ActionType type, IntPair cell, IntPair splitCell) {
		this.type = type;
		this.cell = cell;
		this.splitCell = splitCell; // origin of split
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

	// scores
	public int[] scoreCount = {0, 0};


	// constructors

	// copy	constructor
	public Board(Board b) {
		playerOnTurn = b.playerOnTurn;
		board = new BoardCellState[b.board.GetLength(0),b.board.GetLength(1)];
		Array.Copy(b.board, board, board.Length);
		updateScore();
	}

	public Board(int[,] boardSetup, BoardCellState playerOnTurn = BoardCellState.Player1) {

		this.playerOnTurn = playerOnTurn;

		// init the board from boardSetup
		board = new BoardCellState[boardSetup.GetLength(0),boardSetup.GetLength(1)];
		for (int i = 0; i < board.GetLength(0); i++) {
			for (int j = 0; j < board.GetLength(1); j++) {
				board[i,j] = (BoardCellState) boardSetup[i,j];
			}
		}

		updateScore();
	}


	////////////////////////////////////////////////////////////////////////////
	/// Game Logic
	////////////////////////////////////////////////////////////////////////////

	// Return a board where the player on turn moved to (i,j).
	// Return null if the move is not to an empty cell.
	// Does not check if the move is otherwise invalid.
	Board SimulateMove(int i, int j) {

		// check if the cell is empty
		if (board[i,j] != BoardCellState.Empty)
			throw new Exception("The move destination cell is not empty!");
			//return null;

		// create a new board and place the player on (i,j)
		Board nextBoard = new Board(this);
		nextBoard.board[i,j] = playerOnTurn;
		nextBoard.playerOnTurn = playerOnTurn.Opponent();

		// convert neighbours
		ForEachCellNeighbour(i, j, (int ii, int jj) => {
			if (board[ii,jj] == playerOnTurn.Opponent()) {
				nextBoard.board[ii,jj] = playerOnTurn;
			}
		});

		// fill closed spaces
		nextBoard.FillClosedAreas();

		return nextBoard;
	}

	public List<Action> MakeMove(int i, int j) {

		//Debug.Log("Move to " + i + " " + j);

		// simulate the move
		Board nextBoard = SimulateMove(i,j);

		// prepare action messages
		List<Action> actions = new List<Action>();

		ForEachBoardCell((int ii, int jj) => {
			if (board[ii,jj] != nextBoard.board[ii,jj]) {

				IntPair cell = new IntPair(ii, jj);

				if (i == ii && j == jj) {
					// it is a split, find split cell
					IntPair splitCell = new IntPair(-1,-1);
					ForEachCellNeighbour(ii, jj, (int si, int sj) => {
						if (board[si,sj] == playerOnTurn) {
							splitCell = new IntPair(si,sj);
						}
					});

					// if no cell found spawn instead
					if (splitCell.i == -1)
						actions.Add(new Action(ActionType.Spawn, cell));
					else
						actions.Add(new Action(ActionType.Split, cell, splitCell)); // split
				}
				else if (board[ii,jj] == BoardCellState.Empty)
					actions.Add(new Action(ActionType.Spawn, cell)); // spawn
				else
					actions.Add(new Action(ActionType.Convert, cell)); // convert
			}
		});

		// update the board
		// just to be sure that I do not have a memory leak copy manually
		playerOnTurn = nextBoard.playerOnTurn;
		ForEachBoardCell((int ii, int jj) => {
			board[ii,jj] = nextBoard.board[ii,jj];
		});

		// update score
		updateScore();

		return actions;
	}

	// set highlighted cells
	// returns if layer has move
	public List<IntPair> GetPossibleMoves() {

		List<IntPair> moves = new List<IntPair>();

		ForEachBoardCell((int i, int j) => {
			
			if (board[i,j] == BoardCellState.Empty) {
				// iterate over cell neighbours
				ForEachCellNeighbour(i, j, (int ii, int jj) => {
					if (board[ii,jj] == playerOnTurn) {
						moves.Add(new IntPair(i, j));
					}
				});
			}

		});
		
		return moves;
	}

	// Score counting
	public void updateScore() {
		int[] tempScore = {0, 0};
		
		ForEachBoardCell((int i, int j) => {
			if (board[i,j] != BoardCellState.Empty)
				tempScore[(int)board[i,j]]++;
		});
		
		scoreCount = tempScore;
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

	////////////////////////////////////////////////////////////////////////////
	/// Board utilities
	////////////////////////////////////////////////////////////////////////////

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

	////////////////////////////////////////////////////////////////////////////
	/// AI
	////////////////////////////////////////////////////////////////////////////

	public struct AIScoreBFS {
		public IntPair pos;
		public BoardCellState player;
		public int dist;
	}
	
	public int AIScore() {
		// +4 for my bacteria
		// +2 for closer field
		// +1 for equally far if my turn

		// bfs for closer fields
		Queue<AIScoreBFS> bfs = new Queue<AIScoreBFS>();
		AIScoreBFS[,] closer = new AIScoreBFS[board.GetLength(0),board.GetLength(1)];

		ForEachBoardCell((int i, int j) => {
			// init with out of board
			// pos field is irrelevant
			closer[i,j] = new AIScoreBFS {pos = new IntPair(i,j), player = BoardCellState.OutOfBoard, dist = 9999};

			// init bfs
			if (board[i,j] != BoardCellState.Empty)
				bfs.Enqueue(new AIScoreBFS {pos = new IntPair(i,j), player = board[i,j], dist = 0});
		});

		// breadth first search
		while (bfs.Count != 0) {
			AIScoreBFS next = bfs.Dequeue();
			int i = next.pos.i, j = next.pos.j;

			if (next.dist <= closer[i,j].dist) {
				if (next.dist == closer[i,j].dist) {
					if (next.player == closer[i,j].player || closer[i,j].player == BoardCellState.Empty)
						continue;
					closer[i,j].player = BoardCellState.Empty;
				}
				else {
					closer[i,j] = next;
				}

				ForEachCellNeighbour(i, j, (int ii, int jj) => {
					bfs.Enqueue(new AIScoreBFS {pos = new IntPair(ii,jj), player = next.player, dist = next.dist+1});
				});
			}
		}

		// BFS is done
		int[] score = {0,0};
		for (int p = 0; p < 2; p++)
			score[p] = scoreCount[p] * 2;

		ForEachBoardCell((int i, int j) => {
			if (closer[i,j].player == BoardCellState.Empty)
				score[(int)playerOnTurn] += 1;
			else
				score[(int)closer[i,j].player] += 2;
		});

		return score[(int)playerOnTurn] - score[(int)playerOnTurn.Opponent()];
	}

	public class AIMinimax {
		public IntPair pos;
		public int score;
		public int rand;
	}

	public IntPair GetAIMove() {

		// strategy 1: try all moves and choose the one with best AIscore

		List<IntPair> possibleMoves = GetPossibleMoves();

		// self-explanatory
		if (possibleMoves.Count == 0)
			throw new Exception("No moves for AI.");
		else if (possibleMoves.Count == 1)
			return possibleMoves[0];

		List<AIMinimax> scoredMoves = new List<AIMinimax>();

		// The score of the simulated move is calculated for the opponent.
		// Therefore we want to minimize it.
		// Randomize equal scores a bit.
		foreach (IntPair move in possibleMoves) {
			Board sim = SimulateMove(move.i, move.j);
			scoredMoves.Add(new AIMinimax {pos = move, score = sim.AIScore(), rand = UnityEngine.Random.Range(0,100)});
		}

		// sort moves
		scoredMoves.Sort((x,y) => (10000*x.score+x.rand).CompareTo(10000*y.score+y.rand));

		// normalize
		// this is just plain heuristics

		// keep cBest best options
		// the worst option gets score 1, better gets more
		// just weigh best cBest options w.r.t. normalized weighths
		int baseScore = scoredMoves[scoredMoves.Count-1].score + 1;
		int cBest = Math.Min(3, scoredMoves.Count);
		scoredMoves = scoredMoves.GetRange(0, cBest);
		int randPool = 0;

		foreach (AIMinimax move in scoredMoves) {
			move.score = baseScore - move.score;
			move.score = move.score*move.score; // square
			randPool += move.score;
		}

		int rand = UnityEngine.Random.Range(0,randPool);
		int m = 0;
		while (scoredMoves[m].score < rand) {
			rand -= scoredMoves[m].score;
			m++;
		}

		// TODO
		string dbg = "";
		foreach (AIMinimax move in scoredMoves)
			dbg += " " + move.score;
		Debug.Log("Normalized AIScores: " + dbg + "    Chosen: " + scoredMoves[m].score);

		return scoredMoves[m].pos;

//		int r = UnityEngine.Random.Range(0, possibleMoves.Count);
//		return new IntPair(possibleMoves[r].i, possibleMoves[r].j);
	}

}
