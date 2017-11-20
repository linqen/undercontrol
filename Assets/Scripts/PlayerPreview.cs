using UnityEngine;

public class PlayerPreview : MonoBehaviour {
	public int charPreviewPos=0;
	public int playerNumber;
	public bool selected = false;

	public void SetPreview(int playerNumber,int charPreviewPos){
		this.playerNumber = playerNumber;
		this.charPreviewPos = charPreviewPos;
		transform.Find ("PIndicator").GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite>("PlayersIndicators/" + "P" + playerNumber + "Indicator");
	}
	public void SetCharPreview(int charPreviewPos){
		this.charPreviewPos = charPreviewPos;
	}

}
