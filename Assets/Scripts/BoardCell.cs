using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public enum BoardCellState {Blue, Orange, Empty};

public class BoardCell : MonoBehaviour {

    public Sprite blueBacteriaSprite;
	public Sprite orangeBacteriaSprite;
	public Sprite highlightedSprite;

	// cell state
	BoardCellState state;

	// TODO: currently only empty cell can be highlighted,
	// I do not know how to render two sprites independently.
	bool highlighted = false;

	// cell coordinates
	int iPos, jPos;

	// board builder
	BoardBuilder boardBuilder;

	// called from BoardCellController when creating the board
	public void Initialize(BoardBuilder parent, BoardCellState _state, int _iPos, int _jPos) {
		boardBuilder = parent;
		state = _state;
		iPos = _iPos;
		jPos = _jPos;
		UpdateImage ();
	}

    void OnMouseDown() {
        // switch the sprite
		// state = (BoardCellState)(((int)state + 1) % 3);
		// UpdateImage ();

		boardBuilder.playerSelected (iPos, jPos);
    }

	void UpdateImage() {
		if (state == BoardCellState.Blue)
			gameObject.GetComponent<SpriteRenderer>().sprite = blueBacteriaSprite;
		else if (state == BoardCellState.Orange)
			gameObject.GetComponent<SpriteRenderer>().sprite = orangeBacteriaSprite;
		else if (highlighted)
			gameObject.GetComponent<SpriteRenderer>().sprite = highlightedSprite;
		else
			gameObject.GetComponent<SpriteRenderer>().sprite = null;
	}

	// interface for highlighting
	public bool getHighlighted() { return highlighted; }
	public void setHighlighted(bool _highlighted = true) {
		highlighted = _highlighted;
		UpdateImage ();
	}

	// interface for state change
	public BoardCellState getState() { return state; }
	public void setState(BoardCellState _state) {
		state = _state;
		UpdateImage ();
	}

}
