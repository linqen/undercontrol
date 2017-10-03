using UnityEngine;

public class PlayerPreview : MonoBehaviour {
	public Sprite charPreview;
	public int playerNumber;
	public bool selected = false;

	public void SetPreview(int playerNumber,Sprite charPreview){
		this.playerNumber = playerNumber;
		this.charPreview = charPreview;
	}
	public void SetCharPreview(Sprite charPreview){
		this.charPreview = charPreview;
	}
}
