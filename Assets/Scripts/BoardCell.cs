using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public enum BoardCellState {Blue, Orange, Empty};

public class BoardCell : MonoBehaviour {

    public Sprite blueBacteriaSprite;
	public Sprite orangeBacteriaSprite;
	public Sprite highlightedSprite;

	public Color blueTint;
	public Color orangeTint;

	// cell state
	BoardCellState state;

	// TODO: currently only empty cell can be highlighted,
	// I do not know how to render two sprites independently.
	bool highlighted = false;

	// cell coordinates
	int iPos, jPos;

	// board builder
	BoardBuilder boardBuilder;

	// renderer reference
	SpriteRenderer spriteRenderer;
	
	// called from BoardCellController when creating the board
	public void Initialize(BoardBuilder parent, BoardCellState _state, int _iPos, int _jPos) {
		boardBuilder = parent;
		state = _state;
		iPos = _iPos;
		jPos = _jPos;
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = 0;
		spriteRenderer.sortingLayerName = "Board";
		UpdateImage ();
	}

    void OnMouseDown() {
        // notify BoardBuilder
		boardBuilder.playerSelected(iPos, jPos);
    }

	void UpdateImage() {
		if (state == BoardCellState.Blue)
			spriteRenderer.sprite = blueBacteriaSprite;
		else if (state == BoardCellState.Orange)
			spriteRenderer.sprite = orangeBacteriaSprite;
		else if (highlighted)
			spriteRenderer.sprite = highlightedSprite;
		else
			spriteRenderer.sprite = null;
	}

	// interface for highlighting
	public bool getHighlighted() { return highlighted; }
	public void setHighlighted(bool _highlighted = true) {
		highlighted = _highlighted;

		if (highlighted)
			spriteRenderer.color = (boardBuilder.playerOnTurn == 0 ? blueTint : orangeTint);
		else
			spriteRenderer.color = Color.white;

		UpdateImage ();
	}

	// interface for state change
	public BoardCellState getState() { return state; }
	public void setState(BoardCellState _state) {
		state = _state;
		UpdateImage ();
	}

	// helper function
	public bool isEmpty() { return state == BoardCellState.Empty; }


}
