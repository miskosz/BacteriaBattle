using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {

	public BoardBuilder boardBuilder;
	public GUIStyle myStyle;

	Transform player1SpriteTransform, player2SpriteTransform;
	Transform scoreBox;

	void Start() {

		// position the score labels correctly
		// the sprites are children of this object

		player1SpriteTransform = transform.FindChild("Player1ScoreSprite");
		player2SpriteTransform = transform.FindChild("Player2ScoreSprite");
		scoreBox = transform.FindChild("ScoreBox");

		Vector3 tmp;
		tmp = Camera.main.ScreenToWorldPoint(new Vector3 (0, Screen.height, 0));
		player1SpriteTransform.position = new Vector3(tmp.x, tmp.y, 0);
		player1SpriteTransform.localRotation = Quaternion.Euler(0, 0, 180f);

		tmp = Camera.main.ScreenToWorldPoint(new Vector3 (Screen.width, 0, 0));
		player2SpriteTransform.position = new Vector3(tmp.x, tmp.y, 0);
	}

	void OnGUI() {

		Vector2 boxOrigin = WorldToGUIVector(scoreBox.position);
		Vector2 boxSize = WorldToGUIVector(scoreBox.position + scoreBox.localScale) - boxOrigin;
		boxSize = new Vector2(boxSize.x, -boxSize.y); // don't ask. it's necessary.

		myStyle.fontSize = (int)boxSize.y;

		GUI.Label(new Rect(boxOrigin.x, boxOrigin.y, boxSize.x, boxSize.y),
		          boardBuilder.getScore(BoardCellState.Player2).ToString(), myStyle);

		GUIUtility.RotateAroundPivot(-180, new Vector2(0,0));
		GUI.Label(new Rect(boxOrigin.x-Screen.width, boxOrigin.y-Screen.height, boxSize.x, boxSize.y),
		          boardBuilder.getScore(BoardCellState.Player1).ToString(), myStyle);
	}

	// Unity is super stupid about coordinate spaces
	Vector2 WorldToGUIVector(Vector3 worldVector) {
		Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldVector);
		float x = screenPoint.x;
		float y = Screen.height - screenPoint.y;
		return new Vector2 (x, y);
	}
}
