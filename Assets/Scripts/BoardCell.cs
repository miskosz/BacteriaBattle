using UnityEngine;
using System.Collections;

public enum BoardCellState {Blue, Orange, Empty};

public class BoardCell : MonoBehaviour {

    public Sprite blueBacteriaSprite;
    public Sprite orangeBacteriaSprite;
	BoardCellState state;

	// called from BoardCellController when creating the board
	public void Initialize(BoardCellState _state) {
		state = _state;
		UpdateImage ();
	}

    void OnMouseDown() {
        // switch the sprite
		state = (BoardCellState)(((int)state + 1) % 3);
		UpdateImage ();
    }

	void UpdateImage() {
		if (state == BoardCellState.Blue)
			gameObject.GetComponent<SpriteRenderer>().sprite = blueBacteriaSprite;
		else if (state == BoardCellState.Orange)
			gameObject.GetComponent<SpriteRenderer>().sprite = orangeBacteriaSprite;
		else
			gameObject.GetComponent<SpriteRenderer>().sprite = null;
	}
}
