using UnityEngine;
using System.Collections;
public class PlayerLife : MonoBehaviour {
	public GameManager gameManager;
	public float deathAnimationTime;

	private PlayerMovement playerMovement;
	private int playerNumber;
	private Color color;
	private bool hasShield = true;
	private int lastHitByPlayerNumber=0;
	private Animator shieldAnimator;
	private Animator animator;
	private Coroutine deathCall=null;
	// Use this for initialization
	void Awake () {
		gameManager = GameManager.Instance;
		playerNumber = GetComponent<PlayerPreview> ().playerNumber;
		playerMovement = GetComponent<PlayerMovement> ();
		shieldAnimator = transform.Find ("Shield").GetComponent<Animator> ();
		animator = GetComponent<Animator> ();
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
			if (deathCall == null) {
				deathCall = StartCoroutine (Death ());
			}
		}
	}
	private IEnumerator Death(){
		GetComponent<Rigidbody2D> ().isKinematic = true;
		GetComponent<Rigidbody2D> ().velocity = new Vector2(0,0);
		GetComponent<PlayerMovement> ().enabled = false;
		GetComponent<GrenadeThrowing> ().enabled = false;
		animator.SetBool ("Death",true);
		yield return new WaitForSeconds (deathAnimationTime);
		gameManager.ReportDeath (gameObject,lastHitByPlayerNumber);
	}

	public void RecoverShield(){
		hasShield = true;
		shieldAnimator.SetBool ("hasShield", hasShield);
		shieldAnimator.GetComponent<Renderer> ().enabled = hasShield;
	}

	public void ResetPlayer(){
		GetComponent<Rigidbody2D> ().isKinematic = false;
		GetComponent<PlayerMovement> ().enabled = true;
		GetComponent<GrenadeThrowing> ().enabled = true;
		animator.SetBool ("Death",false);
		deathCall = null;
		hasShield = true;
		shieldAnimator.SetBool ("hasShield", hasShield);
		shieldAnimator.GetComponent<Renderer> ().enabled = hasShield;
		lastHitByPlayerNumber = 0;
		//Reset animations positions to default
		playerMovement.ResetMovement();
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag.Equals ("LevelLimit")) {
			if (deathCall == null) {
				deathCall = StartCoroutine (Death ());
			}
		}
	}
	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag.Equals ("Laser")) {
			if (deathCall == null) {
				deathCall = StartCoroutine (Death ());
			}
		}
	}
}
