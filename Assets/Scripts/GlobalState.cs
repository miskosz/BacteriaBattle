using UnityEngine;
using System.Collections;

static class GlobalState {

	// starting player, 0 or 1
	static BoardCellState startingPlayer;

	static GlobalState() {
		startingPlayer = BoardCellState.Player1;
	}

	public static BoardCellState GetStartingPlayer() {
		startingPlayer = startingPlayer.Opponent();
		return startingPlayer;
	}

	// 0: 2-player, 1: AI
	public static int gameMode = 0;

	// 3 boards
	public static int board = 0;
}
