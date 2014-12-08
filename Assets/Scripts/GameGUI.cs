using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {

	public BoardBuilder boardBuilder;
	
	public int labelBoxWidth = 40;
	public int labelBoxHeight = 40;

	public GUIStyle myStyle;

	Transform player1SpriteTransform, player2SpriteTransform;

	void Start() {
		player1SpriteTransform = transform.FindChild("Player1ScoreSprite");
		player2SpriteTransform = transform.FindChild("Player2ScoreSprite");

		Vector3 tmp;
		tmp = Camera.main.ScreenToWorldPoint(new Vector3 (0, Screen.height, 0));
		player1SpriteTransform.position = new Vector3(tmp.x, tmp.y, 0);
		player1SpriteTransform.localRotation = Quaternion.Euler(0, 0, 180f);

		tmp = Camera.main.ScreenToWorldPoint(new Vector3 (Screen.width, 0, 0));
		player2SpriteTransform.position = new Vector3(tmp.x, tmp.y, 0);
	}

	void OnGUI() {
	
		GUI.Label(new Rect(Screen.width - labelBoxWidth, Screen.height - labelBoxHeight, labelBoxWidth, labelBoxHeight),
		          boardBuilder.getScore(BoardCellState.Player2).ToString(), myStyle);
		
		GUIUtility.RotateAroundPivot(-180, new Vector2(0,0));
		GUI.Label(new Rect(-labelBoxWidth, -labelBoxHeight, labelBoxWidth, labelBoxHeight),
		          boardBuilder.getScore(BoardCellState.Player1).ToString(), myStyle);



	}

}
