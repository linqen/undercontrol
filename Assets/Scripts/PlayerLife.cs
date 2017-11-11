using UnityEngine;

public class PlayerLife : MonoBehaviour {
	public GameManager gameManager;

	private PlayerMovement playerMovement;
	private int playerNumber;
	private Color color;
	private bool hasShield = true;
	private int lastHitByPlayerNumber=0;
	private Animator shieldAnimator;
	// Use this for initialization
	void Awake () {
		gameManager = GameManager.Instance;
		playerNumber = GetComponent<PlayerPreview> ().playerNumber;
		playerMovement = GetComponent<PlayerMovement> ();
		shieldAnimator = transform.Find ("Shield").GetComponent<Animator> ();
	}
	public void NotifyHit(int hittedByPlayerNumber){
		if (hittedByPlayerNumber != 0) {
			lastHitByPlayerNumber = hittedByPlayerNumber;
		}
		if (hasShield) {
			hasShield = false;
			shieldAnimator.SetBool ("hasShield", hasShield);
			shieldAnimator.GetComponent<Renderer> ().enabled = hasShield;
		}else if (!hasShield) {
			gameManager.ReportDeath (gameObject,lastHitByPlayerNumber);
		}
	}


	public void ResetPlayer(){
		hasShield = true;
		shieldAnimator.SetBool ("hasShield", hasShield);
		shieldAnimator.GetComponent<Renderer> ().enabled = hasShield;
		lastHitByPlayerNumber = 0;
		//Reset animations positions to default
		playerMovement.ResetMovement();
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag.Equals ("LevelLimit")) {
			gameManager.ReportDeath (gameObject,lastHitByPlayerNumber);
		}
	}
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag.Equals ("Laser")) {
			gameManager.ReportDeath (gameObject,lastHitByPlayerNumber);
		}
	}
}
