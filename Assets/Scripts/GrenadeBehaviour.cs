using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour {
	public float explodeTime;
	public float explosionForcePlayers;
	public float explosionForceGrenades;
	public float dieTime;
	public float timeExploding;
	public float radius;
	public Sprite[] grenades;

	Animator animator;
	AudioManager audioManager;
	float timeLived;
	int throwedByPlayerNumber;
	int grenadeOfCharacterNumber;
	bool explode=false;
	Vector2 oldPos;
	Rigidbody2D myRigid;
	SpriteRenderer spriteRender;

	void Awake(){
		spriteRender = GetComponent<SpriteRenderer> ();
		myRigid = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
	}

	void Start () {
		audioManager = AudioManager.Instance;
		timeLived = 0.0f;
		oldPos = transform.position;
		audioManager.GrenadeExplode ();
		//spriteRender.sprite = grenades [grenadeOfCharacterNumber-1];
		animator.SetInteger("CharacterNumber", grenadeOfCharacterNumber);
	}
	
	void Update () {
		//To avoid teleport between colliders
		RaycastHit2D hit = Physics2D.Linecast(oldPos, transform.position);
		if(hit!=null&&((hit.collider.CompareTag("Ground")||hit.collider.CompareTag("Wall")))){
			transform.position = hit.point;
			myRigid.velocity=Vector2.Reflect(myRigid.velocity,hit.normal);
		}
		oldPos = transform.position;
		//

		timeLived += Time.deltaTime;
		if (timeLived >= explodeTime) {
			if (explode == false) {
				Explode ();
				GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, 0);
			}
			explode = true;
		}
		if (timeLived >= explodeTime + dieTime) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag.Equals ("Laser")&&explode==false) {
			timeLived = explodeTime;
		}
	}

	void Explode(){
		animator.SetBool("Twinkle", false);
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, radius);
		for (int i = 0; i < cols.Length; i++) {
			Rigidbody2D rigid = cols[i].GetComponent<Rigidbody2D> ();
			if (rigid != null) {
				Vector3 explodeDirection = rigid.transform.position - transform.position;
				float hitPowerForce = radius-explodeDirection.magnitude;
				if (cols[i].tag.Equals ("Player")) {
					cols[i].GetComponent<PlayerLife> ().NotifyHit (throwedByPlayerNumber);
					cols[i].GetComponent<PlayerMovement> ().AddExplosionForce (explodeDirection.normalized * hitPowerForce, timeExploding, explosionForcePlayers);
				} else if(cols[i].tag.Equals("Grenade")) {
					rigid.AddForce (explodeDirection.normalized * hitPowerForce * explosionForceGrenades, ForceMode2D.Impulse);
				}
			}
		}
	}

	public int GrenadeOfCharacterNumber {
		get {
			return this.grenadeOfCharacterNumber;
		}
		set {
			grenadeOfCharacterNumber = value;
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
