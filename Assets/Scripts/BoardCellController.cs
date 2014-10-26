using UnityEngine;
using System.Collections;

public class BoardCellController : MonoBehaviour {

    public Sprite blueBacteriaSprite;
    public Sprite orangeBacteriaSprite;
    bool blueShowing = true;

    void OnMouseDown() {
        // switch the sprite
        blueShowing = !blueShowing;

        if (blueShowing)
			gameObject.GetComponent<SpriteRenderer>().sprite = blueBacteriaSprite;
        else
			gameObject.GetComponent<SpriteRenderer>().sprite = orangeBacteriaSprite;
    }
}
