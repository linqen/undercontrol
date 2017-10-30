using UnityEngine;

public class PlayerLife : MonoBehaviour {
	public GameManager gameManager;

	private PlayerMovement playerMovement;
	private int playerNumber;
	private Color color;
	private bool hasShield = true;
	private int lastHitByPlayerNumber=0;
	// Use this for initialization
	void Start () {
		gameManager = GameManager.Instance;
		playerNumber = GetComponent<PlayerPreview> ().playerNumber;
		playerMovement = GetComponent<PlayerMovement> ();
	}
	public void NotifyHit(int hittedByPlayerNumber){
		if (hittedByPlayerNumber != 0 && hittedByPlayerNumber != playerNumber) {
			lastHitByPlayerNumber = hittedByPlayerNumber;
		}
		if (hasShield) {
			hasShield = false;
		}else if (!hasShield) {
			gameManager.ReportDeath (gameObject,lastHitByPlayerNumber);
		}
	}


	public void ResetPlayer(){
		hasShield = true;
		lastHitByPlayerNumber = 0;
		//Reset animations positions to default
		playerMovement.ResetAnimationStates();
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag.Equals ("LevelLimit")) {
			gameManager.ReportDeath (gameObject,lastHitByPlayerNumber);
		}
	}
}
