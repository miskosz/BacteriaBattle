using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	int[,] boardSetup = {
		{1,4,4},
		{3,3,4},
		{3,3,3},
		{4,3,3},
		{4,4,0}
	};

	// Use this for initialization
	void Start () {
		Board testBoard = new Board(boardSetup);
		testBoard.MakeMove(1, 0);
	}
	
}
