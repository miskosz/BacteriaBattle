using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public enum BoardCellState {Player1, Player2, Empty};

public class BoardCell : MonoBehaviour {

	// cell state
	BoardCellState state;

	// cell coordinates
	int iPos, jPos;

	// board builder
	BoardBuilder boardBuilder;

	// children references
	[HideInInspector]
	public CellAnimator cellAnim;
	Animator animator;
	CellHighlight cellHighlight;
	CellBackground cellBackground;

	// for getting animation length
	public AnimationClip splitAnimationClip;
	public AnimationClip convertAnimationClip;
	public AnimationClip spawnAnimationClip;

	// called from BoardCellController when creating the board
	public void Initialize(BoardBuilder parent, int _iPos, int _jPos) {
		boardBuilder = parent;
		iPos = _iPos;
		jPos = _jPos;
		state = BoardCellState.Empty;

		cellAnim = gameObject.GetComponentInChildren<CellAnimator>();
		animator = cellAnim.GetComponent<Animator>();

		cellHighlight = gameObject.GetComponentInChildren<CellHighlight>();

		cellBackground = gameObject.GetComponentInChildren<CellBackground>();
		cellBackground.Init(iPos, jPos);
	}

    void OnMouseDown() {
        // notify BoardBuilder
		StartCoroutine(boardBuilder.playerSelected(iPos, jPos));
    }

	// interface for highlighting
	public bool getHighlighted() { return cellHighlight.getHighlighted(); }
	public void setHighlighted(bool highlighted, BoardCellState _state = BoardCellState.Empty) {
		cellHighlight.setHighlighted(highlighted, _state);
	}

	// helper function
	public bool isEmpty() { return state == BoardCellState.Empty; }

	// interface for state change
	public BoardCellState getState() { return state; }


	//////////////////////////////////////////////////////////
	/// 
	///      STATE TRANSITIONS
	///
	//////////////////////////////////////////////////////////


	public void Appear(BoardCellState _state) {
		if (state != _state) {
			state = _state;
			if (state == BoardCellState.Player1)
				PlayAnimation("Player1_Idle");
			else if (state == BoardCellState.Player2)
				PlayAnimation("Player2_Idle");
		}
	}

	public void Spawn(BoardCellState _state) {
		if (state != _state) {
			state = _state;
			if (state == BoardCellState.Player1)
				PlayAnimation("Player1_Spawn");
			else if (state == BoardCellState.Player2)
				PlayAnimation("Player2_Spawn");
			GlobalAnimationTimer.AnimationTriggered(spawnAnimationClip);
		}
	}


	// convert bacteria
	public void Convert() {
		BoardCellState newState = (state == BoardCellState.Player1 ? BoardCellState.Player2 : BoardCellState.Player1);
		state = newState;
		animator.SetTrigger("Convert");
		GlobalAnimationTimer.AnimationTriggered(convertAnimationClip);
	}

	// trigger splitting animation
	public void Split(BoardCell dest) {
		// rotate cell
		Vector3 direction = dest.transform.position - transform.position;
		Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);

		cellAnim.transform.rotation = rotation;
		dest.cellAnim.transform.rotation = rotation;

		animator.SetTrigger("Split");
		GlobalAnimationTimer.AnimationTriggered(splitAnimationClip);
	}

	// unity triggers are weird
	void PlayAnimation(string animationName){
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
			animator.SetTrigger(animationName);

		//if (animationName != "Highlighted" && animationName != "Empty")
		//	Debug.Log(animationName + " " + iPos + " " + jPos);
		
	}
}
