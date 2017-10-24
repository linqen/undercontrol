using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour {
	public float explodeTime;
	public float explosionForce;
	public float dieTime;
	float timeLived;
	int throwedByPlayerNumber;
	CircleCollider2D circleCollider;
	bool explode=false;

	void Start () {
		timeLived = 0.0f;
		circleCollider = GetComponent<CircleCollider2D> ();
	}
	
	void Update () {
		timeLived += Time.deltaTime;
		if (timeLived >= explodeTime) {
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
					if ((circleCollider.bounds.extents.magnitude / 1.3) < hitPowerForce) {
						col.GetComponent<PlayerLife> ().NotifyHit (throwedByPlayerNumber);
					} else if ((circleCollider.bounds.extents.magnitude / 1.8) < hitPowerForce && hitPowerForce < (circleCollider.bounds.extents.magnitude / 1.3)) {
						col.GetComponent<PlayerLife> ().NotifyHit (throwedByPlayerNumber);
					} else if (hitPowerForce < (circleCollider.bounds.extents.magnitude / 1.8)) {
						col.GetComponent<PlayerLife> ().NotifyHit (throwedByPlayerNumber);
					}
					StartCoroutine (col.GetComponent<PlayerMovement> ().AddExplosionForce (explodeDirection.normalized * hitPowerForce * explosionForce, 0.2f));
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
