using UnityEngine;

public class PlayerPreview {
	public Sprite charPreview;
	public int playerNumber;
	public bool selected = false;

	public PlayerPreview(int playerNumber,Sprite charPreview){
		this.playerNumber = playerNumber;
		this.charPreview = charPreview;
	}
}
