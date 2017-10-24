using UnityEngine;

public class PlayerLife : MonoBehaviour {
	public GameManager gameManager;

	int playerNumber;
	Color color;
	Renderer rend;
	bool hasShield = true;
	int lastHitByPlayerNumber;
	// Use this for initialization
	void Start () {
		gameManager = GameManager.Instance;
		playerNumber = GetComponent<PlayerPreview> ().playerNumber;

		rend = GetComponent<Renderer> ();
		if (playerNumber == 1) {
			color.b = 1.0f;
			color.g = 0;
			color.r = 0;
		} else if (playerNumber == 2) {
			color.g = 1.0f;
			color.b = 0;
			color.r = 0;
		}else if (playerNumber == 3) {
			color.g = 0.4f;
			color.b = 0.6f;
			color.r = 0;
		}else if (playerNumber == 4) {
			color.g = 0.9f;
			color.b = 0.5f;
			color.r = 0;
		}

		rend.material.color = color;
	}
	public void NotifyHit(int hittedByPlayerNumber){
		if (hittedByPlayerNumber != 0) {
			lastHitByPlayerNumber = hittedByPlayerNumber;
		}
		if (hasShield) {
			hasShield = false;
		}else if (!hasShield) {
			gameManager.ReportDeath (gameObject,lastHitByPlayerNumber);
		}
	}


	public void ResetLife(){
		hasShield = true;
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag.Equals ("LevelLimit")) {
			gameManager.ReportDeath (gameObject,lastHitByPlayerNumber);
		}
	}
}
