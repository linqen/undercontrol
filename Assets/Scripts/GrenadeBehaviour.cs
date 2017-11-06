using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour {
	public float explodeTime;
	public float explosionForce;
	public float dieTime;
	public float timeExploding;

	AudioManager audioManager;
	float timeLived;
	int throwedByPlayerNumber;
	CircleCollider2D circleCollider;
	bool explode=false;

	void Start () {
		audioManager = AudioManager.Instance;
		timeLived = 0.0f;
		circleCollider = GetComponent<CircleCollider2D> ();
	}
	
	void Update () {
		timeLived += Time.deltaTime;
		if (timeLived >= explodeTime) {
			if (explode == false) {
				audioManager.GrenadeExplode ();
			}
			explode = true;
			transform.GetComponentInChildren<ExplosionEffect> ().StartSwap ();
			circleCollider.enabled = false;
		}
		if (timeLived >= explodeTime + dieTime) {
			explode = false;
			Destroy (gameObject);
		}
	}
		
	void OnTriggerExit2D(Collider2D col){
		if (explode) {
			Rigidbody2D rigid = col.GetComponent<Rigidbody2D> ();
			if (rigid != null) {
				Vector3 explodeDirection = rigid.transform.position - transform.position;
				circleCollider.enabled = true;
				float hitPowerForce = circleCollider.bounds.extents.magnitude-explodeDirection.magnitude;
				if (col.tag.Equals ("Player")) {
					col.GetComponent<PlayerLife> ().NotifyHit (throwedByPlayerNumber);
					col.GetComponent<PlayerMovement> ().AddExplosionForce (explodeDirection.normalized * hitPowerForce, timeExploding, explosionForce);
				} else {
					rigid.AddForce (explodeDirection.normalized * hitPowerForce * explosionForce, ForceMode2D.Impulse);
				}
				circleCollider.enabled = false;
			}
		}
	}
	public int ThrowedByPlayerNumber {
		get {
			return this.throwedByPlayerNumber;
		}
		set {
			throwedByPlayerNumber = value;
		}
	}
}
