using UnityEngine;
using System.Collections;

public class CellHighlight : MonoBehaviour {

	public Color Player1Tint;
	public Color Player2Tint;

	void Start() {
		setHighlighted(false);
	}

	public bool getHighlighted() { return renderer.enabled; }

	public void setHighlighted(bool highlighted, BoardCellState state = BoardCellState.Empty) {
		if (highlighted) {
			gameObject.GetComponent<SpriteRenderer>().color = (state == BoardCellState.Player1 ? Player1Tint : Player2Tint);
			renderer.enabled = true;
		}
		else {
			renderer.enabled = false;
		}
	}

}
