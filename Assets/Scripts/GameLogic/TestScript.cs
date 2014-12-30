using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	int[,] boardSetup = {
		{1,3,3},
		{2,2,3},
		{2,2,2},
		{3,2,2},
		{3,3,0}
	};

	// Use this for initialization
	void Start () {
		Board testBoard = new Board(boardSetup);
		testBoard.MakeMove(1, 0);
	}
	
}
