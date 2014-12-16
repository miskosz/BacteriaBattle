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

}
