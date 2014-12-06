using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public enum BoardCellState {Player1, Player2, Empty};

public class BoardCell : MonoBehaviour {

    //public Sprite blueBacteriaSprite;
	//public Sprite orangeBacteriaSprite;
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

	// renderer & animator reference
	SpriteRenderer spriteRenderer;
	Animator animator;
	
	// called from BoardCellController when creating the board
	public void Initialize(BoardBuilder parent, BoardCellState _state, int _iPos, int _jPos) {
		boardBuilder = parent;
		iPos = _iPos;
		jPos = _jPos;
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = 0;
		spriteRenderer.sortingLayerName = "Board";

		animator = gameObject.GetComponent<Animator>();

		setState(_state);

	}

    void OnMouseDown() {
        // notify BoardBuilder
		StartCoroutine(boardBuilder.playerSelected(iPos, jPos));
    }

	void UpdateImage() {
		if (state == BoardCellState.Player1)
			PlayAnimation("Player1_Idle");
		else if (state == BoardCellState.Player2)
			PlayAnimation("Player2_Idle");
		else if (highlighted)
			PlayAnimation("Highlighted");
		else
			PlayAnimation("Empty");
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
		if (state != _state) {
			state = _state;
			UpdateImage ();
		}
	}

	// helper function
	public bool isEmpty() { return state == BoardCellState.Empty; }

	// trigger splitting animation
	public void Split(BoardCell dest) {
		// temporary
		// try rotations
		Vector3 direction = dest.transform.position - transform.position;
		transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
		dest.transform.rotation = transform.rotation;

		animator.SetTrigger("Split");
	}

	// unity triggers are weird
	void PlayAnimation(string animationName){
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
			animator.SetTrigger(animationName);
	}
}
