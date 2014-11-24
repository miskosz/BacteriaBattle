using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {

	public BoardBuilder boardBuilder;

	public int orangex;
	public int orangey;

	public int bluex;
	public int bluey;

	void OnGUI() {
	
		GUI.Label(new Rect(orangex, Screen.height-orangey, 100, 100),
		          "<color=#f3c76f><size=50>"+boardBuilder.getScore(BoardCellState.Orange)+"</size></color>");
		
		GUIUtility.RotateAroundPivot(-180, new Vector2(1, 1));
		GUI.Label(new Rect(-Screen.width+bluex, bluey, 100, 100),
		          "<color=#a6d2d1><size=50>"+boardBuilder.getScore(BoardCellState.Blue)+"</size></color>");


	}

}
