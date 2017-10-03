using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour {
	public float explodeTime;
	public float explosionForce;
	public float dieTime;
	float timeLived;
	CircleCollider2D circleCollider;
	bool explode=false;

	void Start () {
		timeLived = 0.0f;
		circleCollider = GetComponent<CircleCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		timeLived += Time.deltaTime;
		if (timeLived >= explodeTime) {
			explode = true;
			transform.GetComponentInChildren<ExplosionEffect> ().StartSwap ();
			circleCollider.enabled = false;
			//GetComponent<MeshRenderer> ().enabled = false;
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
				float xForce=explosionForce/(col.transform.position.x - transform.position.x );
				float yForce=explosionForce/(col.transform.position.y - transform.position.y);
				if (col.tag.Equals ("Player")) {
					float hitPowerForce = Mathf.Abs(xForce) + Mathf.Abs(yForce);
					if (10.0f<hitPowerForce ) {
						col.GetComponent<PlayerLife> ().NotifyHit (3);
					}else if (5.0f<hitPowerForce &&hitPowerForce  <10.0f ) {
						col.GetComponent<PlayerLife> ().NotifyHit (2);
					}else if (0f<hitPowerForce &&hitPowerForce  <5.0f ) {
						col.GetComponent<PlayerLife> ().NotifyHit (1);
					}


				}
				rigid.AddForce (new Vector2 (xForce,yForce), ForceMode2D.Impulse);
			}
		}
	}
}
