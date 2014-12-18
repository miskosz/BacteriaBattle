using UnityEngine;
using System.Collections;

static class GlobalState {

	// starting player, 0 or 1
	static int startingPlayer;

	static GlobalState() {
		startingPlayer = 0;
	}

	public static int GetStartingPlayer() {
		startingPlayer = 1-startingPlayer;
		return startingPlayer;
	}

	// 0: 2-player, 1: AI
	public static int gameMode = 0;

	// 3 boards
	public static int board = 0;
}
